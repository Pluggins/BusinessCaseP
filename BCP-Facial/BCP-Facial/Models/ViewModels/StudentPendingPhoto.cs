using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class StudentPendingPhotoInput
    {
        public string StudentId { get; set; }
    }
    public class StudentPendingPhotoOutput
    {
        public List<PendingPhotoItem> Photos { get; set; } 
        public string Result { get; set; }
    }
    public class PendingPhotoItem
    {
        public string UserImageId { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
