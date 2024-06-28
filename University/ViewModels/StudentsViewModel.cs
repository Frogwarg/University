using University.Models;

namespace University.ViewModels
{
    public class StudentsViewModel
    {
        public List<Student> Students { get; set; } = new List<Student>();
        public string SearchFIO { get; set; } = string.Empty;
        public string SearchGroup { get; set; } = string.Empty;
        public List<Student> FilteredStudents { get; set; } = new List<Student>();
        public List<Group> Groups { get; set; } = new List<Group>();
        public Student? SelectedStudent { get; set; }
    }
}
