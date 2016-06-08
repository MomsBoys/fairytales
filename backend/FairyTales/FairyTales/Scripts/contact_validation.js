$(document).ready(function($) {
	function showError(container, errorMessage) {
		container.parent().append('<span class="error">' + errorMessage + '</error>');
	}

	function resetError(container) {
		if (container.next().hasClass('error')) {
			container.next().remove();
		}
	}

	function validateContactEmail(email) {
		/* Перевірка електронної пошти на наявність помилок */
		var re = /^[\w-\.]+@[\w-]+\.[a-z]{2,4}$/i;

		resetError(email);

		if (!email.val()) {
			showError(email, 'Вкажіть будь ласка Вашу електронну пошту');
		}

		if (email.val() && !re.test(email.val())) {
			showError(email, 'Ви ввели неправильну адресу. Повторіть будь ласка');
		}
	}

	function validateContactPass(pass) {
		resetError(pass);

		if (!pass.val()) {
			showError(pass, 'Введіть Ваш пароль');
		}

		var p = /^[a-zA-Z0-9]+$/;

		if (pass.val() && !(pass.val().length >= 8 && pass.val().length <= 20)) {
			showError(pass, 'Пароль повинен бути не менше 8 і не більше 20 символів');
		}

		if (pass.val() && !p.test(pass.val())) {
			showError(pass, 'Введено недопустимі символи. Вккористовуйте тільки латинські цифри і букви');
		}
	}

	$('#form_submit').click(function() {
		validateContactEmail($('[name=sender_email]'));

		/* Перевірка заголовку повідомлення */
		var message_title = $('[name=message_title]');

		resetError(message_title);

		if (!message_title.val()) {
			showError(message_title, 'Вкажіть будь ласка заголовок повідомлення');
		}

		/* Перевірка повідомлення */
		var message_text = $('[name=message_text]');

		resetError(message_text);

		if (!message_text.val()) {
			showError(message_text, 'Введіть Ваше повідомлення');
		}
	});
});