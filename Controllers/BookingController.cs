using Microsoft.AspNetCore.Mvc;
using HotelWebsite.Models;
using HotelWebsite.Services;
using System.Text.Json;

namespace HotelWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly TelegramService _telegramService;
        private readonly string _chatId;

        public BookingController()
        {
            // Замените на ваш chat_id (можно получить через @getmyid_bot)
            _chatId = "ВАШ_CHAT_ID";
            _telegramService = new TelegramService("8107760506:AAHspBy8Oq3T3EwOGbFC32QqcQMojKtgLcA");
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            try
            {
                var message = $@"🚀 <b>НОВАЯ ЗАЯВКА НА БРОНИРОВАНИЕ</b>

👤 <b>Имя:</b> {request.Name}
📞 <b>Телефон:</b> {request.Phone}
📧 <b>Email:</b> {request.Email}
🏨 <b>Номер:</b> {request.RoomType}
📅 <b>Заезд:</b> {request.Checkin}
📅 <b>Выезд:</b> {request.Checkout}
👥 <b>Гостей:</b> {request.Guests}
💬 <b>Пожелания:</b> {request.Message}";

                var success = await _telegramService.SendMessageAsync(_chatId, message);

                if (success)
                {
                    return Ok(new { success = true, message = "Заявка успешно отправлена" });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Ошибка при отправке в Telegram" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Ошибка сервера: {ex.Message}" });
            }
        }
    }
}