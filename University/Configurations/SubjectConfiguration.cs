using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University.Models;

namespace University.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            //builder.HasKey(a => a.Id);

            //builder
            //    .HasMany(t=>t.Teachers)
            //    .WithMany(t=>t.Subjects)
            //    .UsingEntity(j => j.ToTable("TeacherSubjects"));
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        }
    }
}
