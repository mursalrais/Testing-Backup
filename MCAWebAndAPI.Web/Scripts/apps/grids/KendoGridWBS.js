"use strict";

var EPMO = window.EPMO || {};
EPMO.Grids = EPMO.Grids || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Grids.displayWBSGrid = function (divId) {

    var _url = EPMO.Utils.getHost() + "ProjectHierarchy/GetAllWBSMappings" + EPMO.Utils.getSiteUrl();

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
                    WBSID: { type: "string" },
                    WBSDescription: { type: "string" },
                    SubActivity: { type: "string" },
                    Activity: { type: "string" },
                    Project: { type: "string" }
                }
            }
        }
    });

    var _columns = [{
        field: "WBSID",
        title: "WBS ID",
        width: 100
    }, {
        field: "WBSDescription",
        title: "WBS Description",
        width: 300
    },  {
        field: "SubActivity",
        title: "Sub Activity"
    },{
        field: "Activity",
        title: "Activity"
    }, {
        field: "Project",
        title: "Project"
    }];

    var grid = $("#" + divId).kendoGrid({
        dataSource: _dataSource,
        columns: _columns
    });

    console.log(grid);
}