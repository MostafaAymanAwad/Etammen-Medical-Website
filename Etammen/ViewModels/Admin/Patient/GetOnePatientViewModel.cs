using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;

namespace Etammen.ViewModels.Admin.Patient
{
    public class GetOnePatientViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }
        public Gender Gender { get; set; }

        public string? ApplicationUserId { get; set; }

        public DataAccessLayerEF.Models.Address? Address { get; set; }

    }
}
