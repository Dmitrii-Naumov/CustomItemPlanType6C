using System;
using PX.Data;

namespace PX.Objects.IN
{
	public sealed class InventorySummaryEnquiryResultExtSOTransfers : PXCacheExtension<InventorySummaryEnquiryResult>
    {
        #region QtySOTransfer
        public abstract class usrQtySOTransfer : PX.Data.BQL.BqlDecimal.Field<usrQtySOTransfer> { }
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Custom Plan Type", Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
        public Decimal? UsrQtySOTransfer
        {
            get; set;
        }
        #endregion
    }

}