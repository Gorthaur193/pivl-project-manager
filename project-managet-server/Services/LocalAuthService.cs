using project_managet_dblayer;
using project_managet_models;
using project_managet_models.Models;

namespace project_managet_server.Services
{
    public class LocalAuthService
    {
        class Session
        {
            public Employee User { get; set; }
            public DateTime LastOp { get; set; }
            public Guid Token { get; set; }

            public bool IsActive => DateTime.Now - LastOp < TimeSpan.FromHours(1);
        }
        public void CleanSessions() =>
            Task.Run(() =>
                Sessions.RemoveWhere(x => !x.IsActive));

        private static LocalAuthService? _instance;
        public static LocalAuthService GetInstance() => _instance ??= new();
        private LocalAuthService() { }

        private HashSet<Session> Sessions { get; set; } = new();
        private readonly EntityGateway db = new();
        public Guid Auth(string username, string password)
        {
            CleanSessions();
            var passhash = Extentions.ComputeSHA256(password);
            var potentialUser = db.GetEmployees(x => x.Login == username && x.Passhash == passhash).FirstOrDefault() ?? throw new Exception("User is not found.");

            var token = Guid.NewGuid();
            Sessions.Add(new()
            {
                User = potentialUser,
                LastOp = DateTime.Now,
                Token = token
            });
            return token;
        }

        public Role GetRole(Guid token)
        {
            CleanSessions();
            return Sessions.FirstOrDefault(x => x.Token == token)?.User.Role ?? throw new Exception("User is not found");
        }
    }
}
