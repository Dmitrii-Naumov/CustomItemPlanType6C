using System;

using PX.Data;
using PX.Objects.IN;
using PX.Objects.IN.Overrides.INDocumentRelease;

namespace PX.Objects.SO
{
    
    public class SOOrderEntryExtPlan6C : PXGraphExtension<SOOrderEntryCustomPlanExtension, SOOrderEntry>
    {

        public const string CustomPlanType = InPlanConstantsSOTransfers.Plan6C;
        public const bool CustomPlanTypeCanBackOrder = true;

        public delegate void UpdateAllocatedQuantitiesWithCustomPlanDel(INItemPlan plan, bool revert);
        [PXOverride]
        public virtual void UpdateAllocatedQuantitiesWithCustomPlan(INItemPlan plan, bool revert, UpdateAllocatedQuantitiesWithCustomPlanDel BaseMehtod)
        {
            BaseMehtod(plan, revert);
            InPlanConstantsSOTransfers.UpdateAllocatedQuantitiesWithCustomPlan(Base, plan, revert);
        }

        public delegate void UpdatefromCustomPlanInLInesDel(SOOrder row, bool DatesUpdated, bool CustomerUpdated);
        [PXOverride]
        public virtual void UpdatefromCustomPlanInLInes(SOOrder row, bool DatesUpdated, bool CustomerUpdated, UpdatefromCustomPlanInLInesDel BaseMehtod)
        {
            BaseMehtod(row, DatesUpdated, CustomerUpdated);
            Base1.UpdatefromCustomPlanInLinesImpl(row, CustomPlanType, DatesUpdated, CustomerUpdated, CustomPlanTypeCanBackOrder);  // blank second argument means the method won't run
        }

    }
}