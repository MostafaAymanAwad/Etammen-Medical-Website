using DataAccessLayerEF.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Helpers
{
    public class DoctorFilterOptions
    {
        public bool IsProfessor { get; set; }
        public bool IsLecturer { get; set; }
        public bool IsConsultant { get; set; }
        public bool IsSpecialist { get; set; }
        public bool IsGP { get; set; }
        public Gender? Gender { get; set; }
        public bool IsFeesLessThan100 { get; set; }
        public bool IsFees100to200 { get; set; }
        public bool IsFees200to300 { get; set; }
        public bool IsFeesMoreThan300 { get; set; }
        [Display(Name = "Opening Days")]
        public OpeningDays? OpeningDays { get; set; }
    }
}
