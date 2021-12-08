using PX.Data;

namespace PX.Objects.IN.Overrides.INDocumentRelease
{
	public class SiteStatusAccumulatorSOTransfersAttribute : SiteStatusAccumulatorAttribute
    {
        //public SiteStatusAccumulatorSOTransfersAttribute{}

        protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
        {
            SiteStatus bal = (SiteStatus)row;
            bool? origPersistEvenZero = bal.PersistEvenZero;
            bal.PersistEvenZero = true;
            if (!base.PrepareInsert(sender, row, columns))
            {
                bal.PersistEvenZero = origPersistEvenZero;
                return false;
            }
            bal.PersistEvenZero = origPersistEvenZero;

            //INSiteStatus bali = (INSiteStatus)bal;
            SiteStatusExtPlan6C balext = bal.GetExtension<SiteStatusExtPlan6C>();
            columns.Update<SiteStatusExtPlan6C.usrQtySOTransfer>(balext.UsrQtySOTransfer, PXDataFieldAssign.AssignBehavior.Summarize);

            if (sender.GetStatus(row) == PXEntryStatus.Inserted && IsZero(bal) && bal.PersistEvenZero != true)
            {
                if (sender.Locate(row) is SiteStatus located && ReferenceEquals(located, row))
                {
                    // only for Persist operation
                    sender.SetStatus(row, PXEntryStatus.InsertedDeleted);
                    return false;
                }
            }

            return true;
        }

        public override bool IsZero(IStatus a)
        {
            if (!(a is SiteStatus))
            {
                return base.IsZero(a);
            }
            else
            {
                SiteStatusExtPlan6C aext = ((SiteStatus)a).GetExtension<SiteStatusExtPlan6C>();
                return base.IsZero(a) && (aext.UsrQtySOTransfer == 0);
            }
        }
    }

}