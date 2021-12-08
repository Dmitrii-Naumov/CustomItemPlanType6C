using System;
using PX.Data;
using PX.Objects.IN.Attributes;


namespace PX.Objects.IN
{
	public sealed class InventoryAllocDetEnqFilterExtPlan6C : PXCacheExtension<InventoryAllocDetEnqFilter>
    {
        #region QtySOTrnasfers
        public abstract class usrQtySOTransfer : PX.Data.BQL.BqlDecimal.Field<usrQtySOTransfer> { }
        protected Decimal? _QtySOTransfer;
        [InventoryAllocationField(IsAddition = false, InclQtyFieldName = nameof(usrInclQtySOTransfer), SortOrder = 160)]
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "SO Allocated [**]", Enabled = false)]
        public Decimal? UsrQtySOTransfer
        {
            get; set;
        }
        #endregion
        #region InclQtySOTransfer
        public abstract class usrInclQtySOTransfer : PX.Data.BQL.BqlBool.Field<usrInclQtySOTransfer> { }
        [PXDBBool()]
        [PXUIField(DisplayName = " ", Enabled = false)]
        public bool? UsrInclQtySOTransfer
        {
            get; set;
        }
        #endregion
    }
}