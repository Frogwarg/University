using University.Models;

namespace University.ViewModels
{
    public class SubjectsViewModel
    {
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public string SearchSubject { get; set; } = string.Empty;
        public List<Subject> FilteredSubjects { get; set; } = new List<Subject>();
    }
}
