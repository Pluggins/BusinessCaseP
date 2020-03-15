using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("GroupImages")]
    public class GroupImage : _CommonAttribute
    {
        [Key]
        public string Id { get; set; }
        public string Url { get; set; }
        /*
         * Status
         * 0 - Cancelled
         * 1 - Pending for processing
         * 2 - Using
         */
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public virtual Class Class { get; set; }

        public GroupImage()
        {
            Id = Guid.NewGuid().ToString();
            Status = 1;
        }
    }
}
