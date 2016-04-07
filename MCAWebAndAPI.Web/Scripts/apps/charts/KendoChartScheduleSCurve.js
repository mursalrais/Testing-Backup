'use strict';

var EPMO = window.EPMO || {};
EPMO.Utils = EPMO.Utils || {};
EPMO.Charts = EPMO.Charts || {};
EPMO.Charts.SCurve = EPMO.Charts.SCurve || {};

EPMO.Charts.SCurve.Utils = (function () {

    var _url = EPMO.Utils.getHost() + "Task/GetProjectScheduleSCurveChart" + EPMO.Utils.getSiteUrl();
    var _legend = EPMO.Charts.Utils.generateLegend();
    _legend.position = "bottom";

    var _dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: _url,
                dataType: 'jsonp',
                type: 'GET'
            },
            parameterMap: function (options, operation) {
                if (operation !== "read") {
                    return { models: kendo.stringify(options.models || [options]) };
                }
            }
        },
        schema: {
            model: {
                fields: {
                    Category: { from: "Category", type: "string" },
                    Value: { from: "Value", type: "number" },
                    DateValue: { from: "DateValue", type: "date" },
                    Color: { from: "Color", type: "string" }
                }
            }
        },
        group: {
            field: 'Category'
        }
    });

    function drawTodayLine(e) {
        var axis = e.sender.getAxis("valueAxis");
        var slot = axis.slot(140);

        // Locate right-most category slot
        //
        var categoryAxis = e.sender.getAxis("categoryAxis");
        var categorySlot = categoryAxis.slot(new Date());

        // Render a line element
        //
        // http://docs.telerik.com/kendo-ui/api/javascript/dataviz/drawing/text
        var line = new kendo.drawing.Path({
            stroke: {
                color: "red",
                width: 3
            }
        });

        line.moveTo(categorySlot.origin).lineTo([categorySlot.origin.x, slot.origin.y]);

        // Render a text element
        //
        // http://docs.telerik.com/kendo-ui/api/javascript/dataviz/drawing/text
        var labelPos = [categorySlot.origin.x - 40, slot.origin.y + 50];
        var label = new kendo.drawing.Text("Today", labelPos, {
            fill: {
                color: "red"
            },
            font: "14px sans"
        });

        var group = new kendo.drawing.Group();
        group.append(line, label);

        // Draw on chart surface
        //
        // http://docs.telerik.com/kendo-ui/framework/drawing/overview
        e.sender.surface.draw(group);
    }

    return {
        dataSource: _dataSource,
        legend: _legend,
        displayTodayLine: drawTodayLine
    };

})();


EPMO.Charts.displaySCurveChart = function (divId) {

    $('#' + divId).kendoChart({
        dataSource: EPMO.Charts.SCurve.Utils.dataSource,
        legend: EPMO.Charts.SCurve.Utils.legend,
        seriesColors: ["red", "blue", "green"],
        series: [{
            field: "Value",
            type: "line",
            style: "smooth",
            categoryField: "DateValue",
            colorField: "Color",
            noteTextField: "Value",
            notes: {
                label: {
                    position: "outside"
                },
                position: "bottom"
            }
        }],
        categoryAxis: {
            name: "categoryAxis",
            labels: {
                format: "dd MMM",
                rotation: "auto"
            },
            baseUnit: "weeks",
            crosshair: {
                visible: true
            }
        },
        valueAxis: {
            name: "valueAxis",
            labels: {
                format: "N0"
            }
        },
        tooltip: {
            visible: true,
            shared: true,
            format: "{0}",
            template: "#= value # Tasks Completed"
        },
        render: EPMO.Charts.SCurve.Utils.displayTodayLine
    });
};


EPMO.Charts.displaySCurveByActivityChart = function (divId) {

    $('#' + divId).kendoChart({
        dataSource: EPMO.Charts.SCurve.Utils.dataSource,
        legend: EPMO.Charts.SCurve.Utils.legend,
        seriesColors: ["red", "blue", "green"],
        series: [{
            field: "Value",
            type: "line",
            style: "smooth",
            categoryField: "DateValue",
            colorField: "Color",
            noteTextField: "Value",
            notes: {
                label: {
                    position: "outside"
                },
                position: "bottom"
            }
        }],
        categoryAxis: {
            name: "categoryAxis",
            labels: {
                format: "MMM yyyy",
                rotation: "auto"
            },
            baseUnit: "months",
            crosshair: {
                visible: true
            }
        },
        valueAxis: {
            name: "valueAxis",
            labels: {
                format: "N0"
            }
        },
        tooltip: {
            visible: true,
            shared: true,
            format: "{0}",
            template: "#= value # Tasks Completed"
        },
        render: EPMO.Charts.SCurve.Utils.displayTodayLine
    });
};