﻿using System;
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
        public string DetectionModel { get; set; }
        public int maxNumOfCandidatesReturned { get; set; }
        public string Url { get; set; }
        public string FaceId { get; set; }
        public string FaceListId { get; set; }

        public PersonGroupInput()
        {
            RecognitionModel = "recognition_02";
        }
    }
}
