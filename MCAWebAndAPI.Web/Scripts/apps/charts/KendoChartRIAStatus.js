"use strict";

var EPMO = window.EPMO || {};
EPMO.Charts = EPMO.Charts || {};
EPMO.Charts.Utils = EPMO.Charts.Utils || {};

EPMO.Charts.displayRIAStatusChart = function(divId, riaType){

    var _url = EPMO.Utils.getHost() + "RIA/GetRIAStatusChart?riaType=" + riaType
     + "&siteUrl=" + _spPageContextInfo.webAbsoluteUrl;

    var _dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: _url,
                dataType: "jsonp",
                type: "GET"
            }
        }
    });

    var _center, _radius;

    $("#"+divId).kendoChart({
        dataSource: _dataSource,
        legend:{
            visible: false
        },
        series: [{
            type: "donut",
            field: "Value",
            categoryField: "Label",
            colorField: "Color",
            visual: function(e) {
              _center = e.center;
              _radius = e.radius;
              return e.createVisual();
            } 
        }],
        seriesDefaults: {
            labels: {
                template: "# if (percentage > 0) { # #= category # - #= kendo.format('{0:P}', percentage)# # } #",
                position: "insideEnd",
                visible: true,
                background: "transparent"
            }
        },
        tooltip: {
            visible: true,
            template: "${ category } : ${ value }"
        },
        render: function(e){
            EPMO.Charts.Utils.generateTitle(e, "Status", _center, _radius);
        }
    });
};