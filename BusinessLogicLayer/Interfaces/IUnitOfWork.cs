using DataAccessLayerEF.Models;

namespace BusinessLogicLayer.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Clinic> Clinics  { get; }
	public IDoctorRepository Doctors { get; }
	IPatientRepository Patients { get; }
    IGenericRepository<ClinicAppointment> ClinicAppointments { get; }
    IGenericRepository<HomeAppointment> HomeAppointment { get; }
    IGenericRepository<DoctorReviews> DoctorReviews { get; }
    Task<int> Commit();


}
