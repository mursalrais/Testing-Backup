"use strict";

var EPMO = window.EPMO || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Utils.getHost = function () {
    
    // if debug mode
    //return "http://localhost:15923/";

    // eCEOs Web Application
    return "https://host.my-epmo.com/";
}

EPMO.Utils.getSiteUrl = function () {
    // if debug mode
    //return "";

    // if dev mode
    return "?siteUrl='" + _spPageContextInfo.webAbsoluteUrl +"'";
}

EPMO.Utils.onRequestEnd = function(e) {
        if (e.response.Data && e.response.Data.length) {
            var data = e.response.Data;
            if (this.group().length && e.type == "read") {
                EPMO.Utils.handleGroups(data);
            } else {
                loopRecords(data);
            }
        }
    };

EPMO.Utils.handleGroups = function(groups) {
        for (var i = 0; i < groups.length; i++) {
            var gr = groups[i];
            offsetDateFields(gr); //handle the Key variable as well
            if (gr.HasSubgroups) {
                handleGroups(gr.Items)
            } else {
                loopRecords(gr.Items);
            }
        }
    };

EPMO.Utils.loopRecords = function(persons) {
        for (var i = 0; i < persons.length; i++) {
            var person = persons[i];
            offsetDateFields(person);
        }
    };

EPMO.Utils.offsetDateFields = function(obj) {
        for (var name in obj) {
            var prop = obj[name];
            if (typeof (prop) === "string" && prop.indexOf("/Date(") == 0) {
                obj[name] = prop.replace(/\d+/, function (n) {
                    var offsetMiliseconds = new Date(parseInt(n)).getTimezoneOffset() * 60000;
                    return parseInt(n) + offsetMiliseconds
                });
            }
        }
    };