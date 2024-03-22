using BusinessLogicLayer.Helpers;
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
		Task<IEnumerable<Doctor>> Search(string specialty, string city, string area, string doctorName, string clinicName);
        List<Doctor> FilterByOptions(DoctorFilterOptions doctorFilterOptions, List<Doctor> doctors);
        List<Doctor> OrderByOption(int orderByOption, List<Doctor> doctors);
        int GetDoctorIdByUserId(string applicationUserID);
    }
}
