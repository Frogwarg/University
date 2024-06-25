namespace University.Models
{
    public class GroupsViewModel
    {
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<Course>? UniqueCourses { get; set; }
        public Course? SelectedCourse { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public List<Group> FilteredGroups { get; set; } = new List<Group>();
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}
