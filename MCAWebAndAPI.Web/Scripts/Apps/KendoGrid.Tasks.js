var KendoGrid = window.KendoGrid || {};
KendoGrid.Tasks = KendoGrid.Tasks || {};

KendoGrid.Tasks.Load = function (divId) {
    $("#"+divId).kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: "http://localhost:15923/api/Task"
            },
            pageSize: 50
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
}