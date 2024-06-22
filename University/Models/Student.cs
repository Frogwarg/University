namespace University.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
}
