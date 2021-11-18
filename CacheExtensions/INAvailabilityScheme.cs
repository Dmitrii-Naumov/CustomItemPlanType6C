using System;
using System.Collections.Generic;

using PX.Data;


namespace PX.Objects.IN
{
    public sealed class INAvailabilitySchemeExtPlan6C : PXCacheExtension<INAvailabilityScheme>
    {
        public abstract class usrInclQtySOTransfer : PX.Data.BQL.BqlBool.Field<usrInclQtySOTransfer> { }
        [PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Deduct Qty. on Transfer Sales Orders")]
        public bool? UsrInclQtySOTransfer { get; set; }
    }
}