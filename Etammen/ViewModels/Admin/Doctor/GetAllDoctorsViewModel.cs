using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels.Admin.Doctor
{
    public class GetAllDoctorsViewModel
    {
    
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }

        [Required, MaxLength(30)]
        public string FirstName { get; set; }

        [Required, MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string Speciality { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
    }
}
