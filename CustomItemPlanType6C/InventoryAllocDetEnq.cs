using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PX.Api;
using PX.Common;
using PX.Objects.CR;
using PX.SM;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.BQLConstants;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.SO;
using PX.Objects.PO;
using PX.Objects.IN.Attributes;
using System.Web.Compilation;
using PX.Objects.Common.Attributes;
using IQtyAllocated = PX.Objects.IN.Overrides.INDocumentRelease.IQtyAllocated;
using PX.Objects;
using PX.Objects.IN;

namespace PX.Objects.IN
{
  public partial class InventoryAllocDetEnqFilterExtSOTransfer : PXCacheExtension<InventoryAllocDetEnqFilter>
  {
    #region QtySOTrnasfers
    public abstract class usrQtySOTransfer : PX.Data.BQL.BqlDecimal.Field<usrQtySOTransfer> { }
    protected Decimal? _QtySOTransfer;
    [InventoryAllocationField(IsAddition = false, InclQtyFieldName = nameof(usrInclQtySOTransfer), SortOrder = 160)]
    [PXDBQuantity()]
    [PXDefault(TypeCode.Decimal, "0.0")]
    [PXUIField(DisplayName = "SO Allocated [**]", Enabled = false)]
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
    #region InclQtySOTransfer
    public abstract class usrInclQtySOTransfer : PX.Data.BQL.BqlBool.Field<usrInclQtySOTransfer> { }
    protected bool? _InclQtySOTransfer;
    [PXDBBool()]
    [PXUIField(DisplayName = " ", Enabled = false)]
    public virtual bool? UsrInclQtySOTransfer
    {
      get
      {
        return this._InclQtySOTransfer;
      }
      set
      {
        this._InclQtySOTransfer = value;
      }
    }
    #endregion
  }
  public class InventoryAllocDetEnqExtSOTransfer : PXGraphExtension<InventoryAllocDetEnq>
  {

    private Dictionary<string, string> _displayNameByEntityTypeExt;
    public PXFilter<InventoryAllocDetEnqFilter> Filter;
    public InventoryAllocDetEnqExtSOTransfer()
    {
      _displayNameByEntityTypeExt = new Dictionary<string, string>();
    }

    /* could not make it possible to override, so wtire own instance 
    public delegate void AggregateDel(InventoryAllocDetEnqFilter aDest, InventoryAllocDetEnqResult aSrc);
    public virtual void Aggregate(InventoryAllocDetEnqFilter aDest, InventoryAllocDetEnqResult aSrc, AggregateDel baseMeth)
    */
    public virtual void AggregateCustomType(InventoryAllocDetEnqFilter aDest, InventoryAllocDetEnqResult aSrc)
    {

      if (aSrc.AllocationType == nameof(INAvailabilitySchemeExtSOTransfer.usrInclQtySOTransfer))
      {
        InventoryAllocDetEnqFilterExtSOTransfer aDestext = aDest.GetExtension<InventoryAllocDetEnqFilterExtSOTransfer>();
        aDestext.UsrQtySOTransfer += (aSrc.LocNotAvailable == false) ? aSrc.PlanQty : 0m;

      }
    
    }

    public virtual IEnumerable filter()
    {
      PXCache cache = Base.Caches[typeof(InventoryAllocDetEnqFilter)];
      //calls the base view del to initialize the curret cache
      foreach (InventoryAllocDetEnqFilter filter1 in Base.Filter.Select())
      {
        //filter 1 already should point to current cache
        InventoryAllocDetEnqFilterExtSOTransfer filterext = filter1.GetExtension<InventoryAllocDetEnqFilterExtSOTransfer>();
        filterext.UsrQtySOTransfer = 0;
        if (filter1 != null && filter1.InventoryID != null)
        {
           
          InventoryItem inventoryItemRec = InventoryItem.PK.Find(Base, filter1.InventoryID);
          INAvailabilityScheme availSchemeRec = PXSelectReadonly2<INAvailabilityScheme,
            InnerJoin<INItemClass, On<INItemClass.FK.AvailabilityScheme>>,
            Where<INItemClass.itemClassID, Equal<Required<INItemClass.itemClassID>>>>
            .Select(Base, inventoryItemRec.ItemClassID);
          //cannot override Aggregate funnction to make it called, so repeating logic here
          foreach (InventoryAllocDetEnqResult it in
            Base.ResultRecords.Select().RowCast<InventoryAllocDetEnqResult>().
            Where(f => f.AllocationType == nameof(INAvailabilitySchemeExtSOTransfer.usrInclQtySOTransfer))) //???
          {
            AggregateCustomType(filter1, it);
          }

          
          INAvailabilitySchemeExtSOTransfer availSchemeRecext = availSchemeRec.GetExtension<INAvailabilitySchemeExtSOTransfer>();
          filterext.UsrInclQtySOTransfer = availSchemeRecext.UsrInclQtySOTransfer;
          filter1.QtyAvail -= ((filterext.UsrInclQtySOTransfer ?? false) ? filterext.UsrQtySOTransfer : 0m);
        }
        yield return (InventoryAllocDetEnqFilter)cache.Current;
      }
    }

    //ProcessItemPlanRecAs
    // will not extend as it is called from this graph only
    /*
    public delegate void ProcessItemPlanRecAsDel(Type planTypeInclQtyField, List<InventoryAllocDetEnqResult> resultList, InventoryAllocDetEnq.ItemPlanWithExtraInfo itemPlanWithExtraInfo);
    public virtual void ProcessItemPlanRecAs(Type planTypeInclQtyField, List<InventoryAllocDetEnqResult> resultList,
      InventoryAllocDetEnq.ItemPlanWithExtraInfo itemPlanWithExtraInfo, ProcessItemPlanRecAsDel baseM)
    */

    public virtual void ProcessItemPlanRecAs(Type planTypeInclQtyField, List<InventoryAllocDetEnqResult> resultList,
      InventoryAllocDetEnq.ItemPlanWithExtraInfo itemPlanWithExtraInfo)
    {
      Base.ProcessItemPlanRecAs(planTypeInclQtyField, resultList, itemPlanWithExtraInfo);
      if (planTypeInclQtyField == typeof(INPlanType.inclQtySOShipping)
        && itemPlanWithExtraInfo.ItemPlan.OrigPlanType == InPlanConstantsSOTransfers.Plan6C)
      {
        foreach (var soOrderLink in itemPlanWithExtraInfo.ShipmentItemPlanToSOOrderItemPlanLinks)
        {
          if (itemPlanWithExtraInfo.ItemPlan.OrigPlanType == InPlanConstantsSOTransfers.Plan6C)
          {
            Base.AdjustPrevResult(typeof(INPlanTypeExtSOTransfers.usrInclQtySOTransfer), resultList, itemPlanWithExtraInfo.ItemPlan,
              soOrderLink.OrderNoteID);
          }

        }
      }
    }

    //CalculateResultRecords
    public delegate IEnumerable<InventoryAllocDetEnqResult> CalculateResultRecordsDel();
    [PXOverride]
    public virtual IEnumerable<InventoryAllocDetEnqResult> CalculateResultRecords(CalculateResultRecordsDel baseM)
    {
      //will not call baseM();
      _displayNameByEntityTypeExt.Clear();

      InventoryAllocDetEnqFilter filter = Base.Filter.Current;

      // InventoryID is required
      if (filter.InventoryID == null)
        return Enumerable.Empty<InventoryAllocDetEnqResult>();

      PXSelectBase<InventoryAllocDetEnq.INItemPlan> cmd = Base.GetResultRecordsSelect(filter);
      PXResultset<InventoryAllocDetEnq.INItemPlan> itemPlansWithExtraInfo = cmd.Select();

      var resultList = new List<InventoryAllocDetEnqResult>();
      foreach (InventoryAllocDetEnq.ItemPlanWithExtraInfo ip in Base.UnwrapAndGroup(itemPlansWithExtraInfo))
      {
        Type inclQtyField = InPlanConstantsSOTransfers.ToInclQtyFieldExtSOTransfers(ip.ItemPlan.PlanType); // append for SO Transfers csase
        if (inclQtyField != null && inclQtyField != typeof(INPlanType.inclQtyINReplaned))
          ProcessItemPlanRecAs(inclQtyField, resultList, ip); // procedure from the extension graph
      }

      // numerate grid lines (key column) to let ViewDocument button work
      int nextLineNbr = 1;
      DateTime minPlanDate = new DateTime(1900, 1, 1);
      foreach (InventoryAllocDetEnqResult it in resultList)
      {
        if (it.PlanDate == minPlanDate)
          it.PlanDate = null;
        it.GridLineNbr = nextLineNbr++;
      }

      PXStringListAttribute.SetList<InventoryAllocDetEnqResult.qADocType>(Base.Caches<InventoryAllocDetEnqResult>(), null,
        _displayNameByEntityTypeExt.Keys.ToArray(), _displayNameByEntityTypeExt.Values.ToArray());

      return resultList;
    }


  }
}