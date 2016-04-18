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

EPMO.Utils.doMath = function (a, b, divId) {
    //var ds = $("#GridBudget").data("kendoGrid").dataSource;
    //var aggregates = ds.aggregates();
    if (a == 0 || b == 0) return 0;
    var percentage = (a / b) * 100;

    return percentage.toFixed(2);
};

EPMO.Utils.isEmpty = function (str) {
    return (!str || 0 === str.length);
};

EPMO.Utils.isEmptyZero = function (str) {
    var strResult = 0;
    if ((EPMO.Utils.isEmpty(str)) == false) {
        strResult = str;
    }
    return strResult;
};

EPMO.Utils.generateMenu = function (navDivId) {

    var menus = [];
    var lis = $("#" + navDivId + " li");
    $.each(lis, function (key, elem) {
        menus.push($(elem).text());
    });

    return menus;
}