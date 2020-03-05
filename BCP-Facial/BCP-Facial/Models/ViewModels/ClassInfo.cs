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
    }
    public class ClassInfoOutput
    {
        public string ClassName { get; set; }
        public string Result { get; set; }
    }
}
