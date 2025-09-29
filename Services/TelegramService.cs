using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HotelWebsite.Services
{
    public class TelegramService
    {
        private readonly string _botToken;
        private readonly HttpClient _httpClient;

        public TelegramService(string botToken)
        {
            _botToken = botToken;
            _httpClient = new HttpClient();
        }

        public async Task<bool> SendMessageAsync(string chatId, string message)
        {
            try
            {
                var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
                var payload = new
                {
                    chat_id = chatId,
                    text = message,
                    parse_mode = "HTML"
                };

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}