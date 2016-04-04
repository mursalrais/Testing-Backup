"use strict";

var EPMO = window.EPMO || {};
EPMO.Charts = EPMO.Charts || {};
EPMO.Charts.Utils = EPMO.Charts.Utils || {};

EPMO.Charts.displayTaskResourceChart = function (divId, riaType) {

    var _url = EPMO.Utils.getHost() + "Task/GetTaskByResourceChart" + EPMO.Utils.getSiteUrl();

    var _legend = EPMO.Charts.Utils.generateLegend();
    _legend.position = "bottom";

    var _dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: _url,
                dataType: "jsonp",
                type: "GET"
            }
        }
    });

    $("#" + divId).kendoChart({
        legend: _legend,
        seriesDefaults: {
            type: "bar"
        },
        dataSource: _dataSource,
        series: [{
            field: "Value",
            categoryField: "CategoryName",
            aggregate: "sum"
        }],
        valueAxis: {
            line: {
                visible: false
            },
            minorGridLines: {
                visible: true
            }
        },
        categoryAxis: {
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            template: " #= value # tasks assigned"
        }
    });
};