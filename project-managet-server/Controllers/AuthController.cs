using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using project_managet_dblayer;
using project_managet_models;
using project_managet_models.Models;
using project_managet_server.Services;

namespace project_managet_server.Controllers
{
    /// <summary>
    /// Auth controller for auth and registration
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly LocalAuthService _localAuthService = LocalAuthService.GetInstance();
        readonly EntityGateway _db = new();

        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ?
                                            Request.Headers["Token"]! : Guid.Empty.ToString());

        /// <summary>
        /// Auth
        /// </summary>
        /// <returns>message with token</returns>
        [HttpPost]
        public IActionResult AuthPost(string username, string password)
        {
            try
            {
                var token = _localAuthService.Auth(username, password);
                return Ok(new
                {
                    status = "ok",
                    token
                });
            }
            catch (Exception E)
            {
                return Unauthorized(new
                {
                    status = "fail",
                    message = E.Message
                });
            }
        }

        /// <summary>
        /// Get rights of authentified user
        /// </summary>
        /// <returns>rights as string</returns>
        [HttpPost]
        [Route("rights")]
        public IActionResult CheckRights()
        {
            try
            {
                return Ok(new
                {
                    status = "ok",
                    rights = _localAuthService.GetRole(Token)
                });
            }
            catch (Exception E)
            {
                return Unauthorized(new
                {
                    status = "fail",
                    message = E.Message
                });
            }
        }

        /// <summary>
        /// Get user by it's token
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("user")]
        public IActionResult GetUserInfo()
        {
            try
            {
                return Ok(new
                {
                    status = "ok",
                    user = _localAuthService.GetUser(Token)
                });
            }
            catch (Exception E)
            {
                return Unauthorized(new
                {
                    status = "fail",
                    message = E.Message
                });
            }
        }

        /// <summary>
        /// register new Employee
        /// </summary>
        /// <param name="userJson">must contain login, password and name of new employee.</param>
        /// <returns>creates user with "NEW" PersonalId</returns>
        [HttpPost]
        [Route("signup")]
        public IActionResult SignUp([FromBody] JObject userJson)
        {
            try
            {
                if (_db.GetEmployees(x => x.Login == userJson["login"]?.ToString()).Any())
                    throw new Exception("User with this login exists");
                Employee potentialEmployee = new()
                {
                    Login = userJson["login"]?.ToString() ?? throw new Exception("Login is missing"),
                    Passhash = Extentions.ComputeSHA256(userJson["password"]?.ToString() ?? throw new Exception("Password is missing")),
                    Name = userJson["name"]?.ToString() ?? throw new Exception("Name is missing"),
                    Role = Role.User,
                    PersonalId = "NEW"
                };
                _db.AddOrUpdate(potentialEmployee);
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