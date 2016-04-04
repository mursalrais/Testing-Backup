"use strict";

var EPMO = window.EPMO || {};
EPMO.Grids = EPMO.Grids || {};
EPMO.Charts.Utils = EPMO.Charts.Utils || {};

EPMO.Grids.displayTaskWithSingleResourceGrid = function (divId) {

    var _url = EPMO.Utils.getHost() + "Task/GetTasksWithSingleResource" + EPMO.Utils.getSiteUrl();

    var _dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: _url,
                dataType: "jsonp",
                type: "GET"
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
                    Title: { type: "string" },
                    AssignedTo: { type: "string" },
                    StartDate: { type: "date" },
                    DueDate: { type: "date" },
                    PercentComplete: { type: "decimal" }
                }
            }
        },
        group: {
            field: "AssignedTo", aggregates: [
               { field: "Title", aggregate: "count" },
               { field: "AssignedTo", aggregate: "count" }
            ]
        },
        aggregate: [
           { field: "Title", aggregate: "count" }
        ]
    });

    $("#" + divId).kendoGrid({
        dataSource: _dataSource,
        height: 550,
        columns: [{
            field: "Title",
            title: "Task Name",
            aggregates: ["count"],
            width: 400
        }, {
            field: "StartDate",
            title: "Start Date",
            template: '#= kendo.toString(StartDate, "D") #'
        }, {
            field: "DueDate",
            title: "Finish Date",
            template: '#= kendo.toString(DueDate, "D") #'
        }, {
            field: "PercentComplete",
            title: "Completion",
            format: "{0:p}",
            width: 100
        }, {
            field: "AssignedTo",
            title: "Assigned To",
            aggregates: ["count", "average"],
            groupHeaderTemplate: "#= value # #= count# Task(s) Assigned"
        }]
    });
};