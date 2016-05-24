$(document).ready(function() {

    /**
     * Navigation Panel
     */
    /* Add class current-menu-item to tags */
    var pgurl = window.location.pathname;

    /* Check if current page is homepage */
    $("#menu ul li.menu-item-home a").each(function() {
        if ($(this).attr("href") == '/index.php') {
            $(this).parent().addClass("current-menu-item");
        }
    });

    /* Check if current page is category page level 1 */
    $("#menu ul li a").each(function() {
        if ($(this).attr("href") == pgurl || $(this).attr("href") == '') {
            $('#menu ul li').removeClass("current-menu-item");
            $(this).parent().addClass("current-menu-item");
        }
    });

    /* Add menu toggle */
    $(".responsive-menu-toggle").click(function() {
        $(this).toggleClass("active");
        $("#Header #menu").stop(true, true).slideToggle(200)
    });

    /* Display back-to-top button */

    /* hide #back-top first - $("#back-top").hide(); */

    // fade in #back-top
    $(function() {
        $(window).scroll(function() {
            if ($(this).scrollTop() > 160) {
                // $('#back-top').fadeIn();
                if ($('#back-top').position().top == '-60') {
                    $('#back-top').animate({
                        opacity: 1,
                        top: "60px"
                    }, 800);
                }
            } else {
                if ($('#back-top').position().top == '60') {
                    $('#back-top').animate({
                        opacity: 0,
                        top: "-60px"
                    }, 800);
                }
            }
        });

        // scroll body to 0px on click
        $('#back-top a').click(function() {
            $('body,html').animate({
                scrollTop: 0
            }, 800);
            return false;
        });
    });


    /**
     * Login Panel
     */
    var formModal = $('.cd-user-modal'),
        formLogin = formModal.find('#cd-login'),
        formSignup = formModal.find('#cd-signup'),
        formModalTab = $('.cd-switcher'),
        tabLogin = formModalTab.children('li').eq(0).children('a'),
        tabSignup = formModalTab.children('li').eq(1).children('a'),
        mainNav = $('#Header');

    // Open modal
    mainNav.on('click', function(event) {
        $(event.target).is(mainNav) && mainNav.children('ul').toggleClass('is-visible');
    });

    // Open sign-up form
    mainNav.on('click', '.cd-signup', signup_selected);

    // Open login-form form
    mainNav.on('click', '.cd-signin', login_selected);

    // Close modal
    formModal.on('click', function(event) {
        if ($(event.target).is(formModal) || $(event.target).is('.cd-close-form')) {
            formModal.removeClass('is-visible');
        }
    });

    // Close modal when clicking the esc keyboard button
    $(document).keyup(function(event) {
        if (event.which == '27') {
            formModal.removeClass('is-visible');
        }
    });

    // Switch from a tab to another
    formModalTab.on('click', function(event) {
        event.preventDefault();
        ($(event.target).is(tabLogin)) ? login_selected() : signup_selected();
    });

    // Hide or show password
    $('.hide-password').on('click', function() {
        var togglePass = $(this),
            passwordField = togglePass.prev('input');

        ('password' == passwordField.attr('type')) ? passwordField.attr('type', 'text') : passwordField.attr('type', 'password');
        ('Сховати' == togglePass.text()) ? togglePass.text('Показати') : togglePass.text('Сховати');

        // Focus and move cursor to the end of input field
        passwordField.putCursorAtEnd();
    });

    function login_selected() {
        mainNav.children('ul').removeClass('is-visible');
        formModal.addClass('is-visible');
        formLogin.addClass('is-selected');
        formSignup.removeClass('is-selected');
        tabLogin.addClass('selected');
        tabSignup.removeClass('selected');
    }

    function signup_selected() {
        mainNav.children('ul').removeClass('is-visible');
        formModal.addClass('is-visible');
        formLogin.removeClass('is-selected');
        formSignup.addClass('is-selected');
        tabLogin.removeClass('selected');
        tabSignup.addClass('selected');
    }

    // REMOVE THIS - it's just to show error messages 
    formLogin.find('input[type="submit"]').on('click', function(event) {
        event.preventDefault();
        formLogin.find('input[type="email"]').toggleClass('has-error').next('span').toggleClass('is-visible');
    });
    formSignup.find('input[type="submit"]').on('click', function(event) {
        event.preventDefault();
        formSignup.find('input[type="email"]').toggleClass('has-error').next('span').toggleClass('is-visible');
    });

    // // IE9 placeholder fallback
    // if(!Modernizr.input.placeholder){
    // 	$('[placeholder]').focus(function() {
    // 		var input = $(this);
    // 		if (input.val() == input.attr('placeholder')) {
    // 			input.val('');
    // 	  	}
    // 	}).blur(function() {
    // 	 	var input = $(this);
    // 	  	if (input.val() == '' || input.val() == input.attr('placeholder')) {
    // 			input.val(input.attr('placeholder'));
    // 	  	}
    // 	}).blur();
    // 	$('[placeholder]').parents('form').submit(function() {
    // 	  	$(this).find('[placeholder]').each(function() {
    // 			var input = $(this);
    // 			if (input.val() == input.attr('placeholder')) {
    // 		 		input.val('');
    // 			}
    // 	  	})
    // 	});
    // }

    $.fn.putCursorAtEnd = function() {
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

    /**
     * Slider
     */
    $("#slider-info").immersive_slider({
        animation: "slide",
        cssBlur: true
    });
});