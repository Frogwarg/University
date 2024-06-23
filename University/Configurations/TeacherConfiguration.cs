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

            builder.HasMany(t => t.Subjects)
                .WithMany(s => s.Teachers);

            builder.HasOne(t => t.Group)
                .WithOne(g => g.Curator)
                .HasForeignKey<Group>(g => g.CuratorId);
            //builder.HasKey(t => t.Id);

            //builder
            //    .HasMany(s => s.Subjects)
            //    .WithMany(s => s.Teachers)
            //    .UsingEntity(j => j.ToTable("TeacherSubjects"));

            ////builder
            ////    .HasOne(s => s.Group)
            ////    .WithOne(g => g.Curator)
            ////    .HasForeignKey<Teacher>(t => t.GroupId);

        }
    }
}
