"use strict";

var EPMO = window.EPMO || {};
EPMO.Charts = EPMO.Charts || {};
EPMO.Charts.Utils = EPMO.Charts.Utils || {};

EPMO.Charts.Utils.generateLegend = function(){

	var _legend = {
		item: {
			visual: function (e) {
				var color = e.series.color;
				for (var i=0;i<e.series.data.length;i++){
    				if (e.series.data[i].Color != "" && e.series.data[i].Color !== undefined && e.series.data[i].fname != "") {
    					color = e.series.data[i].Color
					}
				}

				var rect = new kendo.geometry.Rect([0, 0], [100, 50]);
				var layout = new kendo.drawing.Layout(rect, {
					spacing: 5,
					alignItems: "center"
				});

				var marker = new kendo.drawing.Path({
					fill: {
						color: color
					}
				}).moveTo(10, 0).lineTo(10, 10).lineTo(0, 10).lineTo(0,0).close();

				var label = new kendo.drawing.Text(e.series.name, [0, 0], {
					fill: {
						color: "black"
					}
				});

				layout.append(marker, label);
				layout.reflow()
				return layout;
			}
		}
	};

	return _legend;
};

EPMO.Charts.Utils.generateTitle =  function(e, title, center, radius) {
    var draw = kendo.drawing;
    var geom = kendo.geometry;
    var chart = e.sender;

    // The center and radius are populated by now.
    // We can ask a circle geometry to calculate the bounding rectangle for us.
    //
    // http://docs.telerik.com/kendo-ui/api/javascript/geometry/circle#methods-bbox
    var circleGeometry = new geom.Circle(center, radius);
    var bbox = circleGeometry.bbox();

    // Render the text
    //
    // http://docs.telerik.com/kendo-ui/api/javascript/dataviz/drawing/text
    var text = new draw.Text(title, [0, 0], {
      font: "18px Verdana,Arial,sans-serif"
    });

    // Align the text in the bounding box
    //
    // http://docs.telerik.com/kendo-ui/api/javascript/drawing#methods-align
    // http://docs.telerik.com/kendo-ui/api/javascript/drawing#methods-vAlign
    draw.align([text], bbox, "center");
    draw.vAlign([text], bbox, "center");

    // Draw it on the Chart drawing surface
    e.sender.surface.draw(text);
}