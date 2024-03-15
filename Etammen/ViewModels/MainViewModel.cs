using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels
{
    public class MainViewModel
    {
        public bool IsProfessor { get; set; }
        public bool IsLecturer { get; set; }
        public bool IsConsultant { get; set; }
        public bool IsSpecialist { get; set; }

        public Gender Gender { get; set; }
        public bool IsFeesLessThan100 { get; set; }
        public bool IsFees100to200 { get; set; }
        public bool IsFees200to300 { get; set; }
        public bool IsFeesMoreThan300 { get; set; }

        [Display(Name = "Opening Days")]
        public OpeningDays OpeningDays { get; set; }
        public int Order { get; set; } = 0;

        public List<Doctor> SearchedDoctors { get; set; }
        public List<Doctor> FilteredOrderedDoctors { get; set; }

        public string specialty {  get; set; } 
        public string city {  get; set; } 
        public string area { get; set; }

        public string doctorName { get; set; }
           
        public string clinicName { get; set; }
    }
}
