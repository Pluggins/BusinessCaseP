using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("AttendanceItems")]
    public class AttendanceItem : _CommonAttribute
    {
        [Key]
        public string Id { get; set; }
        public virtual ClassAllocation Student { get; set; }

        public AttendanceItem()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
