"use strict";

var EPMO = window.EPMO || {};
EPMO.Charts = EPMO.Charts || {};
EPMO.Charts.Utils = EPMO.Charts.Utils || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Charts.displayProjectStatusByActivityChart = function (divId) {

    var _url = EPMO.Utils.getHost() +
        "ProjectHierarchy/GetProjectHealthStatusChartByActivity" + EPMO.Utils.getSiteUrl();

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
        seriesDefaults: {
            type: "bar",
            stack: true
        },
        dataSource: _dataSource,
        series: [{
            field: "Value",
            categoryField: "CategoryName",
            aggregate: "sum",
            colorField: "Color",
            visibleInLegend: false
        }],
        valueAxis: {
            max: 20,
            line: {
                visible: true
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
            template: "#= EPMO.Charts.Utils.hideOrder(series.name) #: #= value # Sub Activities"
        }
    });

};