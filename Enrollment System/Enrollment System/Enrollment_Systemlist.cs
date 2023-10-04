using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrollment_System
{
    class Teacher //listview
    {
        public string TeacherName { get; set; }
        public string CourseTitle { get; set; }
        public string Credits { get; set; }
        public string Compulsory { get; set; }
        public string Openingclass { get; set; }
        public string Classtime { get; set; }
        public override string ToString()
        {
            return $"{CourseTitle} {CourseTitle} {Credits}學分 開課班級:{Openingclass}";
        }
    }
    class Student
    {
        public string StudentNum { get; set; }
        public string StudentName { get; set; }
        public override string ToString()
        {
            return $"{StudentName}";
        }
    }
    class TeachersName //treeview
    {
        public string TeacherName { get; set; }
        public ObservableCollection<Teacher> Course { get; set; }
        public TeachersName(string name, Teacher tc)
        {
            Course = new ObservableCollection<Teacher>();
            Course.Add(tc);
            TeacherName = name;
        }
    }
    class Choose
    {
        public string Name{get; set;}
        public string StudentID { get; set; }
        public string Instructor { get; set; }
        public string SubjectName { get; set; }
        public string Compulsory { get; set; }
        public string Credits { get; set; }
        public string OpeningClass { get; set; }
        public override string ToString()
        {
            return $"{Name} {StudentID} ||{SubjectName} {Instructor} {Compulsory} {Credits} {OpeningClass}";
        }
    }
}
