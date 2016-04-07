"use strict";

var EPMO = window.EPMO || {};
EPMO.Charts = EPMO.Charts || {};
EPMO.Charts.Utils = EPMO.Charts.Utils || {};

EPMO.Charts.displayRIAResourceChart = function(divId, riaType){

    var _url = EPMO.Utils.getHost() + "RIA/GetRIAResourceChart?riaType=" + riaType
     + "&siteUrl=" + _spPageContextInfo.webAbsoluteUrl;

    var _legend = EPMO.Charts.Utils.generateLegend();
    _legend.position = "bottom";

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
            stack: true
        },
        dataSource: _dataSource,
        series: [{
            field: "Value",
            categoryField: "CategoryName",
            colorField: "Color", 
            aggregate: "sum"
        }],
        dataBound: onDataBound,
        valueAxis: {
        	max: 10,
            minorGridLines: {
                visible: false
            }
        },
        categoryAxis: {
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            template: "#= series.name #: #= value #"
        }
    });

    function onDataBound(){
        var _chart = $("#" + divId).data("kendoChart");
        _chart.options.series[0].color = _chart.options.series[0].data[0].color;
    }
};