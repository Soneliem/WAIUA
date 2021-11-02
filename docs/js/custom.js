(function ($) {

  "use strict";

    // COLOR MODE
    $('.color-mode').click(function(){
        $('.color-mode-icon').toggleClass('active')
        $('body').toggleClass('dark-mode')
    })

    setInterval(function(){
        // toggle the class every five second
        $('.about-text > h1 > span').toggleClass('hover');

    },5000);

    // HEADER
    $(".navbar").headroom();


    // SMOOTHSCROLL
    $(function() {
      $('.nav-link, .custom-btn-link').on('click', function(event) {
        var $anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: $($anchor.attr('href')).offset().top - 49
        }, 1000);
        event.preventDefault();
      });
    });

    // TOOLTIP
    $('.social-links a').tooltip();



})(jQuery);
