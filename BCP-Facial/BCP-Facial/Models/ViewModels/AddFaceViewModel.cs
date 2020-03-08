using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class AddFaceViewModel
    {
        public StudentViewModel Student { get; set; }
        public List<Recognizer> Recognizers { get; set; }
    }
}
