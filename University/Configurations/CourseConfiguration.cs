﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University.Models;

namespace University.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(1000);

            builder.HasIndex(s => s.Name);

            builder.HasMany(c => c.Groups)
                .WithOne(g => g.Course)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
