namespace University.Models
{
    public class GroupsViewModel
    {
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<string> UniqueCourses { get; set; } = new List<string>();
        public string SelectedCourse { get; set; } = string.Empty;
        public string SearchTerm { get; set; } = string.Empty;
        public List<Group> FilteredGroups { get; set; } = new List<Group>();
    }
}
