(function($) {

    $.fn.zoompanzoom = function(options) {

        var settings = $.extend({
            animationSpeed: "fast",
            zoomfactor: .1,
            maxZoom: 3,
            minZoom: 0.5,
            disablePan: false
        }, options);

        var zoomdiv = this;
        currentZoom = 1.0;

        if (!settings.disablePan) {
            $(this).draggable();
        }

        jQuery('#zoom_in').click(
            function() {
                if (currentZoom < settings.maxZoom) {
                    jQuery(zoomdiv).animate({
                        'zoom': currentZoom += settings.zoomfactor
                    }, settings.animationSpeed);
                }

            })
        jQuery('#zoom_out').click(
            function() {
                if (currentZoom > settings.minZoom) {
                    jQuery(zoomdiv).animate({
                        'zoom': currentZoom -= settings.zoomfactor
                    }, settings.animationSpeed);
                }

            })
        jQuery('#zoom_reset').click(
            function() {
                currentZoom = 1.0;
                jQuery(zoomdiv).animate({
                    'zoom': 1
                }, settings.animationSpeed);
            })
    };

}(jQuery));