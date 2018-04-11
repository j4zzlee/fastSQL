using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastSQL.Sync.Core.Models
{
    [Table("beehexa_core_option_groups")]
    public class OptionGroupModel
    {
        [Key]
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
