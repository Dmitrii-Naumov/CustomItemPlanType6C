using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.IN;
using PX.Objects;
using System.Collections.Generic;
using System;

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