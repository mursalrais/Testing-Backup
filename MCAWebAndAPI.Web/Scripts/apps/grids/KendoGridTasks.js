"use strict";

var EPMO = window.EPMO || {};
EPMO.Grids = EPMO.Grids || {};

EPMO.Grids.displayTaskGrid = function(divId){

	var _url = _spPageContextInfo.webAbsoluteUrl +
	"/_api/web/lists/getbytitle('Tasks')/Items?$"+ 
	"select=Title,PercentComplete,StartDate,DueDate";
	
	$("#" + divId).kendoGrid({
		dataSource: {
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
						StartDate: { type: "date" },
						DueDate: { type: "date" },
						PercentComplete: {type: "decimal"}                              
					}
				}
			}
	},
	height: 550,
	sortable: true,
	pageable: {
		refresh: true,
		pageSizes: true,
		buttonCount: 5,
	},
	filterable: true,
	columns: [{
		field: "Title",
		title: "Task Name",
		width: 300
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
	}]
}); 

};