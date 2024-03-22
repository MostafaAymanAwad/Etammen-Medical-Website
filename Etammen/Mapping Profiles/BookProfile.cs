﻿using AutoMapper;
using DataAccessLayerEF.Models;
using Etammen.ViewModels;

namespace Etammen.Mapping_Profiles
{
    public class BookProfile:Profile
    {
        public BookProfile()
        {
          CreateMap<Doctor, BookViewModel > ()
                .ForMember(d=>d.DoctorFirstName, o=>o.MapFrom(s=>s.ApplicationUser.FirstName))
                .ForMember(d=>d.DoctorLastName, o=>o.MapFrom(s=>s.ApplicationUser.LastName))
                .ForMember(d=>d.DoctorId, o=>o.MapFrom(s=>s.Id))
                .ReverseMap();

            CreateMap<Clinic, BookViewModel>()
                .ForMember(d => d.ClinicFees, o => o.MapFrom(s => s.Fees))
                .ForMember(d => d.DoctorFirstName, o => o.MapFrom(s => s.Doctor.ApplicationUser.FirstName))
                .ForMember(d => d.DoctorLastName, o => o.MapFrom(s => s.Doctor.ApplicationUser.LastName))
                .ForMember(d => d.ISHomeVisit, o => o.MapFrom(s => s.Doctor.IsVisitHome))
                .ForMember(d => d.HomeVisitFees, o => o.MapFrom(s => s.Doctor.HomeVisitFees))
                .ReverseMap();

            CreateMap<ClinicAppointment, BookViewModel>()
                .ForMember(d=>d.ISClinicAppointmentDeleted, o=>o.MapFrom(s=>s.IsDeleted))
                .ReverseMap();

            CreateMap<HomeAppointment, BookViewModel>()
                .ForMember(d=>d.ISHomeAppointmentDeleted, o=>o.MapFrom(s=>s.IsDeleted))
                .ReverseMap();
        }   
    }
}