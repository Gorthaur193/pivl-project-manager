using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_managet_models.Models
{
    public class Device : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Specs { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
