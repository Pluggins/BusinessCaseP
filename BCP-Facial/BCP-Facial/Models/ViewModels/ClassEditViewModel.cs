using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class ClassEditViewModel
    {
        public string ClassCode { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public BCPUser SelectedLecturer { get; set; }
        public List<BCPUser> Lecturers { get; set; }
        public List<ClassEditAllocation> ClassAllocations { get; set; }
    }

    public class ClassEditAllocation
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime DateJoined { get; set; }
        public string AttendanceCount { get; set; }
    }
}
