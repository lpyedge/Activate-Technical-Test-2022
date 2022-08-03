(function ($) {
    $.extend({
        emailservice: function (email, option) {
            option = $.extend({
                size: '0.98rem',
                bgcolor: '#1296db',
                color: '#ffffff',
                location: 'br',
                offset: { 'x': '0.4rem', 'y': '0.4rem'}
            }, option);
            var stylelocation = {};
            var spacewidth = ((window.innerWidth - document.body.clientWidth) / 2) + 'px';
            var halfwidth = (window.innerWidth / 2) + 'px';
            var halfheight = (window.innerHeight / 2) + 'px';
            switch (option.location) {
                case 'tl':
                    stylelocation = { "top": option.offset.y, "left": "calc(" + option.offset.x + " + " + spacewidth + ")" };
                    break;
                case 'tr':
                    stylelocation = { "top": option.offset.y, "right": "calc(" + option.offset.x + " + " + spacewidth + ")" };
                    break;
                case 'bl':
                    stylelocation = { "bottom": option.offset.y, "left": "calc(" + option.offset.x + " + " + spacewidth + ")" };
                    break;
                case 'br':
                    stylelocation = { "bottom": option.offset.y, "right": "calc(" + option.offset.x + " + " + spacewidth + ")" };
                    break;
                case 'b':
                    stylelocation = { "bottom": option.offset.y, "right": "calc(" + halfwidth + " - " + halfnumberwithunit(option.size) + ")" };
                    break;
                case 't':
                    stylelocation = {"top": option.offset.y ,"left": "calc(" + halfwidth + " - " + halfnumberwithunit(option.size) + ")" };
                    break;
                case 'l':
                    stylelocation = { "top": "calc(" + halfheight + " - " + halfnumberwithunit(option.size) + ")", "left": "calc(" + option.offset.x + " + " + spacewidth + ")" };
                    break;
                case 'r':
                    stylelocation = { "top": "calc(" + halfheight + " - " + halfnumberwithunit(option.size) + ")", "right": "calc(" + option.offset.x + " + " + spacewidth + ")" };
                    break;
                default:
            }
            var icon = '<svg viewBox="0 0 1024 1024" xmlns="http://www.w3.org/2000/svg" width="100%" height="100%"><path d="M874.666667 375.189333V746.666667a64 64 0 0 1-64 64H213.333333a64 64 0 0 1-64-64V375.189333l266.090667 225.6a149.333333 149.333333 0 0 0 193.152 0L874.666667 375.189333zM810.666667 213.333333a64.789333 64.789333 0 0 1 22.826666 4.181334 63.616 63.616 0 0 1 26.794667 19.413333 64.32 64.32 0 0 1 9.344 15.466667c2.773333 6.570667 4.48 13.696 4.906667 21.184L874.666667 277.333333v21.333334L553.536 572.586667a64 64 0 0 1-79.893333 2.538666l-3.178667-2.56L149.333333 298.666667v-21.333334a63.786667 63.786667 0 0 1 35.136-57.130666A63.872 63.872 0 0 1 213.333333 213.333333h597.333334z" fill="[color]"></path></svg>';
                        
            if (email) {                
                var $floatemailservice = $("#float_email_service");
                if (!$floatemailservice.length || $floatemailservice.length === 0) {
                    $floatemailservice = $("<div id='float_email_service'></div>");
                    $floatemailservice.css($.extend({
                        "z-index": "999",
                        "display": "none",
                        "border-radius": "100%",
                        "position": "fixed",
                        "background-color": option.bgcolor,
                        "color": option.color,
                        "width": option.size,
                        "height": option.size,
                    }, stylelocation));
                    var $aLink = $("<a></a>");
                    $aLink.attr("href", "mailto:" + email);
                    $aLink.attr("target", "_blank");
                    $aLink.css({ "width": "100%", "height": "100%", "display": "flex" });
                    $aLink.html(icon.replace("[color]",option.color));
                    $aLink.appendTo($floatemailservice);
                    $floatemailservice.appendTo('body'); 
                }
                $floatemailservice.show();
            }
        }
    });
    function halfnumberwithunit(num) {
        var tempnum = Number.parseFloat(num);
        var unit = num.replace(tempnum, '');
        return tempnum / 2 + unit;
    }
})(jQuery);