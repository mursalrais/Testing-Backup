$(document).ready(function () {
    var activities = $("#activity-dropdown").kendoDropDownList({
        optionLabel: "Select Activity...",
        dataTextField: "ActivityName",
        dataValueField: "ActivityName",
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: "https://eceos2.sharepoint.com/sites/mca-dev/_api/lists/getbytitle('activity')/items/",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Accept", "application/json; odata=verbose")
                }
            },
            schema: {
                data: function (data) {
                    return data.d && data.d.results ? data.d.results : [data.d];
                }
            }
        }
    }).data("kendoDropDownList");

    var subactivities = $("#sub-activity-dropdown").kendoDropDownList({
        autoBind: false,
        cascadeFrom: "activities",
        optionLabel: "Select Sub Activity...",
        dataTextField: "SubActivityName",
        dataValueField: "SubActivityName",
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: "http://localhost:15923/api/ProjectHierarchy/GetSubActivities"
            }
        }
    }).data("kendoDropDownList");
});

