namespace InsuranceAgency.Models.ViewModels
{
    public class AllInfoAboutInsuranceEvent
    {
        public Policy Policy { get; set; }
        public InsuranceEvent InsuranceEvent { get; set; }

        public AllInfoAboutInsuranceEvent(Policy policy, InsuranceEvent insuranceEvent)
        {
            Policy = policy;
            InsuranceEvent = insuranceEvent;
        }
    }
}