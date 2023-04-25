using project_managet_models.Models;

namespace project_managet_dblayer
{
    public partial class EntityGateway
    {
        public IEnumerable<JobTitle> GetJobTitles(Func<JobTitle, bool> predicate) =>
            Context.JobTitles.Where(predicate).ToArray();
        public IEnumerable<JobTitle> GetJobTitles() =>
            GetJobTitles(x => true);

        public IEnumerable<Employee> GetEmployees(Func<Employee, bool> predicate) =>
            Context.Employees.Where(predicate).ToArray();
        public IEnumerable<Employee> GetEmployees() =>
            GetEmployees(x => true);

        public IEnumerable<Device> GetDevices(Func<Device, bool> predicate) =>
            Context.Devices.Where(predicate).ToArray();
        public IEnumerable<Device> GetDevices() =>
            GetDevices(x => true);

        public IEnumerable<Project> GetProjects(Func<Project, bool> predicate) =>
            Context.Projects.Where(predicate).ToArray();
        public IEnumerable<Project> GetProjects() =>
            GetProjects(x => true);

    }
}