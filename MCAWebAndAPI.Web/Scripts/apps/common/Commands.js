"use strict";

var EPMO = window.EPMO || {};
EPMO.Commands = EPMO.Commands || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Commands.updateWBSMapping = function () {
    var _url = EPMO.Utils.getHost() +
        "ProjectHierarchy/UpdateWBSMapping" + EPMO.Utils.getSiteUrl();

    $.ajax({
        type: 'GET',
        url: _url,
        async: false,
        jsonpCallback: 'callback',
        contentType: "application/json",
        dataType: 'jsonp',
        success: function(json) {
            console.dir(json.sites);
        },
        error: function(e) {
            console.log(e.message);
        }
    });

};