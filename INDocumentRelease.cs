using PX.Data;

namespace PX.Objects.IN.Overrides.INDocumentRelease
{
	public class INReleaseProcessExtPlan6C : PXGraphExtension<PX.Objects.IN.INReleaseProcess>
    {
        public override void Initialize()
        {
            Base.Caches<SiteStatus>().Interceptor = new SiteStatusAccumulatorSOTransfersAttribute();
            base.Initialize();
        }
    }
}