var createForm = document.querySelector('.create__form');
var dateInput = document.getElementById('createDateOfBirth');

createForm.addEventListener('submit', function (event) {
    var dateValue = new Date(dateInput.value);
    var today = new Date();
    var minDate = new Date(1950, 0, 1); // Минимальная допустимая дата (1 января 1900 года)

    // Проверка: дата не должна быть в будущем
    if (dateValue > today) {
        alert('Дата рождения не может быть установлена на будущее.');
        event.preventDefault();
        return;
    }

    // Проверка: дата не должна быть слишком далекой в прошлом
    if (dateValue < minDate) {
        alert('Слишком высокий возраст.');
        event.preventDefault();
        return;
    }
});


var button = document.getElementById('add__button');

function adjustFooter() {
    const contentHeight = document.querySelector('main').offsetHeight;
    const headerHeight = document.querySelector('header').offsetHeight;
    const windowHeight = window.innerHeight;
    const footer = document.querySelector('footer');

    if (contentHeight+headerHeight+100 < windowHeight) {
        footer.style.position = 'fixed';
        footer.style.bottom = '0';
    } else {
        footer.style.position = 'static';
    }
}

button.addEventListener("change", adjustFooter);
window.addEventListener("load", adjustFooter);