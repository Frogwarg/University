namespace University.Models
{
    public class Teacher
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;
        public List<string> Degrees { get; set; } = [];
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public List<Subject> Subjects { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
}
