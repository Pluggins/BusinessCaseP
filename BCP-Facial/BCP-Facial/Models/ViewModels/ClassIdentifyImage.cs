using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class ClassIdentifyImage
    {
        public List<string> FaceIds { get; set; }
        public string MaxNumOfCandidatesReturned { get; set; }
        public string ConfidenceThreshold { get; set; }
        public string PersonGroupId { get; set; }
    }
}
