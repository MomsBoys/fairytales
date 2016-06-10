jQuery(function($) {

	var pgurl = window.location.pathname;

	$("#adminmenu li a").each(function(){

		if ( $(this).attr("href") == pgurl ) {
			$('#adminmenu li').removeClass("current-menu-item");
			$(this).parent().addClass("current-menu-item");
		}

	});

	$("#adminmenu li ul li a").each(function() {

		if ( $(this).attr("href") == pgurl ) {
			$(this).parent().parent().parent().addClass("current-category-ancestor");
		}

	});

	$("#adminmenu li").each(function() {

		if ( pgurl == '/rev-admin/edit-article.php' ) {
			$('#adminmenu li.nav-posts').addClass("current-menu-item");
		}

		if ( pgurl == '/rev-admin/edit-category.php' ) {
			$('#adminmenu li.nav-cats').addClass("current-menu-item");
		}

		if ( pgurl == '/rev-admin/edit-author.php' ) {
			$('#adminmenu li.nav-authors').addClass("current-menu-item");
		}

		if ( pgurl == '/rev-admin/edit-comment.php' ) {
			$('#adminmenu li.nav-comments').addClass("current-menu-item");
		}

		if ( pgurl == '/rev-admin/' ) {
			$('#adminmenu li:first-child').addClass("current-menu-item");
		}

	});

});