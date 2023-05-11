using Newtonsoft.Json;

namespace project_managet_models.Models
{
    public class Device : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Specs { get; set; }

        [JsonIgnore] public virtual ICollection<Project> Projects { get; set; }
    }
}