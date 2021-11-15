using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN.Overrides.INDocumentRelease;

namespace PX.Objects.IN
{
    public class INIntegrityCheckExt6TPlan : PXGraphExtension<INIntegrityCheck>
    {
        public override void Initialize()
        {
            Base.Caches<SiteStatus>().Interceptor = new SiteStatusAccumulatorSOTransfersAttribute();
            base.Initialize();
        }
        private TNode UpdateAllocatedQuantities<TNode>(INItemPlan plan, INPlanType plantype, bool InclQtyAvail)
            where TNode : class, IQtyAllocatedBase
        {
            INPlanType targettype = INItemPlanIDAttribute.GetTargetPlanTypeBase<TNode>(Base, plan, plantype);
            if (typeof(TNode) != typeof(SiteStatus))
                return INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<TNode>(Base, plan, targettype, InclQtyAvail);
            else
            {
                TNode statuscurr = INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<TNode>(Base, plan, targettype, InclQtyAvail);
                Base.Caches[typeof(SiteStatus)].Current = statuscurr;
                UpdateAllocatedQuantitiesWithCustomPlan(plan);
                return statuscurr;

            }
        }


        public virtual void UpdateAllocatedQuantitiesWithCustomPlan(INItemPlan plan)
        {
            InPlanConstantsSOTransfers.UpdateAllocatedQuantitiesWithCustomPlan(Base, plan);
        }
        public delegate void UpdateAllocatedQuantitiesWithExistingPlansDel(INItemSiteSummary itemsite);

        [PXOverride]
        public virtual void UpdateAllocatedQuantitiesWithExistingPlans(INItemSiteSummary itemsite, UpdateAllocatedQuantitiesWithExistingPlansDel baseM)
        {
            foreach (PXResult<INItemPlan, InventoryItem> res in PXSelectJoin<INItemPlan,
                InnerJoin<InventoryItem, On<INItemPlan.FK.InventoryItem>>,
                Where<INItemPlan.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>,
                    And<INItemPlan.siteID, Equal<Current<INItemSiteSummary.siteID>>,
                    And<InventoryItem.stkItem, Equal<boolTrue>>>>>
                .SelectMultiBound(Base, new object[] { itemsite }))
            {
                INItemPlan plan = (INItemPlan)res;
                INPlanType plantype = INPlanType.PK.Find(Base, plan.PlanType);

                if (plan.InventoryID != null &&
                    plan.SubItemID != null &&
                    plan.SiteID != null)
                {
                    if (plan.LocationID != null)
                    {
                        LocationStatus item = UpdateAllocatedQuantities<LocationStatus>(plan, plantype, true);
                        UpdateAllocatedQuantities<SiteStatus>(plan, plantype, (bool)item.InclQtyAvail);
                        if (!string.IsNullOrEmpty(plan.LotSerialNbr))
                        {
                            UpdateAllocatedQuantities<LotSerialStatus>(plan, plantype, true);
                            UpdateAllocatedQuantities<SiteLotSerial>(plan, plantype, true);
                        }
                    }
                    else
                    {
                        UpdateAllocatedQuantities<SiteStatus>(plan, plantype, true);
                        if (!string.IsNullOrEmpty(plan.LotSerialNbr))
                        {
                            //TODO: check if LotSerialNbr was allocated on OrigPlanType
                            UpdateAllocatedQuantities<SiteLotSerial>(plan, plantype, true);
                        }
                    }
                }
            }

            //Updating cross-site ItemLotSerial
            foreach (INItemPlan plan in PXSelect<INItemPlan,
                    Where<INItemPlan.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>,
                        And<INItemPlan.lotSerialNbr, NotEqual<StringEmpty>,
                        And<INItemPlan.lotSerialNbr, IsNotNull>>>>
                    .SelectMultiBound(Base, new object[] { itemsite }))
            {
                INPlanType plantype = INPlanType.PK.Find(Base, plan.PlanType);

                if (plan.InventoryID != null &&
                    plan.SubItemID != null &&
                    plan.SiteID != null)
                {
                    if (plan.LocationID != null)
                    {
                        UpdateAllocatedQuantities<ItemLotSerial>(plan, plantype, true);
                    }
                    else
                    {
                        UpdateAllocatedQuantities<ItemLotSerial>(plan, plantype, true);
                    }
                }
            }
        }
    }
}