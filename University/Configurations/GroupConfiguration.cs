using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University.Models;

namespace University.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name).IsRequired().HasMaxLength(100);
            builder.Property(g => g.Description).HasMaxLength(1000);

            builder.HasIndex(s => s.Name);

            builder.HasOne(g => g.Course)
                .WithMany(c => c.Groups)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.Curator)
                .WithOne(t => t.Group)
                .HasForeignKey<Group>(g => g.CuratorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(g => g.Students)
                .WithOne(s => s.Group)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}