"use strict";

var EPMO = window.EPMO || {};
EPMO.Grids = EPMO.Grids || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Grids.displayActivityGrid = function (divId) {

    var _url = _spPageContextInfo.webAbsoluteUrl +
	"/_api/web/lists/getbytitle('Activity')/Items?$" +
	"select=Title,Start,Finish,ScheduleStatus";

    var _dataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: _url,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Accept", "application/json; odata=verbose");
                }
            }
        },
        schema: {
            data: function (data) {
                return data.d && data.d.results ? data.d.results : [data.d];
            },
            model: {
                fields: {
                    Title: { type: "string" },
                    Start: { type: "date" },
                    Finish: { type: "date" },
                    ScheduleStatus: { type: "string" }
                }
            }
        }
    });

    var _columns = [{
        field: "Title",
        title: "Activity Name",
        width: 300
    }, {
        field: "Start",
        title: "Start Date",
        template: '#= kendo.toString(StartDate, "D") #'
    }, {
        field: "Finish",
        title: "Finish Date",
        template: '#= kendo.toString(DueDate, "D") #'
    }, {
        field: "ScheduleStatus",
        title: "Schedule Status",
        width: 100
    }];

    $("#" + divId).kendoGrid({
        dataSource: _dataSource,
        height: 550,
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