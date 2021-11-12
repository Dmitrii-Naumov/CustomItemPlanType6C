using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using PX.Common;
using PX.Objects.Common.Extensions;
using PX.Objects.Common.GraphExtensions.Abstract;
using PX.Objects.Common.GraphExtensions.Abstract.DAC;
using PX.Objects.Common.GraphExtensions.Abstract.Mapping;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.IN.Overrides.INDocumentRelease;
using PX.Objects.IN.PhysicalInventory;
using PX.Objects.IN.Services;
using PX.Objects.SO;
using PX.Objects.AP;
using PX.Objects.GL.FinPeriods;
using PX.Objects.PM;
using PX.Objects.PO.LandedCosts;
using PX.Objects;
using PX.Objects.IN;

namespace PX.Objects.IN.Overrides.INDocumentRelease
{
  
  //public partial class SiteStatusExtSOTransfers : InSiteStatusExtSOTransfers
  public partial class SiteStatusExtSOTransfers : PXCacheExtension<SiteStatus>
  {
    public abstract class usrInclQtySOTransfers : PX.Data.BQL.BqlBool.Field<usrInclQtySOTransfers> { }
    protected Boolean? _usrInclQtySOTransfer;
    [PXBool()]
    [PXDefault(typeof(Select<INAvailabilityScheme, Where<INAvailabilityScheme.availabilitySchemeID, Equal<Current<SiteStatus.availabilitySchemeID>>>>),
      CacheGlobal = true, SourceField = typeof(INAvailabilitySchemeExtSOTransfer.usrInclQtySOTransfer), PersistingCheck = PXPersistingCheck.Nothing)]
    
    //[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
    public virtual Boolean? UsrInclQtySOTransfers
    {
      get
      {
        return this._usrInclQtySOTransfer;
      }
      set
      {
        this._usrInclQtySOTransfer = value;
      }
    }

    #region QtySOTransfer
    public abstract class usrQtySOTransfer : PX.Data.BQL.BqlDecimal.Field<usrQtySOTransfer> { }
    protected Decimal? _QtySOTransfer;
    [PXDBQuantity()]
    [PXDefault(TypeCode.Decimal, "0.0")]
    [PXUIField(DisplayName = "Qty. SO Transfers")]
    public virtual Decimal? UsrQtySOTransfer
    {
      get
      {
        return this._QtySOTransfer;
      }
      set
      {
        this._QtySOTransfer = value;
      }
    }
    #endregion
    
  }

  public class SiteStatusAccumulatorSOTransfersAttribute : SiteStatusAccumulatorAttribute
  {
    //public SiteStatusAccumulatorSOTransfersAttribute{}

     protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
      {
        SiteStatus bal = (SiteStatus)row;
        bool? origPersistEvenZero = bal.PersistEvenZero;
        bal.PersistEvenZero = true;
        if (!base.PrepareInsert(sender, row, columns))
        {
            bal.PersistEvenZero = origPersistEvenZero;
            return false;
          }
          bal.PersistEvenZero = origPersistEvenZero;
    
          //INSiteStatus bali = (INSiteStatus)bal;
          SiteStatusExtSOTransfers balext = bal.GetExtension<SiteStatusExtSOTransfers>();
          columns.Update<SiteStatusExtSOTransfers.usrQtySOTransfer>(balext.UsrQtySOTransfer, PXDataFieldAssign.AssignBehavior.Summarize);
    
          if (sender.GetStatus(row) == PXEntryStatus.Inserted && IsZero(bal) && bal.PersistEvenZero != true)
          {
            if (sender.Locate(row) is SiteStatus located && ReferenceEquals(located, row))
            {
              // only for Persist operation
              sender.SetStatus(row, PXEntryStatus.InsertedDeleted);
              return false;
            }
          }

      return true;
    }

    public override bool IsZero(IStatus a)
    {
      if (!(a is SiteStatus))
      {
        return base.IsZero(a);
      }
      else
      {
        SiteStatusExtSOTransfers aext = ((SiteStatus)a).GetExtension<SiteStatusExtSOTransfers>();
        return base.IsZero(a) && (aext.UsrQtySOTransfer == 0);
      }
    }
  }

  public class INReleaseProcessExt6TPlan : PXGraphExtension<PX.Objects.IN.INReleaseProcess>
  {

    public override void Initialize()
    {

      Base.Caches<SiteStatus>().Interceptor = new SiteStatusAccumulatorSOTransfersAttribute();
      base.Initialize();
    }
  }

  
}