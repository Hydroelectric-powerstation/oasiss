// API базовый URL
const API_BASE_URL = 'https://localhost:7000/api';

// Загрузка при старте
document.addEventListener('DOMContentLoaded', function() {
    loadReviews();
    
    // Обработчики форм
    document.getElementById('bookingForm').addEventListener('submit', handleBookingSubmit);
    document.getElementById('reviewForm').addEventListener('submit', handleReviewSubmit);
    
    // Плавная прокрутка
    document.querySelectorAll('nav a').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            const targetElement = document.querySelector(targetId);
            
            if (targetElement) {
                window.scrollTo({
                    top: targetElement.offsetTop - 80,
                    behavior: 'smooth'
                });
            }
        });
    });
});

// Отправка бронирования
async function handleBookingSubmit(e) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const bookingData = {
        name: formData.get('name'),
        phone: formData.get('phone'),
        email: formData.get('email'),
        roomType: formData.get('roomType'),
        checkin: formData.get('checkin'),
        checkout: formData.get('checkout'),
        guests: parseInt(formData.get('guests')),
        message: formData.get('message')
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/booking`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(bookingData)
        });
        
        if (response.ok) {
            showNotification('Заявка успешно отправлена! Мы свяжемся с вами в ближайшее время.');
            e.target.reset();
        } else {
            const error = await response.json();
            showNotification('Ошибка при отправке заявки: ' + (error.message || 'Неизвестная ошибка'));
        }
    } catch (error) {
        console.error('Ошибка:', error);
        showNotification('Ошибка при отправке заявки. Пожалуйста, попробуйте позже.');
    }
}

// Отправка отзыва
async function handleReviewSubmit(e) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const reviewData = {
        name: formData.get('name'),
        text: formData.get('text')
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/review`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(reviewData)
        });
        
        if (response.ok) {
            showNotification('Отзыв успешно отправлен! Спасибо за ваше мнение.');
            e.target.reset();
            loadReviews(); // Перезагружаем отзывы
        } else {
            const error = await response.json();
            showNotification('Ошибка при отправке отзыва: ' + (error.message || 'Неизвестная ошибка'));
        }
    } catch (error) {
        console.error('Ошибка:', error);
        showNotification('Ошибка при отправке отзыва. Пожалуйста, попробуйте позже.');
    }
}

// Загрузка отзывов
async function loadReviews() {
    try {
        const response = await fetch(`${API_BASE_URL}/reviews`);
        const reviews = await response.json();
        
        const reviewsList = document.getElementById('reviewsList');
        reviewsList.innerHTML = '';
        
        if (reviews.length === 0) {
            reviewsList.innerHTML = '<p>Пока нет отзывов. Будьте первым!</p>';
            return;
        }
        
        reviews.forEach(review => {
            const reviewElement = document.createElement('div');
            reviewElement.className = 'review';
            
            const date = new Date(review.date).toLocaleDateString('ru-RU', {
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            });
            
            reviewElement.innerHTML = `
                <div class="review-header">
                    <div class="review-name">${review.name}</div>
                    <div class="review-date">${date}</div>
                </div>
                <div class="review-text">${review.text}</div>
            `;
            
            reviewsList.appendChild(reviewElement);
        });
    } catch (error) {
        console.error('Ошибка при загрузке отзывов:', error);
        document.getElementById('reviewsList').innerHTML = '<p>Ошибка при загрузке отзывов.</p>';
    }
}

// Показать уведомление
function showNotification(message, type = 'info') {
    const notification = document.getElementById('notification');
    notification.textContent = message;
    notification.className = 'notification show';
    
    setTimeout(() => {
        notification.classList.remove('show');
    }, 5000);
}