using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    public class _CommonAttribute
    {
        public bool Deleted { get; set; }
        public DateTime DateCreated { get; set; }

        public _CommonAttribute()
        {
            Deleted = false;
            DateCreated = DateTime.UtcNow.AddHours(8);
        }
    }
}
