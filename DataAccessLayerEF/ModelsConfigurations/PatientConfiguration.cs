using DataAccessLayerEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayerEF.ModelsConfigurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(P => P.ApplicationUserId);
            builder.HasOne(P => P.ApplicationUser)
                .WithOne(A => A.Patient).HasForeignKey<ApplicationUser>(A => A.Id);
        }
    }
}
