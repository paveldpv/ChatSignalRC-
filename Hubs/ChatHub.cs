using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;


namespace ChatService.Hubs
{
    public class ChatHub:Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;
        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _botUser = "MyChat Bot";
            _connections = connections;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if(_connections.TryGetValue(Context.ConnectionId,out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} out in room {userConnection.Room}");

                SendConnectedUsers(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string message)
        {
            
            if (_connections.TryGetValue(Context.ConnectionId,out UserConnection userConnection))
            {
                Console.WriteLine("Write Message");
                Console.WriteLine(message);
                await Clients.Groups(userConnection.Room)
                    .SendAsync("ReceiveMessage", userConnection.User, message);
            }
        }
        public async Task JoinRoom(UserConnection userConnection)
        {
            Console.WriteLine(userConnection);
            await TestMethod(userConnection);

            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);
            _connections[Context.ConnectionId] = userConnection;
            await Clients.Groups(userConnection.Room).SendAsync("ReceiveMessage",
                _botUser,$"{userConnection.User} has joined {userConnection.Room}");

            await SendConnectedUsers(userConnection.Room);
            
        }

        public Task SendConnectedUsers(string room)
        {
            var users = _connections.Values.
                Where(c => c.Room == room)
                .Select(c => c.User);

            Console.WriteLine(users);
            return Clients.Group(room).SendAsync("UsersInRoom",users);
        }
        public Task TestMethod(UserConnection userConnection)
        {
            return Clients.Group(userConnection.Room).SendAsync("TestMethod", "this  bla vla dsadsa test method");
        }

    }
}
