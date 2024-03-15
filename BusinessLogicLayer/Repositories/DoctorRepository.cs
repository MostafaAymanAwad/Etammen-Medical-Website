using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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

        public List<Doctor> FilterByOptions(DoctorFilterOptions doctorFilterOptions, List<Doctor> doctors)
        {
            var today = DateTime.Today;
			var tomorrow = DateTime.Today.AddDays(1);

            IEnumerable<Doctor>query= new List<Doctor>();
			if (doctorFilterOptions.IsGP)
				query=query.UnionBy(doctors.Where(d => d.Degree == "General Practitioner (GP)").ToList(), d=>d.ApplicationUserId);
            if (doctorFilterOptions.IsProfessor)
                query = query.UnionBy(doctors.Where(d => d.Degree == "Professor").ToList(), d=>d.ApplicationUserId);
            if (doctorFilterOptions.IsLecturer)
                query = query.UnionBy(doctors.Where(d => d.Degree == "Lecturer").ToList(), d=>d.ApplicationUserId);
            if (doctorFilterOptions.IsSpecialist)
                query = query.UnionBy(doctors.Where(d => d.Degree == "Specialist").ToList(), d=>d.ApplicationUserId);
            if (doctorFilterOptions.IsConsultant)
                query = query.UnionBy(doctors.Where(d => d.Degree == "Consultant").ToList(), d=>d.ApplicationUserId);

            if (doctorFilterOptions.IsFeesLessThan100)
                query = query.UnionBy(doctors.Where(d => d.Clinics.Any(c => c.Fees < 100)).ToList(), d=>d.ApplicationUserId);
            if (doctorFilterOptions.IsFees100to200)
                query = query.UnionBy(doctors.Where(d => d.Clinics.Any(c => c.Fees >= 100 && c.Fees < 200)).ToList(), d=>d.ApplicationUserId);
            if (doctorFilterOptions.IsFees200to300)
                query = query.UnionBy(doctors.Where(d => d.Clinics.Any(c => c.Fees >= 200 && c.Fees < 300)).ToList(), d=>d.ApplicationUserId);
            if (doctorFilterOptions.IsFeesMoreThan300)
                query = query.UnionBy(doctors.Where(d => d.Clinics.Any(c => c.Fees >= 300)).ToList(), d => d.ApplicationUserId);

			if(doctorFilterOptions.Gender != null)
			{
				if(doctorFilterOptions.Gender == Gender.Male)
				{
					query = query.UnionBy(doctors.Where(d => d.ApplicationUser.Gender == Gender.Male).ToList(), d => d.ApplicationUserId);
				}
                if (doctorFilterOptions.Gender == Gender.Female)
                {
                    query = query.UnionBy(doctors.Where(d => d.ApplicationUser.Gender == Gender.Female).ToList(), d => d.ApplicationUserId);
                }
            }

            if (doctorFilterOptions.OpeningDays != null)
            {
                var allEnumValues = Enum.GetValues(typeof(OpeningDays)).Cast<OpeningDays>().ToList();
                var filteredDays = allEnumValues.Where(day => (doctorFilterOptions.OpeningDays & day) == day).ToList();
                foreach (var day in filteredDays)
                {
                    query = query.UnionBy(doctors.Where(d => d.Clinics.Any(c => (c.OpeningDays & day) == day)), d => d.ApplicationUserId);
                }

            }
            return query.ToList();
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
