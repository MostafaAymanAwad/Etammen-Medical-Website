using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels.Admin.Patient
{
    public class GetAllPatientViewModel
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }

    }
}
