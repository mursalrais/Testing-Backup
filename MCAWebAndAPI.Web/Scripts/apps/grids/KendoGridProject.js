"use strict";

var EPMO = window.EPMO || {};
EPMO.Grids = EPMO.Grids || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Grids.displayProjectGrid = function (divId) {

    var _url = EPMO.Utils.getHost() + "ProjectHierarchy/GetProjects" + EPMO.Utils.getSiteUrl();

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
        requestEnd: EPMO.Utils.onRequestEnd,
        schema: {
            model: {
                fields: {
                    ProjectName: { from: "ProjectName", type: "string" },
                    Director: { from: "Director", type: "string" },
                    Start: { from: "Start", type: "date" },
                    Finish: { from: "Finish", type: "date" },
                    PercentComplete: { from: "PercentComplete", type: "decimal" },
                    Color: { from: "ColorStatus", type: "string" }
                }
            }
        }
    });


    var _columns = [{
        field: "ProjectName",
        title: "Project Name",
        width: 320
    }, {
        field: "ScheduleStatus",
        title: "Status",
        width: 100,
        template: '<svg height="30" width="30"><circle cx="15" cy="15" r="10" stroke="black" stroke-width="1" fill="#= Color #" /></svg>'
    }, {
        field: "Director",
        title: "Project Director"
    }, {
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

    $("#" + divId).kendoGrid({
        dataSource: _dataSource,
        columns: _columns
    });
}