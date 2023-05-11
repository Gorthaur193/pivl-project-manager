using Newtonsoft.Json;

namespace project_managet_models.Models
{
    public class JobTitle : IEntity
    {
        [JsonIgnore] public Guid Id { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}