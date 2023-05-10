using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using project_managet_dblayer;
using project_managet_models;
using project_managet_models.Models;
using project_managet_server.Services;

namespace project_managet_server.Controllers
{
    /// <summary>
    /// Contains project instance.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        readonly EntityGateway _db = new();
        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ?
                                    Request.Headers["Token"]! : Guid.Empty.ToString());

        /// <summary>
        /// List of all projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "ok",
                projects = _db.GetProjects()
            });
        }
        /// <summary>
        /// Project by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var potentialProject = _db.GetProjects(x => x.Id == id).FirstOrDefault();
            return potentialProject is null
                ? NotFound(new
                {
                    status = "fail",
                    message = $"There is no project with this id {id}!"
                })
                : Ok(new
                {
                    status = "ok",
                    project = potentialProject,
                    employees = potentialProject.Employees,
                    useddevices = potentialProject.Devices
                });
        }

        /// <summary>
        /// Add or update project info
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] Project value)
        {
            try
            {
                if (LocalAuthService.GetInstance().GetRole(Token) != Role.Admin)
                    return Unauthorized(new
                    {
                        status = "fail",
                        message = "You have no rights for this op."
                    });
                _db.AddOrUpdate(value);
                return Ok(new
                {
                    status = "ok",
                    id = value.Id
                });
            }
            catch (Exception E)
            {
                return BadRequest(new
                {
                    status = "fail",
                    message = E.Message
                });
            }
        }

        /// <summary>
        /// Employees on the project
        /// </summary>
        [HttpGet]
        [Route("{id}/employees")]
        public IActionResult GetEmployeesInProject([FromRoute] Guid id)
        {
            var potentialProject = _db.GetProjects(x => x.Id == id).FirstOrDefault();
            return potentialProject is null ?
                   NotFound(new
                   {
                       status = "fail",
                       message = $"There is no project with this id {id}!"
                   }) :
                   Ok(new
                   {
                       status = "ok",
                       employees = potentialProject.Employees
                   });
        }

        /// <summary>
        /// Add employees to project
        /// </summary>
        /// <param name="id">project id</param>
        /// <param name="employees">Json array of employees id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/employees/add")]
        public IActionResult AddEmployeesInProject([FromRoute] Guid id, [FromBody] Guid[] employees)
        {
            try
            {
                if (LocalAuthService.GetInstance().GetRole(Token) != Role.Admin)
                    return Unauthorized(new
                    {
                        status = "fail",
                        message = "You have no rights for this op."
                    });

                _db.EmployeesInProject(ActionType.Add, id, employees);
                return Ok(new
                {
                    status = "ok"
                });
            }
            catch (Exception E)
            {
                return BadRequest(new
                {
                    status = "fail",
                    message = E.Message
                });
            }
        }

        /// <summary>
        /// remove employees from project
        /// </summary>
        /// <param name="id">project id</param>
        /// <param name="employees">Json array of employees id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/employees/remove")]
        public IActionResult RemoveEmployeesInProject([FromRoute] Guid id, [FromBody] Guid[] employees)
        {
            try
            {
                if (LocalAuthService.GetInstance().GetRole(Token) != Role.Admin)
                    return Unauthorized(new
                    {
                        status = "fail",
                        message = "You have no rights for this op."
                    });

                _db.EmployeesInProject(ActionType.Remove, id, employees);
                return Ok(new
                {
                    status = "ok"
                });
            }
            catch (Exception E)
            {
                return BadRequest(new
                {
                    status = "fail",
                    message = E.Message
                });
            }
        }
    }
}