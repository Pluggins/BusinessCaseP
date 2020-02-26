using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("UserImages")]
    public class UserImage : _CommonAttribute
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
        public string FaceId { get; set; }
        public string CreatedBy { get; set; }
        public float Confidence { get; set; }
        public virtual BCPUser User { get; set; }

        public UserImage()
        {
            Id = Guid.NewGuid().ToString();
            Status = 1;
        }
    }
}
