using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN.Overrides.INDocumentRelease;

namespace PX.Objects.IN
{
    public class INRegisterEntryBaseExtPlan6C : PXGraphExtension<INRegisterEntryBase>
    {
        public override void Initialize()
        {
            Base.Caches<SiteStatus>().Interceptor = new SiteStatusAccumulatorSOTransfersAttribute();
            base.Initialize();
        }
    }
}