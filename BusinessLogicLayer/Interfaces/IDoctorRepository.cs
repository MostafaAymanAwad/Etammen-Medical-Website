using DataAccessLayerEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
	public interface IDoctorRepository:IGenericRepository<Doctor>
	{
		Task<IEnumerable<Doctor>> Search(string specialty, string city, string area,string doctorName, string clinicName);
	}
}
