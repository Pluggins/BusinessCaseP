using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class PersonGroupInput
    {
        public string Name { get; set; }
        public string UserData { get; set; }
        public string RecognitionModel { get; set; }

        public PersonGroupInput()
        {
            RecognitionModel = "recognition_02";
        }
    }
}
