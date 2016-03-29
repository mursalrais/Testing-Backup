"use strict";

var EPMO = window.EPMO || {};
EPMO.Grids = EPMO.Grids || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Grids.displayActivityGrid = function (divId) {

    var _url = EPMO.Utils.getHost() + "ProjectHierarchy/GetActivities" + EPMO.Utils.getSiteUrl();

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
                    ActivityName: { from: "ActivityName", type: "string" },
                    Director: { from: "Director", type: "string" },
                    Start: { from: "Start", type: "date" },
                    Finish: { from: "Finish", type: "date" },
                    Color: { from: "ColorStatus", type: "string"}
                }
            }
        }
    });



    var _columns = [{
        field: "ActivityName",
        title: "Activity Name",
        width: 320
    },
    {
        field: "Director",
        title: "Director Name"
    },
    {
        field: "Start",
        title: "Start Date",
        template: '#= kendo.toString(Start, "D") #'
    }, {
        field: "Finish",
        title: "Finish Date",
        template: '#= kendo.toString(Finish, "D") #'
    }, {
        field: "ScheduleStatus",
        title: "Status",
        width: 150,
        template: '<svg height="50" width="50"><circle cx="25" cy="25" r="20" stroke="black" stroke-width="1" fill="#= Color #" /></svg>'
    }
    ];

    $("#" + divId).kendoGrid({
        dataSource: _dataSource,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: true,
            buttonCount: 5,
        },
        filterable: true,
        columns: _columns
    });
}