"use strict";

var EPMO = window.EPMO || {};
EPMO.Charts = EPMO.Charts || {};

EPMO.Charts.displayGanttChart = function(divId){

    var _url = EPMO.Utils.getHost() + "Task/GetGanttChart";
    var tasksDataSource = new kendo.data.GanttDataSource({

    transport: {
        read: {
            url: _url,
            dataType: "jsonp"
        },
        parameterMap: function(options, operation) {
            if (operation !== "read") {
                return { models: kendo.stringify(options.models || [options]) };
            }
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { from: "ID", type: "number" },
                orderId: { from: "OrderID", type: "number", validation: { required: true } },
                parentId: { from: "ParentID", type: "number", defaultValue: null, validation: { required: true } },
                start: { from: "Start", type: "date" },
                end: { from: "End", type: "date" },
                title: { from: "Title", defaultValue: "", type: "string" },
                percentComplete: { from: "PercentComplete", type: "number" },
                summary: { from: "Summary", type: "boolean" },
                expanded: { from: "Expanded", type: "boolean", defaultValue: true }
            }
        }
    }
    });

    var _dependencyUrl = 'https://host.my-epmo.com/Task/GetGanttDependencies';

    var dependenciesDataSource = new kendo.data.GanttDependencyDataSource({
    transport: {
        read: {
            url: _dependencyUrl,
            dataType: "jsonp"
        },
        parameterMap: function(options, operation) {
            if (operation !== "read") {
                return { models: kendo.stringify(options.models || [options]) };
            }
        }
    },
    schema: {
        model: {
            id: "id",
            fields: {
                id: { from: "ID", type: "number" },
                predecessorId: { from: "PredecessorID", type: "number" },
                successorId: { from: "SuccessorID", type: "number" },
                type: { from: "Type", type: "number" }
            }
        }
    }
    });

    var gantt = $("#" + divId).kendoGantt({
    dataSource: tasksDataSource,
    dependencies: dependenciesDataSource,
    views: [
        "day",
        { type: "week", selected: true },
        "month"
    ],
    columns: [
        { field: "id", title: "ID", width: 60 },
        { field: "title", title: "Title", editable: true, sortable: true },
        { field: "start", title: "Start Time", format: "{0:MM/dd/yyyy}", width: 100, editable: true, sortable: true },
        { field: "end", title: "End Time", format: "{0:MM/dd/yyyy}", width: 100, editable: true, sortable: true }
    ],
    height: 700,

    showWorkHours: false,
    showWorkDays: false,

    snap: false
    }).data("kendoGantt");

    $(document).bind("kendo:skinChange", function() {
        gantt.refresh();
    });
}