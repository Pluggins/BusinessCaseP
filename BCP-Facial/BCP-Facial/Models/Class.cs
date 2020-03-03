using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("Classes")]
    public class Class : _CommonAttribute
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public int Capacity { get; set; }
        public virtual BCPUser Lecturer { get; set; }
        public virtual ICollection<ClassAllocation> List_ClassAllocation { get; set; }
        
        public Class()
        {
            Id = Guid.NewGuid().ToString();
            Capacity = 0;
        }
    }
}
