using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerEF.Models
{
    public abstract class BaseModel
    {
        public DateOnly LastUpdate { get; set;}
        public DateOnly CreationDate { get; set;} = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? DeletionDate { get; set; } 
        public bool IsDeleted { get; set;} 
    }
}
