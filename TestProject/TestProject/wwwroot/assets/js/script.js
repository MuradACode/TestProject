$(document).ready(function(){
    $(window).scroll(function(){
        let scroll = $(window).scrollTop();

        if (scroll > 10) {
            $(".navBar").css("background-color", "white");
        }
        else{
            $(".navBar").css("background-color", "transparent");
        }
    });
});