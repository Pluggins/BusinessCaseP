using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class SetClassViewModel
    {
        public List<SetClassItem> Classes { get; set; }
        public StudentViewModel Student { get; set; }
    }

    public class SetClassItem
    {
        public string ClassName { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
