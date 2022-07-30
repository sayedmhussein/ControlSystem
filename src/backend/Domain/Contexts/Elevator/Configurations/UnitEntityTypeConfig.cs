using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeeControl.SharedKernel.Elevator.Constants;

namespace WeeControl.Domain.Contexts.Elevator.Configurations;

public class UnitEntityTypeConfig : IEntityTypeConfiguration<UnitDbo>
{
    public void Configure(EntityTypeBuilder<UnitDbo> builder)
    {
        builder.ToTable(nameof(UnitDbo), schema: nameof(Elevator));
        builder.HasKey(x => x.UnitNumber);
        builder.Property(x => x.UnitNumber).HasMaxLength(10);
        

        builder.Property(x => x.UnitIdentification).HasMaxLength(10);

        builder.HasOne<BuildingDbo>(x => x.Building)
            .WithMany(x => x.Units)
            .HasForeignKey(x => x.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}