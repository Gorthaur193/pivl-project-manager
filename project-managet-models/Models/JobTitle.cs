﻿namespace project_managet_models.Models
{
    public class JobTitle : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}