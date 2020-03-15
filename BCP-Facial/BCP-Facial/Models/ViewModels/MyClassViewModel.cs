using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class MyClassViewModel
    {
        public List<MyClassItem> Classes { get; set; }
    }

    public class MyClassItem
    {
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public string Capacity { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
