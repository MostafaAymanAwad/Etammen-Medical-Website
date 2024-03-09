using DataAccessLayerEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayerEF.ModelsConfigurations
{
    public class DoctorConfigurtion : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(D => D.ApplicationUserId);
            builder.HasOne(D => D.ApplicationUser)
                .WithOne(A => A.Doctor).HasForeignKey<ApplicationUser>(A=>A.Id);
        }
    }
}
