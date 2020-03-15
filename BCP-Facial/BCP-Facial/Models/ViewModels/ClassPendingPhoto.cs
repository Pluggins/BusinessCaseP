using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class ClassPendingPhotoInput
    {
        public string ClassId { get; set; }
    }
    public class ClassPendingPhotoOutput
    {
        public List<ClassPendingPhotoItem> Photos { get; set; }
        public string Result { get; set; }
    }

    public class ClassPendingPhotoItem
    {
        public string Url { get; set; }
    }
}
