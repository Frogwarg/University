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

            var curs1 = new Course { Name = "3", Description = "Леди гага" };
            var curs2 = new Course { Name = "4", Description = "Выпускнички" };

            var teach1 = new Teacher
            {
                Name = "Светлана",
                Surname = "Андронатий",
                Patronymic = "Георгиевна",
                DateOfBirth = new DateTime(),
                Degrees = new List<string> { "кандидат наук" },
                Subjects = new List<Subject> { subj1, subj2 }
            };

            var teach2 = new Teacher
            {
                Name = "Павел",
                Surname = "Воля",
                Patronymic = "Викторович",
                DateOfBirth = new DateTime(),
                Degrees = new List<string> { "ученый" },
                Subjects = new List<Subject> { subj3, subj4 }
            };

            var group1 = new Group { Name = "21ПИ", Description = "Программисты фиговы", Course = curs1, Curator = teach1 };
            var group2 = new Group { Name = "20ПИ", Description = "Программисты фиговы 2", Course = curs2, Curator = teach2 };

            var stud1 = new Student
            {
                Name = "Георгий",
                Surname = "Победоносный",
                Patronymic = "Викторович",
                DateOfBirth = new DateTime(),
                Group = group1,
                Description = "черт"
            };

            var stud2 = new Student
            {
                Name = "Елизавета",
                Surname = "Андронатий",
                Patronymic = "Петровна",
                DateOfBirth = new DateTime(),
                Group = group2,
                Description = "отличница"
            };

            var stud3 = new Student
            {
                Name = "Виктор",
                Surname = "Ворошилов",
                Patronymic = "Степанович",
                DateOfBirth = new DateTime(),
                Group = group1,
                Description = "хулиган"
            };

            var stud4 = new Student
            {
                Name = "Олег",
                Surname = "Юнлинг",
                Patronymic = "Никитич",
                DateOfBirth = new DateTime(),
                Group = group2,
                Description = "хулиган"
            };

            context.Subjects.AddRange(subj1, subj2, subj3, subj4);
            context.Courses.AddRange(curs1, curs2);
            context.Teachers.AddRange(teach1, teach2);
            context.Groups.AddRange(group1, group2);
            context.Students.AddRange(stud1, stud2, stud3, stud4);

            context.SaveChanges();
        }
    }
}
