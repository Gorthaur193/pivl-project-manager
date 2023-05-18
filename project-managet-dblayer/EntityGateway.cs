using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using project_managet_models;
using project_managet_models.Models;

namespace project_managet_dblayer
{
    public partial class EntityGateway : IDisposable
    {

        internal ProjectManagerContext Context { get; set; } = new();

        public void AddOrUpdate(params IEntity[] entities)
        {
            var toAdd = entities.Where(x => x.Id == Guid.Empty);
            var toUpdate = entities.Except(toAdd);
            Context.AddRange(toAdd);
            Context.UpdateRange(toUpdate);
            Context.SaveChanges();

        }

        public int EmployeesInProject(ActionType action, Guid projectId, params Guid[] employeeIds)
        {
            var project = Context.Projects.FirstOrDefault(x => x.Id == projectId) 
                                        ?? throw new Exception("Project is not found.");
            var employees = Context.Employees.Where(x => employeeIds.Contains(x.Id)).Except(project.Employees).ToArray();

            foreach (Employee employee in employees)
                if (action == ActionType.Add)
                    project.Employees.Add(employee);
                else
                    project.Employees.Remove(employee);
            AddOrUpdate(project);
            Context.SaveChanges();
            return employees.Length;
        }

        public int DevicesInProject(ActionType action, Guid projectId, params Guid[] devicesIds) // sh*tcode
        {
            var project = Context.Projects.FirstOrDefault(x => x.Id == projectId)
                                        ?? throw new Exception("Project is not found.");
            var devices = Context.Devices.Where(x => devicesIds.Contains(x.Id)).Except(project.Devices).ToArray();

            foreach (var employee in devices)
                if (action == ActionType.Add)
                    project.Devices.Add(employee);
                else
                    project.Devices.Remove(employee);
            AddOrUpdate(project);
            Context.SaveChanges();
            return devices.Length;
        }

        public void Delete(params IEntity[] entities)
        {
            Context.RemoveRange(entities);
            Context.SaveChanges();
        }

        #region IDisposable implementation
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public enum ActionType
    {
        Add,
        Remove
    }
}