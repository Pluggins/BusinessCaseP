using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class StudentInfoInput
    {
        public string RecognizerId { get; set; }
        public string RecognizerKey { get; set; }
        public string StudentId { get; set; }
        public string ImageUrl { get; set; }
    }
    public class StudentInfoOutput
    {
        public string Result { get; set; }
    }
}
