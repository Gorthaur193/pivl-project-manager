using Microsoft.AspNetCore.Mvc;
using project_managet_dblayer;
using project_managet_models;
using project_managet_models.Models;
using project_managet_server.Services;

namespace project_managet_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        readonly EntityGateway _db = new();
        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ?
                                    Request.Headers["Token"]! : Guid.Empty.ToString());

        /// <summary>
        /// List of all devices
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "ok",
                projects = _db.GetDevices()
            });
        }
        /// <summary>
        /// Device by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var potentialDevice = _db.GetDevices(x => x.Id == id).FirstOrDefault();
            return potentialDevice is null
                ? NotFound(new
                {
                    status = "fail",
                    message = $"There is no device with this id {id}!"
                })
                : Ok(new
                {
                    status = "ok",
                    device = potentialDevice,
                    projects = potentialDevice.Projects.Select(x => x.Id)
                });
        }

        /// <summary>
        /// Add or update device info
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] Device value)
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
        /// 
        /// </summary>
        [HttpGet]
        [Route("{id}/projects")]
        public IActionResult GetProjectsInDevice([FromRoute] Guid id)
        {
            var potentialDevice = _db.GetDevices(x => x.Id == id).FirstOrDefault();
            return potentialDevice is null
                ? NotFound(new
                {
                    status = "fail",
                    message = $"There is no project with this id {id}!"
                })
                : Ok(new
                {
                    status = "ok",
                    projects = potentialDevice.Projects
                });
        }
    }
}