using Microsoft.AspNetCore.Mvc;
using HotelWebsite.Models;
using HotelWebsite.Services;
using System.Text.Json;

namespace HotelWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly TelegramService _telegramService;
        private readonly string _chatId;
        private readonly string _reviewsFile;

        public ReviewsController()
        {
            _chatId = "ВАШ_CHAT_ID";
            _telegramService = new TelegramService("8107760506:AAHspBy8Oq3T3EwOGbFC32QqcQMojKtgLcA");
            _reviewsFile = Path.Combine(Directory.GetCurrentDirectory(), "reviews.json");
        }

        [HttpGet]
        public IActionResult GetReviews()
        {
            try
            {
                if (!System.IO.File.Exists(_reviewsFile))
                {
                    return Ok(new List<Review>());
                }

                var json = System.IO.File.ReadAllText(_reviewsFile);
                var reviews = JsonSerializer.Deserialize<List<Review>>(json) ?? new List<Review>();
                
                // Сортируем по дате (новые первыми)
                reviews = reviews.OrderByDescending(r => r.Date).ToList();
                
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Ошибка загрузки отзывов: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] Review review)
        {
            try
            {
                // Загружаем существующие отзывы
                var reviews = new List<Review>();
                if (System.IO.File.Exists(_reviewsFile))
                {
                    var json = System.IO.File.ReadAllText(_reviewsFile);
                    reviews = JsonSerializer.Deserialize<List<Review>>(json) ?? new List<Review>();
                }

                // Добавляем новый отзыв
                review.Id = reviews.Count > 0 ? reviews.Max(r => r.Id) + 1 : 1;
                review.Date = DateTime.Now;
                reviews.Add(review);

                // Сохраняем
                var options = new JsonSerializerOptions { WriteIndented = true };
                var newJson = JsonSerializer.Serialize(reviews, options);
                System.IO.File.WriteAllText(_reviewsFile, newJson);

                // Отправляем уведомление в Telegram
                var telegramMessage = $@"📝 <b>НОВЫЙ ОТЗЫВ</b>

👤 <b>От:</b> {review.Name}
💬 <b>Текст:</b> {review.Text}
📅 <b>Дата:</b> {review.Date:dd.MM.yyyy HH:mm}";

                await _telegramService.SendMessageAsync(_chatId, telegramMessage);

                return Ok(new { success = true, message = "Отзыв успешно добавлен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Ошибка добавления отзыва: {ex.Message}" });
            }
        }
    }
}