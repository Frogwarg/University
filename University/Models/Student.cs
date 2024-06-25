namespace University.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public DateTimeOffset DateOfBirth { get; set; } = DateTimeOffset.MinValue;
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = "/images/default_empty.jpg";
        public Guid? GroupId { get; set; }
        public Group Group { get; set; }
    }
}
