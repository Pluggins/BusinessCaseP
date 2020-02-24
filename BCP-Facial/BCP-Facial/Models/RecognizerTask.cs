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
        public string Command { get; set; }
        public int Status { get; set; }
        public virtual Recognizer Recognizer { get; set; }

        public RecognizerTask()
        {
            Id = Guid.NewGuid().ToString();
            Status = 1;
        }
    }
}
