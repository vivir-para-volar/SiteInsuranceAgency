using System.Collections.Generic;

namespace InsuranceAgency.Models.ViewModels
{
    public class AllInfoAboutPolicy
    {
        public Policy Policy { get; set; }
        public List<PersonAllowedToDrive> PersonsAllowedToDrive { get; set; }
        public List<InsuranceEvent> InsuranceEvents { get; set;}

        public AllInfoAboutPolicy(Policy policy, List<PersonAllowedToDrive> personsAllowedToDrive, List<InsuranceEvent> insuranceEvents)
        {
            Policy = policy;
            PersonsAllowedToDrive = personsAllowedToDrive;
            InsuranceEvents = insuranceEvents;
        }
    }
}