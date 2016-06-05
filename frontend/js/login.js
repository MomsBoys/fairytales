jQuery(document).ready(function($){
	var formModal = $('.cd-user-modal'),
		formLogin = formModal.find('#cd-login'),
		formSignup = formModal.find('#cd-signup'),
		formModalTab = $('.cd-switcher'),
		tabLogin = formModalTab.children('li').eq(0).children('a'),
		tabSignup = formModalTab.children('li').eq(1).children('a'),
		mainNav = $('#Header');

	// Open modal
	mainNav.on('click', function(event){
		$(event.target).is(mainNav) && mainNav.children('ul').toggleClass('is-visible');
	});

	// Open sign-up form
	mainNav.on('click', '.cd-signup', signup_selected);

	// Open login-form form
	mainNav.on('click', '.cd-signin', login_selected);

	// Close modal
	formModal.on('click', function(event){
		if( $(event.target).is(formModal) || $(event.target).is('.cd-close-form') ) {
			formModal.removeClass('is-visible');
		}	
	});

	// Close modal when clicking the esc keyboard button
	$(document).keyup(function(event){
    	if(event.which=='27'){
    		formModal.removeClass('is-visible');
	    }
    });

	// Switch from a tab to another
	formModalTab.on('click', function(event) {
		event.preventDefault();
		( $(event.target).is( tabLogin ) ) ? login_selected() : signup_selected();
	});

	// Hide or show password
	$('.hide-password').on('click', function(){
		var togglePass= $(this),
			passwordField = togglePass.prev().prev('input');
		
		( 'password' == passwordField.attr('type') ) ? passwordField.attr('type', 'text') : passwordField.attr('type', 'password');
		( 'Сховати' == togglePass.text() ) ? togglePass.text('Показати') : togglePass.text('Сховати');

		// Focus and move cursor to the end of input field
		passwordField.putCursorAtEnd();
	});

	function login_selected(){
		mainNav.children('ul').removeClass('is-visible');
		formModal.addClass('is-visible');
		formLogin.addClass('is-selected');
		formSignup.removeClass('is-selected');
		tabLogin.addClass('selected');
		tabSignup.removeClass('selected');
	}

	function signup_selected(){
		mainNav.children('ul').removeClass('is-visible');
		formModal.addClass('is-visible');
		formLogin.removeClass('is-selected');
		formSignup.addClass('is-selected');
		tabLogin.removeClass('selected');
		tabSignup.addClass('selected');
	}

	var EMAIL_REGEX = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

	function validateEmail(email) {
		return EMAIL_REGEX.test(email);
	}

	function validatePassword(password) {
		return password.length > 0;
	}

	formLogin.find('input[type="submit"]').on('click', function(event) {
		var emailInput = formLogin.find('input[type="email"]');
		var passwordInput = formLogin.find('#signin-password');

		var isValidEmail = validateEmail(emailInput.val());
		defineErrorMessage(emailInput, isValidEmail);

		if (!isValidEmail) {
			event.preventDefault();
			return;
		}

		var isValidPassword = validatePassword(passwordInput.val());
		defineErrorMessage(passwordInput, isValidPassword);

		if (!isValidPassword) {
			event.preventDefault();
			return;
		}
	});

	formSignup.find('input[type="submit"]').on('click', function(event) {
		var emailInput = formSignup.find('input[type="email"]');
		var firstNameInput = formSignup.find('#signup-firstname');
		var lastNameInput = formSignup.find('#signup-lastname');
		var passwordInput = formSignup.find('#signup-password');

		var isValidFirstName = firstNameInput.val().length;
		defineErrorMessage(firstNameInput, isValidFirstName);

		if (!isValidFirstName) {
			event.preventDefault();
			return;
		}

		var isValidLastName = lastNameInput.val().length;
		defineErrorMessage(lastNameInput, isValidLastName);

		if (!isValidLastName) {
			event.preventDefault();
			return;
		}

		var isValidEmail = validateEmail(emailInput.val());
		defineErrorMessage(emailInput, isValidEmail);

		if (!isValidEmail) {
			event.preventDefault();
			return;
		}

		var isValidPassword = validatePassword(passwordInput.val());
		defineErrorMessage(passwordInput, isValidPassword);

		if (!isValidPassword) {
			event.preventDefault();
			return;
		}
	});

	function defineErrorMessage(formInput, isValid) {
		if (isValid) {
			formInput.removeClass('has-error').next('span').removeClass('is-visible');
		} else {
			formInput.addClass('has-error').next('span').addClass('is-visible');
			return;
		}
	}

	// IE9 placeholder fallback
	if(!Modernizr.input.placeholder){
		$('[placeholder]').focus(function() {
			var input = $(this);
			if (input.val() == input.attr('placeholder')) {
				input.val('');
		  	}
		}).blur(function() {
		 	var input = $(this);
		  	if (input.val() == '' || input.val() == input.attr('placeholder')) {
				input.val(input.attr('placeholder'));
		  	}
		}).blur();
		$('[placeholder]').parents('form').submit(function() {
		  	$(this).find('[placeholder]').each(function() {
				var input = $(this);
				if (input.val() == input.attr('placeholder')) {
			 		input.val('');
				}
		  	})
		});
	}
});

jQuery.fn.putCursorAtEnd = function() {
	return this.each(function() {
    	// If this function exists...
    	if (this.setSelectionRange) {
      		// ... then use it (Doesn't work in IE)
      		// Double the length because Opera is inconsistent about whether a carriage return is one character or two. Sigh.
      		var len = $(this).val().length * 2;
      		this.focus();
      		this.setSelectionRange(len, len);
    	} else {
    		// ... otherwise replace the contents with itself
    		// (Doesn't work in Google Chrome)
      		$(this).val($(this).val());
    	}
	});
};

// search bar sliding
$(function(){
  var $searchlink = $('#searchtoggl i');
  var $searchbar  = $('#searchbar');
  
  $('#searchtoggl').on('click', function(e){
    e.preventDefault();
    
    if($(this).attr('id') == 'searchtoggl') {
      if(!$searchbar.is(":visible")) { 
        // if invisible we switch the icon to appear collapsable
        $searchlink.removeClass('fa-search').addClass('fa-search-minus');
      } else {
        // if visible we switch the icon to appear as a toggle
        $searchlink.removeClass('fa-search-minus').addClass('fa-search');
      }
      
      $searchbar.slideToggle(300, function(){
        // callback after search bar animation
      });
    }
  });
  
  $('#searchform').submit(function(e){
    e.preventDefault(); // stop form submission
  });
});