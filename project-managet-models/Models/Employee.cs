﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace project_managet_models.Models
{
    public class Employee : IEntity
    {
        public Guid Id { get; set; }
        [Required] [StringLength(50)] public string Name { get; set; }
        [Required] [StringLength(50)] public string Login { get; set; }
        [JsonIgnore] [Required] [StringLength(50)] public string Passhash { get; set; }
        public Role Role { get; set; }
        [Required] [StringLength(50)] public string PersonalId { get; set; }
        public double Salary { get; set; }

        [Required] public virtual JobTitle JobTitle { get; set; }
        [JsonIgnore] public virtual Employee? Supervisor { get; set; }
        [JsonIgnore] public virtual ICollection<Employee> Supervisees { get; set; }
        [JsonIgnore] public virtual ICollection<Project> Projects { get; set; }
    }
}