using University.Models;

namespace University
{
    public static class DataInitializer
    {
        public static void Initialize(ApplicationContext context)
        {
            if (context.Subjects.Any())
            {
                return;
            }
            var subj1 = new Subject { Name = "Математика" };
            var subj2 = new Subject { Name = "Информатика" };
            var subj3 = new Subject { Name = "Физика" };
            var subj4 = new Subject { Name = "Базы данных" };
            var subj5 = new Subject { Name = "Технологии" };

            var curs1 = new Course { Name = "3", Description = "Третьекурсники" };
            var curs2 = new Course { Name = "2", Description = "Второкурсники" };

            var teach1 = new Teacher
            {
                Name = "Светлана",
                Surname = "Андронатий",
                Patronymic = "Георгиевна",
                DateOfBirth = new DateTimeOffset(1982, 12, 07, 0, 0, 0, TimeSpan.Zero),
                Degrees = new List<string> { "Кандидат наук" },
                Subjects = new List<Subject> { subj1, subj2 }
            };

            var teach2 = new Teacher
            {
                Name = "Павел",
                Surname = "Воля",
                Patronymic = "Викторович",
                DateOfBirth = new DateTimeOffset(1979, 04, 29, 0, 0, 0, TimeSpan.Zero),
                Degrees = new List<string> { "Ученый" },
                Subjects = new List<Subject> { subj3, subj4, subj5 }
            };

            var group1 = new Group { Name = "21ПИ", Description = "Программисты фиговы", Course = curs1, Curator = teach1 };
            var group2 = new Group { Name = "22ПИ", Description = "Программисты фиговы 2", Course = curs2, Curator = teach2 };

            var stud1 = new Student
            {
                Name = "Георгий",
                Surname = "Победоносный",
                Patronymic = "Викторович",
                DateOfBirth = new DateTimeOffset(2004, 02, 12, 0, 0, 0, TimeSpan.Zero),
                Group = group1,
                Description = "Черт"
            };

            var stud2 = new Student
            {
                Name = "Елизавета",
                Surname = "Андронатий",
                Patronymic = "Петровна",
                DateOfBirth = new DateTimeOffset(2005, 07, 03, 0, 0, 0, TimeSpan.Zero),
                Group = group2,
                Description = "Отличница"
            };

            var stud3 = new Student
            {
                Name = "Виктор",
                Surname = "Ворошилов",
                Patronymic = "Степанович",
                DateOfBirth = new DateTimeOffset(2003, 09, 01, 0, 0, 0, TimeSpan.Zero),
                Group = group1,
                Description = "Хулиган"
            };

            var stud4 = new Student
            {
                Name = "Олег",
                Surname = "Юнлинг",
                Patronymic = "Никитич",
                DateOfBirth = new DateTimeOffset(2003, 11, 28, 0, 0, 0, TimeSpan.Zero),
                Group = group2,
                Description = "Хулиган"
            };

            context.Subjects.AddRange(subj1, subj2, subj3, subj4, subj5);
            context.Courses.AddRange(curs1, curs2);
            context.Teachers.AddRange(teach1, teach2);
            context.Groups.AddRange(group1, group2);
            context.Students.AddRange(stud1, stud2, stud3, stud4);

            context.SaveChanges();
        }
    }
}
