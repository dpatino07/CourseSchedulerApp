using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WGUCourseApp.Classes
{
    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string AssessName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string AssessType { get; set; }
        public int Course { get; set; }
        public int Notify { get; set; }

    }
}
