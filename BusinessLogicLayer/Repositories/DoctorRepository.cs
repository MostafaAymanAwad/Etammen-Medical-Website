using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Repositories
{
	public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
	{
		private readonly EtammenDbContext _context;
		public DoctorRepository(EtammenDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Doctor>> Search(string specialty, string city, string area, string doctorName, string clinicName)
		{
			IQueryable<Doctor> query = _context.Doctors.Where(D => D.IsDeleted == false).Include(D=>D.Clinics).Include(D=>D.ApplicationUser);
			if(specialty!="ALL")
				query=query.Where(D=>D.Speciality==specialty);
			List<Doctor>queryDoctors= await query.ToListAsync();
			if(doctorName != null)
				queryDoctors= filterDoctorName(queryDoctors, doctorName);
			if(clinicName!=null)
				queryDoctors = filterClinicName(queryDoctors, clinicName);
			if (city!="ALL")
				queryDoctors=filterCity(queryDoctors, city);
			if (area !="ALL")
				queryDoctors=filterArea(queryDoctors, area);
			return queryDoctors;
		}

		private List<Doctor> filterClinicName(List<Doctor> queryDoctors, string clinicName)
		{
			clinicName = clinicName.ToLower().Trim().Replace(" ", "");
			for (int i = 0; i < queryDoctors.Count; i++)
			{
				bool contain = false;
				foreach (var clinic in queryDoctors[i].Clinics ?? [])
				{
					string name = clinic.Name;
					name.ToLower().Trim().Replace(" ","");
					if(name.Contains(clinicName))
					{
						contain = true;
						break;
					}
				}
				if (!contain)
					queryDoctors.Remove(queryDoctors[i]);
			}
			return queryDoctors;
		}

		private List<Doctor> filterDoctorName(List<Doctor> queryDoctors, string doctorName)
        {
			doctorName = doctorName.ToLower().Trim().Replace(" ","");
            for (int i = 0; i < queryDoctors.Count; i++)
			{
				string firstName = queryDoctors[i].ApplicationUser.FirstName.ToLower();
				string lastName = queryDoctors[i].ApplicationUser.LastName.ToLower();
				string fullName=firstName+lastName;
                if (fullName.Contains(doctorName)==false)
                    queryDoctors.Remove(queryDoctors[i]);
			}
			return queryDoctors;
        }
        private List<Doctor> filterCity(List<Doctor> queryDoctors,string city)
		{
			for (int i = 0; i < queryDoctors.Count; i++)
			{
				bool cityIncluded = false;
				foreach (var clinic in queryDoctors[i].Clinics ?? [])
				{
					if (clinic.Address.governorate?.ToLower() == city.ToLower())
					{
						cityIncluded = true;
						break;
					}
				}
				if (!cityIncluded)
					queryDoctors.Remove(queryDoctors[i]);
			}
			return queryDoctors;
		}
		private List<Doctor> filterArea(List<Doctor> queryDoctors, string area)
		{
			for (int i = 0; i < queryDoctors.Count; i++)
			{
				bool areaIncluded = false;
				foreach (var clinic in queryDoctors[i].Clinics ?? [])
				{
					if (clinic.Address.City.ToLower() == area.ToLower())
					{
						areaIncluded = true;
						break;
					}
				}
				if (!areaIncluded)
					queryDoctors.Remove(queryDoctors[i]);
			}
			return queryDoctors;
		}

        public Task<List<Doctor>> FilterByOptions(DoctorFilterOptions doctorFilterOptions, List<Doctor> doctors)
        {
			IEnumerable<Doctor>query= new List<Doctor>();
			if (doctorFilterOptions.IsLecturer)
				query=query.Union(doctors.Where(d => d.Degree == "Lecturer").ToList(), new DoctorEquailtyComparer());
            if (doctorFilterOptions.IsSpecialist)
                query = query.Union(doctors.Where(d => d.Degree == "Lecturer").ToList(), new DoctorEquailtyComparer());
            if (doctorFilterOptions.IsLecturer)
                query = query.Union(doctors.Where(d => d.Degree == "Lecturer").ToList(), new DoctorEquailtyComparer());
            if (doctorFilterOptions.IsLecturer)
                query = query.Union(doctors.Where(d => d.Degree == "Lecturer").ToList(), new DoctorEquailtyComparer());
            if (doctorFilterOptions.IsLecturer)
                query = query.Union(doctors.Where(d => d.Degree == "Lecturer").ToList(), new DoctorEquailtyComparer());
            if (doctorFilterOptions.IsLecturer)
                query = query.Union(doctors.Where(d => d.Degree == "Lecturer").ToList(), new DoctorEquailtyComparer());
            if (doctorFilterOptions.IsLecturer)
                query = query.Union(doctors.Where(d => d.Degree == "Lecturer").ToList(), new DoctorEquailtyComparer());
            if (doctorFilterOptions.IsLecturer)
                query = query.Union(doctors.Where(d => d.Degree == "Lecturer").ToList(), new DoctorEquailtyComparer());

        }

        public List<Doctor> OrderByOption(int orderByOption, List<Doctor> doctors)
        {
            switch(orderByOption)
			{
				case 1:
					return doctors.OrderByDescending(d => d.ActualRting).ToList();
                case 2:
                    return doctors.OrderBy(doctor => doctor.Clinics.Min(clinic => clinic.Fees)).ToList();
                case 3:
                    return doctors.OrderByDescending(doctor => doctor.Clinics.Max(clinic => clinic.Fees)).ToList();
				default:
					return doctors;
            }
        }
    }
}
