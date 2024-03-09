using DataAccessLayerEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerEF.ModelsConfigurations
{
    public class DoctorReviewsConfiguration : IEntityTypeConfiguration<DoctorReviews>
    {
        public void Configure(EntityTypeBuilder<DoctorReviews> builder)
        {
            builder.HasKey(E => new {E.PatientId,E.DoctorId});
        }
    }
}
