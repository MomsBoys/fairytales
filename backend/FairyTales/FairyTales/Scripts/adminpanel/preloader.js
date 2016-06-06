jQuery(window).load(function() {

	// Page Preloader
	jQuery('#status').fadeOut();
	jQuery('#preloader').delay(350).fadeOut(function(){
		jQuery('body').delay(350).css({'overflow':'visible'});
	});

});