using Microsoft.EntityFrameworkCore;
using University.Configurations;

namespace University.Models
{
    public class ApplicationContext: DbContext
    {
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new CourseConfiguration());
            //modelBuilder.ApplyConfiguration(new GroupConfiguration());
            //modelBuilder.ApplyConfiguration(new StudentConfiguration());
            //modelBuilder.ApplyConfiguration(new SubjectConfiguration());
            //modelBuilder.ApplyConfiguration(new TeacherConfiguration());

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    Subject subj1 = new Subject { SubjectId = 1, Name = "Математика" };
        //    Subject subj2 = new Subject { SubjectId = 2, Name = "Информатика" };
        //    Subject subj3 = new Subject { SubjectId = 3, Name = "Физика" };
        //    Subject subj4 = new Subject { SubjectId = 4, Name = "Базы данных" };

        //    Course curs1 = new Course { CourseId = 101, Name = "3", Description = "Леди гага" };
        //    Course curs2 = new Course { CourseId = 102, Name = "4", Description = "Выпускнички" };

        //    Teacher teach1 = new Teacher
        //    {
        //        CuratorId = 1001,
        //        Name = "Светлана",
        //        Surname = "Андронатий",
        //        Patronymic = "Георгиевна",
        //        DateOfBirth = new DateTime(),
        //        Degrees = new List<string> { "кандидат наук" },
        //        Subjects = new List<Subject> { subj1, subj2 }
        //    };
        //    Teacher teach2 = new Teacher
        //    {
        //        CuratorId = 1002,
        //        Name = "Павел",
        //        Surname = "Воля",
        //        Patronymic = "Викторович",
        //        DateOfBirth = new DateTime(),
        //        Degrees = new List<string> { "ученый" },
        //        Subjects = new List<Subject> { subj3, subj4 }
        //    };

        //    Group group1 = new Group { GroupId = 201, Name = "21ПИ", Description = "Программисты фиговы", CourseId = curs1.CourseId, CuratorId = teach1.CuratorId };
        //    Group group2 = new Group { GroupId = 202, Name = "20ПИ", Description = "Программисты фиговы 2", CourseId = curs2.CourseId, CuratorId = teach2.CuratorId };

        //    Student stud1 = new Student
        //    {
        //        StudentId = 10001,
        //        Name = "Георгий",
        //        Surname = "Победоносный",
        //        Patronymic = "Викторович",
        //        DateOfBirth = new DateTime(),
        //        Description = "черт",
        //        GroupId = group1.GroupId
        //    };
        //    Student stud2 = new Student
        //    {
        //        StudentId = 10002,
        //        Name = "Елизавета",
        //        Surname = "Андронатий",
        //        Patronymic = "Петровна",
        //        DateOfBirth = new DateTime(),
        //        Description = "отличница",
        //        GroupId = group2.GroupId
        //    };
        //    Student stud3 = new Student
        //    {
        //        StudentId = 10003,
        //        Name = "Виктор",
        //        Surname = "Ворошилов",
        //        Patronymic = "Степанович",
        //        DateOfBirth = new DateTime(),
        //        Description = "хулиган",
        //        GroupId = group1.GroupId
        //    };
        //    Student stud4 = new Student
        //    {
        //        StudentId = 10004,
        //        Name = "Олег",
        //        Surname = "Юнлинг",
        //        Patronymic = "Никитич",
        //        DateOfBirth = new DateTime(),
        //        Description = "хулиган",
        //        GroupId = group2.GroupId
        //    };

        //    modelBuilder.Entity<Group>().HasData(group1, group2);

        //    modelBuilder.Entity<Course>().HasData(curs1, curs2);

        //    modelBuilder.Entity<Teacher>().HasData(teach1, teach2);

        //    modelBuilder.Entity<Subject>().HasData(subj1, subj2, subj3, subj4);

        //    modelBuilder.Entity<Student>().HasData(stud1, stud2, stud3, stud4);

        //}
    }
}
