namespace University.Models
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Teacher> Teachers { get; set; }
    }
}
