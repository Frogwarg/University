namespace University.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public ICollection<Group> Groups { get; set; }
    }
}
