using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Common;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.SM;
using PX.Objects.IN.Overrides.INDocumentRelease;
using POLineType = PX.Objects.PO.POLineType;
using POReceiptLine = PX.Objects.PO.POReceiptLine;
//using PX.CarrierService;
using PX.Data.DependencyInjection;
//using PX.LicensePolicy;
using PX.Objects.SO.Services;
using PX.Objects.PO;
using PX.Objects.AR.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Common.Collection;
using PX.Objects.SO.GraphExtensions.CarrierRates;
using PX.Api;
using ShipmentActions = PX.Objects.SO.SOShipmentEntryActionsAttribute;
using PX.Objects;
using PX.Objects.SO;

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