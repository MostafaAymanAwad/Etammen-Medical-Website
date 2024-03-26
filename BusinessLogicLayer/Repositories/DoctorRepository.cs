using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
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
			IQueryable<Doctor> query = _context.Doctors.Include(D=>D.Clinics).Where(D => D.IsDeleted == false && D.Clinics.Count != 0);
			if(specialty!="ALL")
				query=query.Where(D=>D.Speciality==specialty);
			List<Doctor>queryDoctors= await query.ToListAsync();
			if(doctorName != "")
				queryDoctors= filterDoctorName(queryDoctors, doctorName);
			if(clinicName!="")
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
			List<Doctor> removed =new List<Doctor>();
			for (int i = 0; i < queryDoctors.Count; i++)
			{
				bool contain = false;
				foreach (var clinic in queryDoctors[i].Clinics ?? [])
				{
					string name = clinic.Name;
					name=name.ToLower().Trim().Replace(" ","");
					if(name.Contains(clinicName))
					{
						contain = true;
						break;
					}
				}
				if (!contain)
					removed.Add(queryDoctors[i]);
			}
            foreach (var doctor in removed)
				 queryDoctors.Remove(doctor);
			return queryDoctors;
		}
		private List<Doctor> filterDoctorName(List<Doctor> queryDoctors, string doctorName)
        {
			doctorName = doctorName.ToLower().Trim().Replace(" ","");
            List<Doctor> removed = new List<Doctor>();
            for (int i = 0; i < queryDoctors.Count; i++)
			{
				var doctorAppName = _context.Doctors
					.Include(d => d.ApplicationUser)
					.Where(d => d.Id == queryDoctors[i].Id)
					.Select(d => d.ApplicationUser.FirstName + d.ApplicationUser.LastName)
					.FirstOrDefault();
                if (doctorAppName.ToLower().Contains(doctorName)==false)
                    removed.Add(queryDoctors[i]);
            }
            foreach (var doctor in removed)
                queryDoctors.Remove(doctor);
            return queryDoctors;
        }
        private List<Doctor> filterCity(List<Doctor> queryDoctors,string city)
		{
            List<Doctor> removed = new List<Doctor>();
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
                    removed.Add(queryDoctors[i]);
            }
            foreach (var doctor in removed)
                queryDoctors.Remove(doctor);
            return queryDoctors;
		}
		private List<Doctor> filterArea(List<Doctor> queryDoctors, string area)
		{
            List<Doctor> removed = new List<Doctor>();
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
                    removed.Add(queryDoctors[i]);
            }
            foreach (var doctor in removed)
                queryDoctors.Remove(doctor);
            return queryDoctors;
		}
		
		public List<Doctor> FilterByOptions(DoctorFilterOptions doctorFilterOptions, List<Doctor> doctors)
        {
			List<Doctor> temp = new List<Doctor>();
			temp.AddRange(doctors);
			bool[] isFilterCritrea = CheckCategories(doctorFilterOptions);
			List<Doctor> removedDoctors= new List<Doctor>();
			for(int i=0;i<doctors.Count; i++)
			{
				if (isFilterCritrea[0])
				{
					if (!IsDoctorMatchDegree(doctors[i],doctorFilterOptions))
					{
                        removedDoctors.Add(doctors[i]);
						continue;
                    }	
                }
				if (isFilterCritrea[1])
				{
					if(!IsDoctorMatchGender(doctors[i],doctorFilterOptions))
                    {
                        removedDoctors.Add(doctors[i]);
                        continue;
                    }
                }
				if (isFilterCritrea[2])
				{
                    if (!IsDoctorMatchFees(doctors[i], doctorFilterOptions))
                    {
                        removedDoctors.Add(doctors[i]);
                        continue;
                    }
                }
                if (doctorFilterOptions.OpeningDays != (OpeningDays)127)
                {
                    if (doctors[i].Clinics.Any(c => (c.OpeningDays & doctorFilterOptions.OpeningDays) > 0)) continue;
                    removedDoctors.Add(doctors[i]);
                }
            }
			for (int i = 0; i < removedDoctors.Count; i++)
				temp.Remove(removedDoctors[i]);
			return temp;
        }
        private bool[] CheckCategories(DoctorFilterOptions doctorFilterOptions)
        {
            bool[] arr = new bool[3];
            arr[0] = doctorFilterOptions.IsLecturer || doctorFilterOptions.IsProfessor
                || doctorFilterOptions.IsSpecialist || doctorFilterOptions.IsGP || doctorFilterOptions.IsConsultant;
            arr[1] = doctorFilterOptions.Gender != null;
            arr[2] = doctorFilterOptions.IsFees100to200 || doctorFilterOptions.IsFeesLessThan100
                     || doctorFilterOptions.IsFees200to300 || doctorFilterOptions.IsFeesMoreThan300;
            return arr;
        }
        private bool IsDoctorMatchDegree(Doctor doctor, DoctorFilterOptions doctorFilterOptions)
        {
            if (doctor.Degree == "Lecturer" && doctorFilterOptions.IsLecturer == false) return false;
			if (doctor.Degree == "Professor" && doctorFilterOptions.IsProfessor == false) return false;
            if (doctor.Degree == "Specialist" && doctorFilterOptions.IsSpecialist == false) return false;
            if (doctor.Degree == "GP" && doctorFilterOptions.IsGP == false) return false;
			return true;
        }
        private bool IsDoctorMatchGender(Doctor doctor, DoctorFilterOptions doctorFilterOptions)
        {
            var doctorWithApplicationUser = _context.Doctors
                                                    .Include(d => d.ApplicationUser)
                                                    .FirstOrDefault(d => d.Id == doctor.Id);
            return doctorWithApplicationUser.ApplicationUser.Gender == doctorFilterOptions.Gender;
        }
        private bool IsDoctorMatchFees(Doctor doctor, DoctorFilterOptions doctorFilterOptions)
        {
            if (doctorFilterOptions.IsFeesLessThan100 && doctor.Clinics.Any(c => c.Fees < 100)) return true;
            if (doctorFilterOptions.IsFees100to200 && doctor.Clinics.Any(c => c.Fees >= 100 && c.Fees < 200)) return true;
            if (doctorFilterOptions.IsFees200to300 && doctor.Clinics.Any(c => c.Fees >= 200 && c.Fees < 300)) return true;
            if (doctorFilterOptions.IsFeesMoreThan300 && doctor.Clinics.Any(c => c.Fees >= 300)) return true;
			return false;
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
                    var query=doctors.OrderByDescending(doctor => doctor.Clinics.Max(clinic => clinic.Fees)).ToList();
					Debug.WriteLine(query[0].Clinics);
					return query;
				default:
					return doctors;
            }
        }
        public int GetDoctorIdByUserId(string applicationUserID)
        {
            return _context.Doctors.Where(d => d.ApplicationUserId == applicationUserID)
                                     .Select(d => d.Id).FirstOrDefault();
        }
    }
}
