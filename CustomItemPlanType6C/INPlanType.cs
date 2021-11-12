using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.IN;
using PX.Objects;
using System.Collections.Generic;
using System;
using PX.Objects.IN.Overrides.INDocumentRelease;

namespace PX.Objects.IN
{
   public partial class INPlanTypeExtSOTransfers : PXCacheExtension<INPlanType>
  {
    //usrInclQtySOTransfer
    public abstract class usrInclQtySOTransfer : PX.Data.BQL.BqlShort.Field<usrInclQtySOTransfer> { }
    protected Int16? _UsrInclQtySOTransfer;
    [PXDBShort()]
    [PXDefault((short)0)]
    [PXUIField(DisplayName = "Custom Plan Type", Enabled = false)]
    public virtual Int16? UsrInclQtySOTransfer
    {
      get
      {
        return this._UsrInclQtySOTransfer;
      }
      set
      {
        this._UsrInclQtySOTransfer = value;
      }
    }
  }
        
    public class InPlanConstantsSOTransfers : INPlanConstants
  {
    public const string Plan6C = "6C";
    public class plan6C : PX.Data.BQL.BqlString.Constant<plan6C>
    {
      public plan6C() : base(Plan6C) {; }
    }

    public static Type ToInclQtyFieldExtSOTransfers(string planType)
    {
      if (planType == InPlanConstantsSOTransfers.Plan6C)
      {
        return (typeof(INPlanTypeExtSOTransfers.usrInclQtySOTransfer));
      }
      else
        return INPlanConstants.ToInclQtyField(planType);
    }

    public static string ToModuleFieldExtSOTransfers(string planType)
    {
      if (planType == InPlanConstantsSOTransfers.Plan6C)
      {
        return GL.BatchModule.SO; 
      }
      else
        return INPlanConstants.ToModuleField(planType);
    }

    public static void UpdateAllocatedQuantitiesWithCustomPlan(PXGraph graph, INItemPlan plan, bool revert = false)
    {
      string SoTransferPlanType = InPlanConstantsSOTransfers.Plan6C;
      if (plan == null || !(plan.PlanType == SoTransferPlanType || plan.OrigPlanType == SoTransferPlanType))
        return;

      INPlanType plantype = INPlanType.PK.Find(graph, (plan.PlanType == SoTransferPlanType) ? plan.PlanType : plan.OrigPlanType);
      //plantype = revert ? -plantype : plantype;
      InventoryItem stkitem = InventoryItem.PK.Find(graph, plan.InventoryID);
      bool updateOrigPlan = plan.OrigPlanType == SoTransferPlanType;
      bool revertplan = updateOrigPlan ^ revert;

      if (plan.InventoryID != null &&
          plan.SubItemID != null &&
          plan.SiteID != null &&
          stkitem != null && stkitem.StkItem == true)
      //only updating the sitestatus layer aince it is so plan w/o allocations
      {
        //BASE SOLUTION
        //UpdateAllocatedQuantities<SiteStatus>(sender.Graph, plan, plantype, true);
        //calls INPlanType targettype = GetTargetPlanType<TNode>(graph, plan, plantype);
        //In SOOrderEntry screen, we assume OrigPlanType cannot be filled 

        //SieStatus
        SiteStatus palncurr = (SiteStatus)graph.Caches[typeof(SiteStatus)].Current;
        SiteStatusExtSOTransfers targetext =
          (SiteStatusExtSOTransfers)graph.Caches[typeof(SiteStatus)].GetExtension<SiteStatusExtSOTransfers>(palncurr);

        INPlanTypeExtSOTransfers plantypeext = plantype.GetExtension<INPlanTypeExtSOTransfers>();
        targetext.UsrQtySOTransfer += (revertplan ? -plantypeext.UsrInclQtySOTransfer : plantypeext.UsrInclQtySOTransfer ?? 0) * plan.PlanQty;

        palncurr.QtyAvail -= targetext.UsrInclQtySOTransfers == true ?
        (revertplan ? -plantypeext.UsrInclQtySOTransfer : plantypeext.UsrInclQtySOTransfer ?? 0) * plan.PlanQty
        : 0m;
      }

    }
  }
}