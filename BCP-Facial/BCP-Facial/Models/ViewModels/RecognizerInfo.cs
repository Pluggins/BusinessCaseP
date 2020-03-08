using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class RecognizerInfoInput
    {
        public string RecognizerId { get; set; }
        public string RecognizerKey { get; set; }
        public string StudentId { get; set; }
    }
    public class RecognizerInfoOutput
    {
        public string RecognizerId { get; set; }
        public string RecognizerStatus { get; set; }
        public string LastActivity { get; set; }
        public string Result { get; set; }

        //Recognizer input
        public string Command { get; set; }
        public string PrimaryValue { get; set; }
        public string SecondaryValue { get; set; }
    }
}
