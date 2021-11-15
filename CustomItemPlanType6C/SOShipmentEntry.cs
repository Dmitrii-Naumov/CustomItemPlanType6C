using PX.Data;
using PX.Objects.IN;
using PX.Objects.IN.Overrides.INDocumentRelease;

namespace PX.Objects.SO
{
    public class SOShipmentEntryCustomPlanExtension : PXGraphExtension<SOShipmentEntry>
    {
        public PXSelect<INItemPlan> dummyitemplan;
        public override void Initialize()
        {
            Base.Caches<SiteStatus>().Interceptor = new SiteStatusAccumulatorSOTransfersAttribute();
            base.Initialize();
        }

        public virtual void InItemPlan_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            UpdateAllocatedQuantitiesWithCustomPlan( (INItemPlan)e.Row);
            UpdateAllocatedQuantitiesWithCustomPlan( (INItemPlan)e.OldRow, revert: true);
        }

        public virtual void InItemPlan_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            UpdateAllocatedQuantitiesWithCustomPlan((INItemPlan)e.Row);
        }

        public virtual void InItemPlan_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            UpdateAllocatedQuantitiesWithCustomPlan((INItemPlan)e.Row, revert: true);
        }

        public virtual void UpdateAllocatedQuantitiesWithCustomPlan( INItemPlan plan, bool revert = false)
        {
            //InPlanConstantsSOTransfers.UpdateAllocatedQuantitiesWithCustomPlan(Base, plan, revert);
        }
    }

    public class SOShipmentEntryExt6TPlan : PXGraphExtension<SOShipmentEntryCustomPlanExtension, SOShipmentEntry>
    {
        public delegate void UpdateAllocatedQuantitiesWithCustomPlanDel(INItemPlan plan, bool revert);
        [PXOverride]
        public virtual void UpdateAllocatedQuantitiesWithCustomPlan(INItemPlan plan, bool revert, 
            UpdateAllocatedQuantitiesWithCustomPlanDel BaseMehtod)
        {
            BaseMehtod(plan, revert);
            InPlanConstantsSOTransfers.UpdateAllocatedQuantitiesWithCustomPlan(Base, plan, revert);
        }
    }
}