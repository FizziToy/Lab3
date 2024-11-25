using System.Collections.Generic;
using System.Linq;

namespace MauiApp3
{
    public static class LINQ
    {
        public static List<StudentInfo> FlattenData(University university)
        {
            var result = university.Departments.SelectMany(dept =>
                dept.Disciplines.SelectMany(disc =>
                    disc.Students.Select(student => new StudentInfo
                    {
                        Name = student.Name,
                        Discipline = disc.Name,
                        Department = dept.Name,
                        Grade = student.Grade
                    }))).ToList();

            Console.WriteLine("Flattened Data:");
            foreach (var student in result)
            {
                Console.WriteLine($"Name: {student.Name}, Discipline: {student.Discipline}, Department: {student.Department}, Grade: {student.Grade}");
            }

            return result;
        }


        public static List<StudentInfo> SearchStudents(List<StudentInfo> students, string searchText)
        {
            Console.WriteLine($"Search Text: {searchText}");
            foreach (var student in students)
            {
                Console.WriteLine($"Before Filter: {student.Name}, Discipline: {student.Discipline}, Department: {student.Department}, Grade: {student.Grade}");
            }

            searchText = searchText.ToLower();
            var filtered = students.Where(s =>
                s.Name.ToLower().Contains(searchText) ||
                s.Discipline.ToLower().Contains(searchText) ||
                s.Department.ToLower().Contains(searchText) ||
                s.Grade.ToString().Contains(searchText)).ToList();

            foreach (var student in filtered)
            {
                Console.WriteLine($"After Filter: {student.Name}, Discipline: {student.Discipline}, Department: {student.Department}, Grade: {student.Grade}");
            }

            return filtered;
        }
        public static List<Department> GroupData(List<StudentInfo> students)
        {
            return students
                .GroupBy(s => s.Department)
                .Select(g => new Department
                {
                    Name = g.Key,
                    Disciplines = g.GroupBy(d => d.Discipline)
                                   .Select(dg => new Discipline
                                   {
                                       Name = dg.Key,
                                       Students = dg.Select(s => new Student
                                       {
                                           Name = s.Name,
                                           Grade = s.Grade
                                       }).ToList()
                                   }).ToList()
                }).ToList();
        }

    }
}
