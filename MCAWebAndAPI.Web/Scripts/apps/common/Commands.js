"use strict";

var EPMO = window.EPMO || {};
EPMO.Commands = EPMO.Commands || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Commands.updateWBSMapping = function () {
    var _url = EPMO.Utils.getHost() +
        "ProjectHierarchy/UpdateWBSMapping" + EPMO.Utils.getSiteUrl();

    $.getJSON(_url, function (result) {
        if (result){
            alert("WBS Mapping has been sucessfully updated");
        }
        else {
            alert("There is no change in WBS Mapping in project sites");
        }
    })
};