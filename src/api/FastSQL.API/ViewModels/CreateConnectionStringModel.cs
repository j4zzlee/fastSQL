using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.API.ViewModels
{
    public class CreateConnectionStringModel
    {
        [Required]
        public string ConnectionString { get; set; }
    }
}
