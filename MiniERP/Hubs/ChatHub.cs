using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MiniERP.Hubs   
{
    public class ChatHub : Hub
    {
        // Gửi tin nhắn từ user A tới user B (theo username)
        public async Task SendMessage(string fromUser, string toUser, string message)
        {
            // Thời gian hệ thống tại server
            var sentAt = DateTime.Now;

            // Hiện tại vẫn broadcast cho tất cả client,
            // client sẽ tự lọc theo from/to để hiển thị phù hợp.
            await Clients.All.SendAsync(
                "ReceiveMessage",
                fromUser,
                toUser,
                message,
                sentAt.ToString("HH:mm dd/MM/yyyy")
            );
        }
    }
}
