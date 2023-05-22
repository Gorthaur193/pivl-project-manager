using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project_managet_dblayer;
using project_managet_server.Services;

namespace project_managet_server.Controllers
{
    [Route("api/projects/{project_id}/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        readonly LocalAuthService _localAuthService = LocalAuthService.GetInstance();
        readonly EntityGateway _db = new();

        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ?
                                            Request.Headers["Token"]! : Guid.Empty.ToString());

        readonly ChatService _chatService = ChatService.GetInstance();

        [HttpGet]
        public async Task<IActionResult> ConnectUser([FromRoute] Guid project_id, Guid token)
        {
            var potentialProject = _db.GetProjects(x => x.Id == project_id).FirstOrDefault();
            if (potentialProject is null) 
                return NotFound(new
                {
                    status = "fail",
                    message = "There is no project with this Id."
                });
            if (!ControllerContext.HttpContext.WebSockets.IsWebSocketRequest)
                return BadRequest(new
                {
                    status = "fail",
                    message = "Unsupported action!"
                });
            var socket = await ControllerContext.HttpContext.WebSockets.AcceptWebSocketAsync();
            try
            {
                var user = _localAuthService.GetUser(token);
                await Task.Delay(TimeSpan.FromMilliseconds(-1), 
                                _chatService.CreateConnection(user, potentialProject, socket));
                return Ok();
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
