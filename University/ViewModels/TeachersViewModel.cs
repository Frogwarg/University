using System.Diagnostics;
using University.Models;

namespace University.ViewModels
{
    public class TeachersViewModel
    {
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();
        public string SearchFIO { get; set; } = string.Empty;
        public List<string> UniqueDegrees { get; set; } = new List<string>();
        public string SelectedDegree { get; set; } = string.Empty;
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public Subject SelectedSubject { get; set; } = new Subject();
        public List<Teacher> FilteredTeachers { get; set; } = new List<Teacher>();
        public Teacher? SelectedTeacher { get; set; }
    }
}
