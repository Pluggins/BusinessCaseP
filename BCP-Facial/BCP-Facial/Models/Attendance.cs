using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("Attendances")]
    public class Attendance : _CommonAttribute
    {
        [Key]
        public string Id { get; set; }
        public virtual AttendanceItem AttendanceItem { get; set; }
        public virtual Class Class { get; set; }

        public Attendance()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
