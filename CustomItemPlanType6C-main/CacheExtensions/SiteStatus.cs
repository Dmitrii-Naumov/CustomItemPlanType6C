using System;
using PX.Data;

namespace PX.Objects.IN.Overrides.INDocumentRelease
{
	public sealed class SiteStatusExtPlan6C : PXCacheExtension<SiteStatus>
    {
        public abstract class usrInclQtySOTransfers : PX.Data.BQL.BqlBool.Field<usrInclQtySOTransfers> { }
        [PXBool()]
        [PXDefault(typeof(Select<INAvailabilityScheme, Where<INAvailabilityScheme.availabilitySchemeID, Equal<Current<SiteStatus.availabilitySchemeID>>>>),
          CacheGlobal = true, SourceField = typeof(INAvailabilitySchemeExtPlan6C.usrInclQtySOTransfer), PersistingCheck = PXPersistingCheck.Nothing)]
        public Boolean? UsrInclQtySOTransfers
        {
            get; set;
        }

        #region QtySOTransfer
        public abstract class usrQtySOTransfer : PX.Data.BQL.BqlDecimal.Field<usrQtySOTransfer> { }
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. SO Transfers")]
        public Decimal? UsrQtySOTransfer
        {
            get; set;
        }
        #endregion

    }

}