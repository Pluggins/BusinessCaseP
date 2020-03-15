using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class MyClassAttendanceViewModel
    {
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public List<Recognizer> Recognizers { get; set; }
    }
}
