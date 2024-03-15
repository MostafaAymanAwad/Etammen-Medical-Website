using DataAccessLayerEF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Helpers
{
    public class DoctorEquailtyComparer : IEqualityComparer<Doctor>
    {
        public bool Equals(Doctor? x, Doctor? y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] Doctor obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
