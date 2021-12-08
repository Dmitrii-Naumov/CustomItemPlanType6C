using System;

using PX.Data;
using PX.Objects.IN;
using PX.Objects.IN.Overrides.INDocumentRelease;

namespace PX.Objects.SO
{
    public class SOOrderEntryCustomPlanExtension : PXGraphExtension<SOOrderEntry>
    //actually abstract class with necessary cache and event handlers
    {
        public PXSelect<INItemPlan> dummyitemplan;
        public override void Initialize()
        {
            Base.Caches<SiteStatus>().Interceptor = new SiteStatusAccumulatorSOTransfersAttribute();
            base.Initialize();
        }

        #region generic plan bucket update
        public virtual void InItemPlan_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            UpdateAllocatedQuantitiesWithCustomPlan((INItemPlan)e.Row, false);
            UpdateAllocatedQuantitiesWithCustomPlan((INItemPlan)e.OldRow, true);
        }

        public virtual void InItemPlan_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            UpdateAllocatedQuantitiesWithCustomPlan((INItemPlan)e.Row, false);
        }

        public virtual void InItemPlan_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            UpdateAllocatedQuantitiesWithCustomPlan((INItemPlan)e.Row, true);
        }

        public virtual void UpdateAllocatedQuantitiesWithCustomPlan(INItemPlan plan, bool revert = false)
        {
            //actually abstract method for further plan types extensions
        }
        #endregion

        #region  repeating logic from Parent_RowUpdated of the SOLineSplitPlanId attribute to add to hardcoded plan types
        private bool IsOrderOnHold(SOOrder order)
        {
            return (order != null) && ((order.Hold ?? false) || (order.CreditHold ?? false) || (!order.Approved ?? false) || (!order.PrepaymentReqSatisfied ?? false));
        }

        private string CalcPlanTypeWithoutBackOrdering(INItemPlan plan, SOLineSplit split, SOOrderType ordertype)
        {
            if (ordertype == null || ordertype.RequireShipping == true)
            {
                return (split.IsAllocated == true) ? split.AllocatedPlanType
                    : (plan.Hold ?? false) ? INPlanConstants.Plan69
                    : (split.RequireAllocation != true || split.IsStockItem != true) ? split.PlanType : split.BackOrderPlanType;
            }
            else
            {
                return (plan.Hold != true || split.IsStockItem != true) ? split.PlanType : INPlanConstants.Plan69;
            }
        }

        private string CalcPlanType(INItemPlan plan, SOLineSplit split, SOOrderType ordertype, bool canBackOrder = false)
        {
            string CalcedlanType = CalcPlanTypeWithoutBackOrdering(plan, split, ordertype);

            //copied from original code, but unlikely to be the case for custom plan type
            bool isAllocation = (split.IsAllocated == true) || INPlanConstants.IsAllocated(plan.PlanType) || INPlanConstants.IsFixed(plan.PlanType);

            if (canBackOrder && CalcedlanType != INPlanConstants.Plan69 && !isAllocation)
                return INPlanConstants.Plan68;

            return CalcedlanType;
        }

        // this is not supposed to be customized
        public virtual void UpdatefromCustomPlanInLinesImpl(SOOrder row, string customplantype, bool DatesUpdated, bool CustomerUpdated, bool useBackOrder = false)
        {
            if (customplantype == String.Empty)
                return;

            //cmd.WhereAnd<Where<INItemPlan.planType, Equal<Current<SOOrder.lastSiteID>>, And<INItemPlan.planDate, LessEqual<Current<SOOrder.lastShipDate>>>>>();
            PXCache plancache = Base.Caches[typeof(INItemPlan)];

            //temporary - take standard BackOrdered field
            /*
			SOOrderExtCustomAllocationType rowExt = row.GetExtension<SOOrderExtCustomAllocationType>();
			bool backordered = rowExt.UsrBackOrdered ?? false;
			*/
            bool backordered = row.BackOrdered ?? false;

            PXSelectBase<INItemPlan> cmd = new PXSelect<INItemPlan, Where<INItemPlan.refNoteID, Equal<Current<SOOrder.noteID>>>>(Base);

            //BackOrdered is tri-state
            if (backordered == true && row.LastSiteID != null && row.LastShipDate != null)
            {
                cmd.WhereAnd<Where<INItemPlan.siteID, Equal<Current<SOOrder.lastSiteID>>,
                    And<INItemPlan.planDate, LessEqual<Current<SOOrder.lastShipDate>>>>>();
            }

            //	foreach (INItemPlan plan in cmd.View.SelectMultiBound(new[] { e.Row }))

            foreach (INItemPlan plan in cmd.Select(new object[] { row.NoteID, customplantype }))
            {
                if ((row.Cancelled ?? false) || (row.Completed ?? false))
                {
                    plancache.Delete(plan);
                }
                else
                {
                    INItemPlan copy = PXCache<INItemPlan>.CreateCopy(plan);
                    if (DatesUpdated)
                    {
                        plan.PlanDate = (DateTime?)row.ShipDate;
                    }
                    if (CustomerUpdated)
                    {
                        plan.BAccountID = (int?)row.CustomerID;
                    }
                    plan.Hold = IsOrderOnHold((SOOrder)row);

                    // We should skip allocated plans. In general we should process only "normal" plans.
                    if (Base.soordertype.Current.RequireAllocation != true)
                    {
                        SOLineSplit split = PXSelect<SOLineSplit,
                        Where<SOLineSplit.orderType, Equal<Required<SOOrder.orderType>>,
                            And<SOLineSplit.orderNbr, Equal<Required<SOOrder.orderNbr>>,
                            And<SOLineSplit.planID, Equal<Required<SOLineSplit.planID>>>>>>.Select(Base, new object[]
                                { row.OrderType, row.OrderNbr, plan.PlanID });

                        if (split == null) break;

                        plan.PlanType =
                                CalcPlanType(plan, split, Base.soordertype.Current, useBackOrder && backordered);

                        if (!string.Equals(copy.PlanType, plan.PlanType))
                        {
                            plancache.RaiseRowUpdated(plan, copy);
                        }
                    }
                    plancache.MarkUpdated(plan);
                }
            }
        }

        // the event handler itself is not supposed to be overridden
        public virtual void SOOrder_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e, PXRowUpdated basem)
        {
            basem(sender, e);

            SOOrder row = e.Row as SOOrder;
            SOOrder oldRow = e.OldRow as SOOrder;
            if (row == null) return;

            PXView view;
            WebDialogResult answer = Base.Views.TryGetValue("Document", out view) ? view.Answer : WebDialogResult.None;
            bool DatesUpdated = !sender.ObjectsEqual<SOOrder.shipDate>(e.Row, e.OldRow) && (answer == WebDialogResult.Yes ||
                ((SOOrder)e.Row).ShipComplete != SOShipComplete.BackOrderAllowed);
            bool RequestOnUpdated = !sender.ObjectsEqual<SOOrder.requestDate>(e.Row, e.OldRow) && (answer == WebDialogResult.Yes || ((SOOrder)e.Row).ShipComplete != SOShipComplete.BackOrderAllowed);
            bool CreditHoldApprovedUpdated = !sender.ObjectsEqual<SOOrder.creditHold>(e.Row, e.OldRow) || !sender.ObjectsEqual<SOOrder.approved>(e.Row, e.OldRow);
            bool CustomerUpdated = !sender.ObjectsEqual<SOOrder.customerID>(e.Row, e.OldRow);

            // use standard backOrdered
            /*
            SOOrderExtCustomAllocationType rowExt = sender.GetExtension<SOOrderExtCustomAllocationType>(e.Row);
            SOOrderExtCustomAllocationType oldrowExt = sender.GetExtension<SOOrderExtCustomAllocationType>(e.OldRow);                      
            bool BackOrderedUpdated = !sender.ObjectsEqual<SOOrderExtCustomAllocationType.usrbackOrdered>(rowExt, oldrowExt);
            */
            bool BackOrderedUpdated = !sender.ObjectsEqual<SOOrder.backOrdered>(row, oldRow);

            if (CustomerUpdated || DatesUpdated || RequestOnUpdated || CreditHoldApprovedUpdated
                || BackOrderedUpdated //Duplicate Back Ordered field
                || !sender.ObjectsEqual<SOOrder.hold, SOOrder.cancelled, SOOrder.completed,
                //	SOOrderExtCustomAllocationType.usrbackOrdered,  
                SOOrder.shipComplete, SOOrder.prepaymentReqSatisfied>(e.Row, e.OldRow))
            {
                DatesUpdated |= !sender.ObjectsEqual<SOOrder.shipComplete>(e.Row, e.OldRow) && ((SOOrder)e.Row).ShipComplete != SOShipComplete.BackOrderAllowed;
                UpdatefromCustomPlanInLInes(row, DatesUpdated, CustomerUpdated);
            }



            // SOOrder.BackOrdered value should be handled only single time and only in this method
            //sender.SetValue<SOOrderExtCustomAllocationType.usrbackOrdered>(e.Row, null);
        }

        //this method should be overridden using base method invoke - it adds logic for the current custom plan type
        public virtual void UpdatefromCustomPlanInLInes(SOOrder row, bool DatesUpdated, bool CustomerUpdated)
        {
            UpdatefromCustomPlanInLinesImpl(row, String.Empty, DatesUpdated, CustomerUpdated, false);  // blank second argument means the method won't run
        }


        #endregion
    }

    
}