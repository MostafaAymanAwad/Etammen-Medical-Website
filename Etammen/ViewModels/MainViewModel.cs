using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels
{
    public class MainViewModel
    {
        public bool IsProfessor { get; set; }
        public bool IsLecturer { get; set; }
        public bool IsConsultant { get; set; }
        public bool IsSpecialist { get; set; }
        public bool IsGP { get; set; }
        public Gender? Gender { get; set; } = null;
        public bool IsFeesLessThan100 { get; set; }
        public bool IsFees100to200 { get; set; }
        public bool IsFees200to300 { get; set; }
        public bool IsFeesMoreThan300 { get; set; }

        [Display(Name = "Opening Days")]
        public OpeningDays? OpeningDays { get; set; } = null;
        public int Order { get; set; } = 0;

        public List<Doctor> SearchedDoctors { get; set; }
        public List<Doctor> FilteredOrderedDoctors { get; set; }

        public string specialty {  get; set; } 
        public string city {  get; set; } 
        public string area { get; set; }

        public string doctorName { get; set; }
           
        public string clinicName { get; set; }

        public List<string> Specialties { get; set; }
        public Dictionary<string, List<string>> city_areaDict { get; set; }
    }
}
