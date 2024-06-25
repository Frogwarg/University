namespace University.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid? CuratorId { get; set; }
        public Teacher Curator { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
