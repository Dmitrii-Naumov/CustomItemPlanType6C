using PX.Data;
using PX.Objects.IN;

namespace PX.Objects.SO
{
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