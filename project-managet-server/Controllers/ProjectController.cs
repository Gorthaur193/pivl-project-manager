using Microsoft.AspNetCore.Mvc;
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
                    employees = potentialProject.Employees.Select(x => x.Id),
                    useddevices = potentialProject.Devices.Select(x => x.Id)
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
        [Route("{id}/{subdata}")]
        public IActionResult GetSubdataFromProject([FromRoute] Guid id, [FromRoute] ProjectSubdata subdata)
        {
            var potentialProject = _db.GetProjects(x => x.Id == id).FirstOrDefault();
            object res = subdata switch
            {
                ProjectSubdata.Employees => new
                {
                    status = "ok",
                    employees = potentialProject?.Employees
                },
                ProjectSubdata.UsedDevices => new
                {
                    status = "ok",
                    useddevices = potentialProject?.Devices
                },
                _ => throw new Exception($"{subdata} instance is not covered.")
            };
            return potentialProject is null
                ? NotFound(new
                {
                    status = "fail",
                    message = $"There is no project with this id {id}!"
                })
                : Ok(res);
        }

        /// <summary>
        /// change subdataIds from project
        /// </summary>
        /// <param name="action"></param>
        /// <param name="subdata"></param>
        /// <param name="id">project id</param>
        /// <param name="subdataIds">Json array of subdataIds id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/{subdata}/{action}")]
        public IActionResult ManipulateSubDataInProject([FromRoute] ActionType action, 
                                                        [FromRoute] ProjectSubdata subdata, 
                                                        [FromRoute] Guid id, 
                                                        [FromBody] Guid[] subdataIds)
        {
            try
            {
                if (LocalAuthService.GetInstance().GetRole(Token) != Role.Admin)
                    return Unauthorized(new
                    {
                        status = "fail",
                        message = "You have no rights for this op."
                    });
                var changed = subdata switch
                {
                    ProjectSubdata.Employees => _db.EmployeesInProject(action, id, subdataIds),
                    ProjectSubdata.UsedDevices => _db.DevicesInProject(action, id, subdataIds),
                    _ => throw new Exception($"{subdata} instance is not covered.")
                };
                return Ok(new
                {
                    status = "ok",
                    changed
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

#pragma warning disable CS1591 
        public enum ProjectSubdata
        {
            UsedDevices,
            Employees
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}