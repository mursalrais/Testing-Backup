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
                    ActivityName: { type: "string" },
                    Director: { type: "string" },
                    Start: { type: "date" },
                    Finish: {type: "date" },
                    Color: { from: "ColorStatus", type: "string" },
                    NoofSubActivity: { type: "number" },
                    PercentComplete: { type: "decimal"}
                }
            }
        }
    });




    var _columns = [{
        field: "ActivityName",
        title: "Activity Name",
        width: 320
    },{
        field: "NoofSubActivity",
        title: "SubAct Total",
        width: 120
    },{
        field: "ScheduleStatus",
        title: "Schedule Status",
        width: 100,
        template: '<svg height="30" width="30"><circle cx="15" cy="15" r="10" stroke="black" stroke-width="1" fill="#= Color #" /></svg>'
    },{
        field: "Director",
        title: "Project Director"
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
        field: "PercentComplete",
        title: "%Complete",
        format: "{0:p}",
        width: 100
    }
    ];

    var grid = $("#" + divId).kendoGrid({
        dataSource: _dataSource,
        columns: _columns
    });
    
        console.log(grid);
}