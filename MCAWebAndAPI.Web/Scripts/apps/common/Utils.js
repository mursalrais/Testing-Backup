"use strict";

var EPMO = window.EPMO || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Utils.getHost = function () {

    // if debug mode
    return "http://localhost:15923/";

    // eCEOs Web Application
    //return "https://host.my-epmo.com/";
}

EPMO.Utils.getSiteUrl = function () {
    // if debug mode
    return "";

    //if dev mode
    //return "?siteUrl='" + _spPageContextInfo.webAbsoluteUrl +"'";
}

EPMO.Utils.onRequestEnd = function (e) {
    if (e.response && e.response.length) {
        var data = e.response;
        if (this.group().length && e.type == "read") {
            EPMO.Utils.handleGroups(data);
        } else {
            //console.log(data);
            EPMO.Utils.loopRecords(data);

        }
    }
};

EPMO.Utils.handleGroups = function (groups) {
    for (var i = 0; i < groups.length; i++) {
        var gr = groups[i];
        EPMO.Utils.offsetDateFields(gr); //handle the Key variable as well
        if (gr.HasSubgroups) {
            EPMO.Utils.handleGroups(gr.Items)
        } else {
            EPMO.Utils.loopRecords(gr.Items);
        }
    }
};

EPMO.Utils.loopRecords = function (persons) {
    for (var i = 0; i < persons.length; i++) {
        var person = persons[i];
        EPMO.Utils.offsetDateFields(person);
    }

};

EPMO.Utils.offsetDateFields = function (obj) {
    var time = 13;
    for (var name in obj) {
        var prop = obj[name];
        if (typeof (prop) === "string" && prop.indexOf("/Date(") == 0) {
            //obj[name] = prop.replace(/\d+/, function (n) {
            //    var offsetMiliseconds = new Date(parseInt(n)).getTimezoneOffset() * 60000;
            //    return parseInt(n) + offsetMiliseconds
            //});
            var actualDate = new Date(parseInt(prop.substr(6)));
            console.log(obj[name]);
            //actualDate = actualDate.setTime(actualDate.getTime() + (time*60*60*1000)); 
            actualDate = actualDate.setTime(actualDate.getTime() + (time * 60 * 60 * 1000));
            obj[name] = new Date(actualDate);
            //console.log(actualDate);
            console.log(obj[name]);
        }
    }
};


EPMO.Utils.displayGroupNameinGroupFooter = function (e) {
    var t = $("#grid .k-grouping-row"); //get all grouping rows
    $.each(t, function (key, elem) {
        if ($(elem).find("td p.k-reset").text().indexOf("SubActivity:") !== -1) {
            $(elem).addClass("sub-activity-grouping-row"); //add a class to the grouping row, so we can find it later
        } else if ($(elem).find("td p.k-reset").text().indexOf("Activity:") !== -1) {
            $(elem).addClass("activity-grouping-row"); //add a class to the grouping row, so we can find it later
        } else if ($(elem).find("td p.k-reset").text().indexOf("Project:") !== -1) {
            $(elem).addClass("project-grouping-row"); //add a class to the grouping row, so we can find it later
        }
    });

    t = $(".k-group-footer"); //get all group footer rows
    $.each(t, function (key, elem) {
        // total
        if ($(elem).prev().attr("class") == "k-group-footer") {
            //year group footer rows are always preceded by a make group footer row
            var prevLabel = $(elem).prevAll(".sub-activity-grouping-row:first").find("td p.k-reset").text().replace("SubActivity: ", "");
            $(elem).find("td:contains('Total:')").text("Total (" + prevLabel + "):"); //update total text to show group value
        }
        //    //make total
        //else { //make group footer rows are never preceded by a group footer row
        //    var prevLabel = $(elem).prevAll(".k-grouping-row:first").find("td p.k-reset").text().replace("Make: ", ""); //traverse backwards to find closest grouping row and get group value
        //    $(elem).find("td:contains('Total:')").text("Total (" + prevLabel + "):")
        //}
    });
}