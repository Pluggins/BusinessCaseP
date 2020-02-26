using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("BCPUsers")]
    public class BCPUser : _CommonAttribute
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        /*
         * Status
         * 1 - User
         * 2 - Lecturer
         * 3 - Admin
         * 4 - Lecturer & Admin
         */
        public int Status { get; set; }
        public string PersonId { get; set; }
        [Required]
        public virtual IdentityUser AspUser { get; set; }
        public virtual ICollection<ClassAllocation> List_ClassAllocation { get; set; }
        public virtual ICollection<UserImage> List_UserImage { get; set; }

        public BCPUser()
        {
            Id = Guid.NewGuid().ToString();
            Status = 1;
        }
    }
}
