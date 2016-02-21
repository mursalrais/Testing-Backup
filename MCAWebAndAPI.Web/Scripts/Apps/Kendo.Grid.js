$(document).ready(function () {
    $("#kendo-grid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: "http://localhost:15923/api/Task"
            },
            pageSize: 50,
            group:
                [ { field: "Activity", dir: "asc" }, { field: "SubActivity", dir: "asc" } ] 
		},
		height: 550,
		sortable: true,
		pageable: {
			refresh: true,
			pageSizes: true,
			buttonCount: 5,
		},
		columns: [{
			field: "Title",
			title: "Task Name",
			width: 400
		}, {
		    field: "StartDateString",
			title: "Start Date"
		}, {
			field: "DueDateString",
			title: "Finish Date"
		}, {
			field: "PercentageString",
			title: "Completion",
			width: 150
		}]
	});
});