using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerEF.Models
{
    public class DoctorReviews:BaseModel
    {
        public int DoctorId { get; set; }

        public int PatientId { get; set; }

        public virtual Doctor? Doctor { get; set; }
        public virtual Patient? Patient { get; set; }

        public int? Rate { get; set; }

        public string? Comment { get; set; }

        

    }
}
