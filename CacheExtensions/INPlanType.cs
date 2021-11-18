using System;

using PX.Data;


namespace PX.Objects.IN
{
	public sealed class INPlanTypeExtPlan6C : PXCacheExtension<INPlanType>
    {
        //usrInclQtySOTransfer
        public abstract class usrInclQtySOTransfer : PX.Data.BQL.BqlShort.Field<usrInclQtySOTransfer> { }
        [PXDBShort()]
        [PXDefault((short)0)]
        [PXUIField(DisplayName = "Custom Plan Type", Enabled = false)]
        public Int16? UsrInclQtySOTransfer
        {
            get; set;
        }
    }
}