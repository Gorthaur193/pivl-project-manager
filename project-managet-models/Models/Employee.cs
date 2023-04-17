using System.ComponentModel.DataAnnotations;

namespace project_managet_models.Models
{
    public class Employee : IEntity
    {
        public Guid Id { get; set; }
        [Required] [StringLength(50)] public string Name { get; set; }
        [Required] [StringLength(50)] public string PersonalId { get; set; }
        public double Salary { get; set; }

        [Required] public virtual JobTitle JobTitle { get; set; }
        public virtual Employee Supervisor { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}