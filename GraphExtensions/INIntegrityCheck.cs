using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN.Overrides.INDocumentRelease;

namespace PX.Objects.IN
{
    public class INIntegrityCheckExtPlan6C : PXGraphExtension<INIntegrityCheckCustomPlanExtension,INIntegrityCheck>
    {
        public delegate void UpdateAllocatedQuantitiesWithCustomPlanDel(INItemPlan plan);

        [PXOverride]
        public virtual void UpdateAllocatedQuantitiesWithCustomPlan(INItemPlan plan, 
            UpdateAllocatedQuantitiesWithCustomPlanDel baseMeth)
        {
            baseMeth(plan);
            InPlanConstantsSOTransfers.UpdateAllocatedQuantitiesWithCustomPlan(Base, plan);
        }
    }
}