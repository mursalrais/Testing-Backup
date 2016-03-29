var EPMO = window.EPMO || {};
EPMO.Charts = EPMO.Charts || {};

EPMO.Charts.displayDonutsChart = function(divId, args){

    function createDonutsData(arg1, arg2){
        var result = [];

        for(var i = 0; i < createDonutsData.arguments.length; i++){
            var temp = {
                name: createDonutsData.arguments[i], 
                value: createDonutsData.arguments[++i]
            };
            result.push(temp);
        }
        return result;
    }

    var chartData = createDonutsData.apply(this, args);
    $("#"+divId).kendoChart({
        legend: {
            position: "bottom"
        },
        dataSource: {
            data: chartData
        },
        series: [{
            type: "donut",
            field: "value",
            categoryField: "name"
        }],
        seriesColors: ["#40d47e", "#e74c3c"],
        tooltip: {
            visible: true,
            template: "${ category } : ${ value }"
        }
    });
};