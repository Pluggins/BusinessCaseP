using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class MyAttendanceViewModel
    {
        public string StudentName { get; set; }
        public List<MyAttendanceViewItem> Classes { get; set; }
    }

    public class MyAttendanceViewItem
    {
        public string ClassName { get; set; }
        public string Attendance { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
