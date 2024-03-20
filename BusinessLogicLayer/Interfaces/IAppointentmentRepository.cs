using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IAppointmentRepository
    {
        bool IsAppointmentsAvailable(int id);
    }
}
