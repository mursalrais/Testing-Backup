"use strict";

var EPMO = window.EPMO || {};
EPMO.Utils = EPMO.Utils || {};

EPMO.Utils.getHost = function () {
    
    // if debug mode
    return "http://localhost:15923/";

    // eCEOs Web Application
    // return EPMO.Utils.getHost() + "";
}

EPMO.Utils.getSiteUrl = function () {
    // if debug mode
    return "";

    // if dev mode
    //return "?siteUrl='" + _spPageContextInfo.webAbsoluteUrl +"'";
}