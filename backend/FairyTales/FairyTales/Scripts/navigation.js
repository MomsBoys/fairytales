$(document).ready(function () {
    /* Add class current-menu-item to tags */
    var pgurl = window.location.pathname;

    /* Check if current page is homepage */
    $("#menu ul li.menu-item-home a").each(function () {
        if ($(this).attr("href") == '/index.php') {
            $(this).parent().addClass("current-menu-item");
        }
    });

    /* Check if current page is category page level 1 */
    $("#menu ul li a").each(function () {
        if ($(this).attr("href") == pgurl || $(this).attr("href") == '') {
            $('#menu ul li').removeClass("current-menu-item");
            $(this).parent().addClass("current-menu-item");
        }
    });

    /* Add menu toggle */
    jQuery(".responsive-menu-toggle").click(function () {
        jQuery(this).toggleClass("active");
        jQuery("#Header #menu").stop(true, true).slideToggle(200)
    });

    /* Display back-to-top button */

    /* hide #back-top first - $("#back-top").hide(); */

    // fade in #back-top
    $(function () {
        $(window).scroll(function () {
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
        $('#back-top a').click(function () {
            $('body,html').animate({
                scrollTop: 0
            }, 800);
            return false;
        });
    });
});