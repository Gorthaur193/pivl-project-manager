using Microsoft.EntityFrameworkCore;
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

        public int AddEmployeesToProject(Project project, params Employee[] employees)
        {
            if (Context.Entry(project).State == EntityState.Detached)
                project = Context.Projects.FirstOrDefault(x => x.Id == project.Id) ?? 
                    throw new Exception("Project dooes not exist.");
            List<Employee> employeesList = new(employees);
            for (int i = 0; i < employees.Length; i++)
            {
                Employee employee = employeesList[i];
                if (Context.Entry(employee).State == EntityState.Detached || 
                    (employee = Context.Employees.FirstOrDefault(x => x.Id == employee!.Id)!) is null)
                    employeesList.RemoveAt(i--);
            }
            var toChange = Context.Employees
                .Where(x => employeesList.Contains(x))
                .Except(project.Employees)
                .ToArray();



            AddOrUpdate(project);
            Context.SaveChanges();
            return toChange.Length;
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
}