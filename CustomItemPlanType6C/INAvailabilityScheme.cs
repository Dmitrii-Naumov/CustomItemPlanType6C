using System;
using System.Collections.Generic;

using PX.Data;


namespace PX.Objects.IN
{
 public class INAvailabilitySchemeExtSOTransfer: PXCacheExtension<INAvailabilityScheme>
  {
    public abstract class usrInclQtySOTransfer: PX.Data.BQL.BqlBool.Field<usrInclQtySOTransfer> { }
    [PXDBBool]
    [PXDefault(true)]
    [PXUIField(DisplayName = "Deduct Qty. on Transfer Sales Orders")]
    public virtual bool? UsrInclQtySOTransfer { get; set; }
  }
}