jQuery(function($) {
	var pgurl = window.location.pathname;

	$("#adminmenu li a").each(function() {
		if ($(this).attr("href") === pgurl) {
			$("#adminmenu li").removeClass("current-menu-item");
			$(this).parent().addClass("current-menu-item");
		}
	});

	$("#adminmenu li ul li a").each(function() {
		if ($(this).attr("href") === pgurl) {
			$(this).parent().parent().parent().addClass("current-category-ancestor");
		}
	});

	$("#adminmenu li").each(function() {
	    if (pgurl.toLowerCase().indexOf("/adminpanel/edittale/") >= 0) {
	        $("#adminmenu li.nav-posts").addClass("current-menu-item");
	    }
        
		if (pgurl.toLowerCase().indexOf("/adminpanel/editcategory/") >= 0) {
			$("#adminmenu li.nav-cats").addClass("current-menu-item");
		}

		if (pgurl.toLowerCase().indexOf("/adminpanel/editauthor/") >= 0) {
			$("#adminmenu li.nav-authors").addClass("current-menu-item");
		}

		if (pgurl.toLowerCase().indexOf("/adminpanel/edittype/") >= 0) {
		    $("#adminmenu li.nav-types").addClass("current-menu-item");
		}

		if (pgurl.toLowerCase().indexOf("/adminpanel/edittag/") >= 0) {
		    $("#adminmenu li.nav-tags").addClass("current-menu-item");
		}

		if (pgurl.toLowerCase().indexOf("/adminpanel/edituser/") >= 0) {
		    $("#adminmenu li.nav-users").addClass("current-menu-item");
		}

		if ( pgurl === "/adminpanel/" ) {
			$("#adminmenu li:first-child").addClass("current-menu-item");
		}
	});
});