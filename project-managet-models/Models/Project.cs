using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_managet_models.Models
{
    public class Project : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public double Budget { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Device> Devices { get; set; }    
    }
}