"use strict";

var EPMO = window.EPMO || {};
EPMO.Charts = EPMO.Charts || {};

EPMO.Charts.displayProjectPercentageChart = function(divId){

	$("#" + divId).kendoRadialGauge({
			pointer: {
				value: 0
			},
			scale: {
				minorUnit: 5,
				startAngle: -30,
				endAngle: 210,
				max: 100,
				labels: {
					position: "outside"
				},
				ranges: [{
					from: 70,
					to: 100,
					color: "#88C332" 
				},
				{
					from: 50,
					to: 70,
					color: "#ffc700"
				}, {
					from: 30,
					to: 50,
					color: "#ff7a00"
				}, {
					from: 0,
					to: 30,
					color: "#c20000"
				}]
			}
		});

	var _url = _spPageContextInfo.webAbsoluteUrl + 
	"/_api/web/lists/getbytitle('Tasks')/Items?$top=1&$orderby=ID asc" + 
	"&$select=PercentComplete";

	var dataSource = new kendo.data.DataSource({
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
			}
		}
	});

	var result;

	$.when($.ajax(dataSource.fetch(function(){
  		var data = dataSource.data();
  		result = data[0].PercentComplete;
  	}))).then(function () {
		var gauge = $("#" + divId).data("kendoRadialGauge");
		gauge.pointers[0].value(result*100);
	});
};