using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Repositories
{
    public class DoctorReviewsRepository : IDoctorReviewsRepository
    {
        private readonly EtammenDbContext _context;
        public DoctorReviewsRepository(EtammenDbContext context) 
        {
           _context = context;
        }
        public bool IsReviewdBy(int doctorId, int patientId)
        {
            var existingReview = _context.DoctorReviews
                .Any(r => r.DoctorId == doctorId && r.PatientId == patientId);

            return existingReview;
        }

    }
}
