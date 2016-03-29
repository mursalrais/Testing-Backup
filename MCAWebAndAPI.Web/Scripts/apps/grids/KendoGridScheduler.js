"use strict";

var EPMO = window.EPMO || {};
EPMO.Grids = EPMO.Grids || {};

EPMO.Grids.displayCalendarSchedulerGrid = function(divId){
    var _url = EPMO.Utils.getHost() + "Calendar/GetEvents?siteUrl='" + _spPageContextInfo.webAbsoluteUrl + "'";

    $("#" + divId).kendoScheduler({
        date: new Date(),
        startTime: new Date("2016/1/1 07:00 AM"),
        height: 500,
        views: ["day", "month",
            { type: "agenda", selected: true}],
        timezone: "Etc/UTC",
        dataSource: {
            batch: true,
            transport: {
                read: {
                    url: _url,
                    dataType: "jsonp",
                    type: "GET"
                }
            },
            schema: {
                model: {
                    id: "taskId",
                    fields: {
                        taskId: { from: "ID", type: "number" },
                        title: { from: "Title", defaultValue: "No title", validation: { required: true } },
                        start: { type: "date", from: "StartDate" },
                        end: { type: "date", from: "EndDate" },
                        isAllDay: { type: "boolean", from: "IsAllDayEvent" },
                        ownerId : 1
                    }
                }
            }
        },
        resources: [
            {
                field: "ownerId",
                title: "Owner",
                dataSource: [{ text: "All Team", value: 1, color: "#f8a398" }]
            }
        ]
    });

};