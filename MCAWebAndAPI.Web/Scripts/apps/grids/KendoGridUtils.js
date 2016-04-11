"use strict";

var EPMO = window.EPMO || {};
EPMO.Common = EPMO.Common || {};
EPMO.Common.DataStructures = EPMO.Common.DataStructures || {};
EPMO.Grids = EPMO.Grids || {};
EPMO.Grids.Utils = EPMO.Grids.Utils || {};

EPMO.Grids.Utils.displayGroupNameinGroupFooter = function (divId) {

    var tr = $("#" + divId + " .k-grouping-row, #" + divId + " .k-group-footer");
    var stack = [];

    $.each(tr, function (key, elem) {

        if ($(elem).hasClass("k-grouping-row")) {
            var groupName = $(elem).find("td p.k-reset").text();
            stack.push(groupName);
        } else {
            var groupName = stack.pop();
            var groupFooter = $(elem).find("td:contains('Total:')");
            groupFooter.text("Total (" + groupName + "):");
        }
    });

}