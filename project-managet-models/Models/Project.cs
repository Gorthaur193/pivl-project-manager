using Newtonsoft.Json;

namespace project_managet_models.Models
{
    public class Project : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public double Budget { get; set; }

        [JsonIgnore] public virtual ICollection<Employee> Employees { get; set; }
        [JsonIgnore] public virtual ICollection<Device> Devices { get; set; }    
    }
}