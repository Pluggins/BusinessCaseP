using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("ClassAllocations")]
    public class ClassAllocation : _CommonAttribute
    {
        [Key]
        public string Id { get; set; }
        public string PersonId { get; set; }
        public virtual Class Class { get; set; }
        public virtual BCPUser Student { get; set; }

        public ClassAllocation()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
