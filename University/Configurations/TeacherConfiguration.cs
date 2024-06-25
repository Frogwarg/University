using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University.Models;

namespace University.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Surname).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Patronymic).HasMaxLength(100);
            builder.Property(t => t.DateOfBirth).IsRequired();
            builder.Property(t => t.Description).HasMaxLength(1000);
            builder.Property(t => t.ImagePath).HasMaxLength(250);

            builder.HasIndex(t => t.Name);
            builder.HasIndex(t => t.Surname);
            builder.HasIndex(t => t.Patronymic);
            builder.HasIndex(t => t.DateOfBirth);

            builder.HasMany(t => t.Subjects)
                .WithMany(s => s.Teachers)
                .UsingEntity<Dictionary<string, object>>(
                "TeacherSubject",
                j => j.HasOne<Subject>().WithMany().HasForeignKey("SubjectId"),
                j => j.HasOne<Teacher>().WithMany().HasForeignKey("TeacherId"));

            builder.HasOne(t => t.Group)
                .WithOne(g => g.Curator)
                .HasForeignKey<Group>(g => g.CuratorId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
