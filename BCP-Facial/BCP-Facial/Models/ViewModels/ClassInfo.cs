using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class ClassInfoInput
    {
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassCode { get; set; }
        public string LecturerId { get; set; }
        public string StudentId { get; set; }
        public int Capacity { get; set; }
    }
    public class ClassInfoOutput
    {
        public string ClassName { get; set; }
        public string Result { get; set; }
    }
}
