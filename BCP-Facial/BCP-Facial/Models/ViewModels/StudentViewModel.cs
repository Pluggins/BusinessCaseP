using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models.ViewModels
{
    public class StudentViewModel
    {
        public string Category { get; set; }
        public string StudentValue { get; set; }
        public List<BCPUser> Students { get; set; }
        public string StudentName { get; set; }
        public int AccountRole { get; set; }
        public List<UserImage> StudentImages { get; set; }
    }
}
