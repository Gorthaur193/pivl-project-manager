using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_managet_dblayer;
using project_managet_models.Models;
using project_managet_models;
using project_managet_server.Services;

namespace project_managet_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EntityGateway _db = new();
        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ?
                                    Request.Headers["Token"]! : Guid.Empty.ToString());

        /// <summary>
        /// Get all jobtitles
        /// </summary>
        [HttpGet]
        public IActionResult GetAll() =>
            Ok(new
            {
                status = "ok",
                jobtitles = _db.GetEmployees()
            });

        /// <summary>
        /// Get job title by id. 
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById(Guid id)
        {
            var potentialEmployee = _db.GetEmployees(x => x.Id == id).FirstOrDefault();
            if (potentialEmployee is not null)
                return Ok(new
                {
                    status = "ok",
                    jobtitles = potentialEmployee, 
                    projects = potentialEmployee.Projects.Select(x => x.Id)
                });
            else
                return NotFound(new
                {
                    status = "fail",
                    message = $"There is no employee with {id} id"
                });
        }

        /// <summary>
        /// Get supervisor
        /// </summary>
        [HttpGet]
        [Route("{id}/{subdata}")]
        public IActionResult GetSubdata([FromRoute] Guid id, [FromRoute] EmployeeSubData subData)
        {
            var potentialEmployee = _db.GetEmployees(x => x.Id == id).FirstOrDefault();
            object res = subData switch
            {
                EmployeeSubData.Supervisor => new
                {
                    status = "ok",
                    supervisor = potentialEmployee?.Supervisor
                },
                EmployeeSubData.Supervisees => new
                {
                    status = "ok",
                    supervisee = potentialEmployee?.Supervisees
                },
                EmployeeSubData.Projects => new
                {
                    status = "ok",
                    projects = potentialEmployee?.Projects
                },
                _ => throw new Exception($"{subData} instance is not covered.")
            };

            if (potentialEmployee is not null)
                return Ok(res);
            else
                return NotFound(new
                {
                    status = "fail",
                    message = $"There is no employee with {id} id"
                });
        }

        /// <summary>
        /// Set supervisor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="supervisorId">if null, then removes supervisor</param>
        [HttpGet]
        [Route("{id}/supervisor")]
        public IActionResult SetSupervisor([FromRoute] Guid id, Guid? supervisorId)
        {
            try
            {
                Employee potentialEmployee = _db.GetEmployees(x => x.Id == id).FirstOrDefault() ?? throw new Exception("Employee is not found.");
                Employee? potentialSupervisor = supervisorId is not null
                    ? _db.GetEmployees(x => x.Id == supervisorId).FirstOrDefault() ?? throw new Exception("Supervisor is not found.")
                    : null;
                potentialEmployee.Supervisor = potentialSupervisor;
                _db.AddOrUpdate(potentialEmployee);
                return Ok(new
                {
                    status = "ok"
                });
            }
            catch (Exception E)
            {
                return NotFound(new
                {
                    status = "fail",
                    message = E.Message
                }));
            }

        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum EmployeeSubData
        {
            Supervisor,
            Supervisees,
            Projects
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
