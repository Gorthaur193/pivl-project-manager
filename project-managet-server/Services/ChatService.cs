using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using project_managet_models.Models;
using System.Net.WebSockets;
using System.Text;

namespace project_managet_server.Services
{
    internal class ChatService
    {
        public static ChatService GetInstance() => _instance ??= new();
        private static ChatService? _instance;
        private ChatService() { }

        private HashSet<ChatRoom> ChatRooms { get; set; } = new();
        public CancellationToken CreateConnection(Employee user, Project project, WebSocket webSocket)
        {
            var room = ChatRooms.FirstOrDefault(x=> x.Project.Id == project.Id);
            if (room is null)
                ChatRooms.Add(room = new(project));
            return new ChatUser(user, webSocket, room).SocketToken;
        }
    }

    internal class ChatRoom
    {
        public ChatRoom(Project project)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
        }

        public Project Project { get; private set; }
        public HashSet<ChatUser> Users { get; private set; } = new();

        public void SendMessage(object message, ChatUser? source = null)
        {
            Users.RemoveWhere(x => x.SocketToken.IsCancellationRequested || x.Socket.State != WebSocketState.Open);
            foreach (var user in Users)
                if (user != source)
                    user.SendMessage(message);
        }
    }

    internal class ChatUser
    {
        public ChatUser(Employee @base, WebSocket socket, ChatRoom chatRoom)
        {
            Base = @base ?? throw new ArgumentNullException(nameof(@base));
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            ChatRoom = chatRoom ?? throw new ArgumentNullException(nameof(chatRoom));
            ChatRoom.Users.Add(this);

            Task.Run(ListenerTask, SocketToken);
        }

        public void SendMessage(string message) =>
            Socket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, SocketToken);
        public void SendMessage(object message) =>
            SendMessage(JObject.FromObject(message).ToString());

        private async void ListenerTask()
        {
            while (Socket.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new(new byte[4096]);
                var socketMessage = await Socket.ReceiveAsync(buffer, SocketToken);
                if (SocketToken.IsCancellationRequested || socketMessage.MessageType == WebSocketMessageType.Close)
                    break;
                try
                {
                    var json = JObject.Parse(Encoding.UTF8.GetString(buffer));
                    switch ((string?)json["status"])
                    {
                        case "message":
                            ChatRoom.SendMessage(json.ToString(), this);
                            // todo: save to db
                            break;
                        case "close":
                            ChatRoom.SendMessage(new
                            {
                                status = "message",
                                message = $"{Base.Name} is offline!"
                            }, this);
                            SocketTokenSource.Cancel();
                            break;
                        case "notify": // todo: implement ;)
                        case "photo":  // todo: implement ;)
                            throw new NotImplementedException();
                        case null:
                        default:
                            throw new Exception();
                    }
                }
                catch 
                {
                    SendMessage(new
                    {
                        status = "fail"
                    });
                }
            }
        }

        public Employee Base { get; private set; }
        public WebSocket Socket { get; private set; }
        public ChatRoom ChatRoom { get; private set; }

        private CancellationTokenSource SocketTokenSource { get; set; } = new();
        public CancellationToken SocketToken => SocketTokenSource.Token;
    }
}
