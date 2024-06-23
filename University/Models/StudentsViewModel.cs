﻿namespace University.Models
{
    public class StudentsViewModel
    {
        public List<Student> Students { get; set; } = new List<Student>();
        public string SearchFIO { get; set; } = string.Empty;
        public string SearchGroup { get; set; } = string.Empty;
        public List<Student> FilteredStudents { get; set; } = new List<Student>();
        public Student? SelectedStudent { get; set; }
    }
}