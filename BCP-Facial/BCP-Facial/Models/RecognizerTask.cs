using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("RecognizerTasks")]
    public class RecognizerTask : _CommonAttribute
    {
        [Key]
        public string Id { get; set; }
        /*
         * Command
         * - REGISTER_NEW_FACE
         * - CAPTURE_CLASS_IMAGE
         */
        public string Command { get; set; }
        public string PrimaryValue { get; set; }
        public string SecondaryValue { get; set; }
        /*
         * Status
         * 0 - Cancelled
         * 1 - Unread
         * 2 - Read
         * 3 - Done
         */
        public int Status { get; set; }
        public virtual Recognizer Recognizer { get; set; }
        public DateTime DateModified { get; set; }

        public RecognizerTask()
        {
            Id = Guid.NewGuid().ToString();
            Status = 1;
            DateModified = DateTime.UtcNow.AddHours(8);
        }
    }
}
