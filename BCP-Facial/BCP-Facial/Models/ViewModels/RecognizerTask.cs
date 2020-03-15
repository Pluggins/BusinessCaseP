using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class RecognizerTaskInput
    {
        public string StudentId { get; set; }
        public string ClassId { get; set; }
        public string RecognizerId { get; set; }
        public string RecognizerTaskId { get; set; }
        public string Command { get; set; }
    }
    public class RecognizerTaskOutput
    {
        public string RecognizerTaskId { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
    }
}
