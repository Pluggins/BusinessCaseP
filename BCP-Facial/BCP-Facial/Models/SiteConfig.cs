using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Models
{
    [Table("SiteConfigs")]
    public class SiteConfig
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
