"use strict";

var EPMO = window.EPMO || {};
EPMO.Sliders = EPMO.Sliders || {};

EPMO.Sliders.displaySlider = function(divId){

	var siteurl = _spPageContextInfo.webAbsoluteUrl;

  $.ajax({
    url: siteurl + "/_api/web/lists/getbytitle('sliders')/items",
    method: "GET",
    headers: { "Accept": "application/json; odata=verbose" },
    success: function (data) {
      if (data.d.results.length > 0 ) {
        var _sliders = data.d.results;
        displaySlider(_sliders, divId);
      }       
    },
    error: function (data) {
      Console.log("Error: "+ data);
    }
  });

  function displaySlider(slider, divId){
    var totalSlider = slider.length;
    var iter = 0;

    $("#"+divId).append('<img id="slider-image" src="' + slider[iter].image.Description + '"/>');
    $("#"+divId).append('<div id="slider-caption" class="tp-caption light_medium_30_shadowed" style="z-index: 2; max-width: auto; max-height: auto; white-space: nowrap;">' + '<a href="' + slider[iter].link + '" target="_blank">' + slider[iter].Title + '</a>'+ '</div>');

    setInterval(function(){
      var currentIndex = ++iter % totalSlider;
      $('#slider-image').attr('src', slider[currentIndex].image.Description);
      $('#slider-caption').html('<a href="' + slider[currentIndex].link + '" target="_blank">' + slider[currentIndex].Title + '</a>');
    }, 3000);
    
  }

};