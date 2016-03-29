"use strict";

var EPMO = window.EPMO || {};
EPMO.Charts = EPMO.Charts || {};
EPMO.Charts.Utils = EPMO.Charts.Utils || {};

EPMO.Charts.displayRIAChart = function (divId) {

    var _url = EPMO.Utils.getHost() + "RIA/GetOverallRiaChart?siteUrl='" + _spPageContextInfo.webAbsoluteUrl + "'";
    var _legend = EPMO.Charts.Utils.generateLegend();
    var _dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: _url,
                dataType: "jsonp",
                type: "GET"
            }
        },
        group: {
            field: "GroupName"
        }
    });

    $("#" + divId).kendoChart({
        legend: _legend,
        seriesDefaults: {
            type: "bar",
            stack: {
                type: "100%"
            }
        },
        dataSource: _dataSource,
        series: [{
            field: "Value",
            categoryField: "CategoryName",
            colorField: "Color"
        }],
        dataBound: onDataBound,
        valueAxis: {
            line: {
                visible: false
            },
            minorGridLines: {
                visible: true
            }
        },
        categoryAxis: {
            categories: ["Risk", "Issue", "Action"],
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            template: "#= series.name #: #= value #"
        }
    });

    function onDataBound() {
        var _chart = $("#" + divId).data("kendoChart");
        _chart.options.series[0].color = _chart.options.series[0].data[0].color;
    }
};
