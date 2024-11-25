using System.Text.Json.Serialization;

namespace MauiApp3
{
    public class Root
    {
        [JsonPropertyName("university")]
        public University University { get; set; }
    }

    public class University
    {
        [JsonPropertyName("departments")]
        public List<Department> Departments { get; set; }
    }

    public class Department
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("disciplines")]
        public List<Discipline> Disciplines { get; set; }
    }

    public class Discipline
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("students")]
        public List<Student> Students { get; set; }
    }

    public class Student
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("grade")]
        public int Grade { get; set; }
    }

    public class StudentInfo
    {
        public string Name { get; set; }
        public string Discipline { get; set; }
        public string Department { get; set; }
        public int Grade { get; set; }
    }
}
