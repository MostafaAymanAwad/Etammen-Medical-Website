using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IApplicationUser
    {
        string FirstName(int id);
        string LastName(int id);
    }
}
