using BCP_Facial.Data;
using BCP_Facial.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("Recognizers")]
    public class Recognizer : _CommonAttribute
    {
        [Key]
        public string Id { get; set; }
        public string Key { get; set; }
        public DateTime LastActivityDateTime { get; set; }
        public string LastActivityAction { get; set; }
        public virtual ICollection<RecognizerTask> List_RecognizerTask { get; set; }

        public Recognizer()
        {
            Id = Guid.NewGuid().ToString();
            Key = HashingService.GenerateSHA256(Convert.FromBase64String(Guid.NewGuid().ToString().Replace("-", "")), Convert.FromBase64String(ApplicationDbContext._hashSalt));
            LastActivityDateTime = DateTime.UtcNow.AddHours(8);
            LastActivityAction = "IDLE";
        }
    }
}
