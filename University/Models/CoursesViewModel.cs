﻿namespace University.Models
{
    public class CoursesViewModel
    {
        public List<Course> Courses { get; set; }
        public string SearchCourse { get; set; } = string.Empty;
        public List<Course> FilteredCourses { get; set; }
    }
}
