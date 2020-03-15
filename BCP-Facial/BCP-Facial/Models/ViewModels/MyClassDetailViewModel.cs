using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class MyClassDetailViewModel
    {
        public string ClassCode { get; set; }
        public string ClassCapacity { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public string LecturerName { get; set; }
        public List<MyClassDetailItem> Students { get; set; }
    }

    public class MyClassDetailItem
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string AttendanceCount { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
