using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University.Models;

namespace University.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Surname).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Patronymic).HasMaxLength(100);
            builder.Property(s => s.DateOfBirth).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(1000);
            builder.Property(s => s.ImagePath).HasMaxLength(250);

            builder.HasIndex(s => s.Name);
            builder.HasIndex(s => s.Surname);
            builder.HasIndex(s => s.DateOfBirth);

            builder.HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
