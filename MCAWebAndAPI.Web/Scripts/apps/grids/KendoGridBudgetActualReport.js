// JavaScript source code
"use strict";

var EPMO = window.EPMO || {};
EPMO.Grids = EPMO.Grids || {};
EPMO.Grids.Utils = EPMO.Grids.Utils || {};

EPMO.Grids.displayBudgetActualGrid = function (divId, strList, strQuarter, strProject, strMain) {

    function getQtrMonth(strQtr, strCheck) {
        var columns = [];
        var strMainTitle1, strMainTitle2, strMainTitle3, strMainTitle4;
        var strField1a, strField2a, strField3a;
        var strField1b, strField2b, strField3b;
        var strField1c, strField2c, strField3c;
        var strField1d, strField2d, strField3d;
        var strTitle1, strTitle2, strTitle3;

        strTitle1 = "Budget"; strTitle2 = "Actual"; strTitle3 = "% Complete";
        strField1a = "Budget1"; strField2a = "Actual1"; strField3a = "Complete1";
        strField1b = "Budget2"; strField2b = "Actual2"; strField3b = "Complete2";
        strField1c = "Budget3"; strField2c = "Actual3"; strField3c = "Complete3";
        strField1d = "Budget4"; strField2d = "Actual4"; strField3d = "Complete4";

        if (EPMO.Utils.isEmpty(strQtr) || strQtr == "Q01") {
            strMainTitle1 = "Apr-13"; strMainTitle2 = "May-13"; strMainTitle3 = "Jun-13"; strMainTitle4 = "QTR 01";
        }
        else if (strQtr == "Q02") {
            strMainTitle1 = "Jul-13"; strMainTitle2 = "Aug-13"; strMainTitle3 = "Sep-13"; strMainTitle4 = "QTR 02";
        }
        else if (strQtr == "Q03") {
            strMainTitle1 = "Oct-13"; strMainTitle2 = "Nov-13"; strMainTitle3 = "Dec-13"; strMainTitle4 = "QTR 03";
        }
        else if (strQtr == "Q04") {
            strMainTitle1 = "Jan-14"; strMainTitle2 = "Feb-14"; strMainTitle3 = "Mar-14"; strMainTitle4 = "QTR 04";
        }
        else if (strQtr == "Q05") {
            strMainTitle1 = "Apr-14"; strMainTitle2 = "May-14"; strMainTitle3 = "Jun-14"; strMainTitle4 = "QTR 05";
        }
        else if (strQtr == "Q06") {
            strMainTitle1 = "Jul-14"; strMainTitle2 = "Aug-14"; strMainTitle3 = "Sep-14"; strMainTitle4 = "QTR 06";
        }
        else if (strQtr == "Q07") {
            strMainTitle1 = "Oct-14"; strMainTitle2 = "Nov-14"; strMainTitle3 = "Dec-14"; strMainTitle4 = "QTR 07";
        }
        else if (strQtr == "Q08") {
            strMainTitle1 = "Jan-15"; strMainTitle2 = "Feb-15"; strMainTitle3 = "Mar-15"; strMainTitle4 = "QTR 08";
        }
        else if (strQtr == "Q09") {
            strMainTitle1 = "Apr-15"; strMainTitle2 = "May-15"; strMainTitle3 = "Jun-15"; strMainTitle4 = "QTR 09";
        }
        else if (strQtr == "Q10") {
            strMainTitle1 = "Jul-15"; strMainTitle2 = "Aug-15"; strMainTitle3 = "Sep-15"; strMainTitle4 = "QTR 10";
        }
        else if (strQtr == "Q11") {
            strMainTitle1 = "Oct-15"; strMainTitle2 = "Nov-15"; strMainTitle3 = "Dec-15"; strMainTitle4 = "QTR 11";
        }
        else if (strQtr == "Q12") {
            strMainTitle1 = "Jan-16"; strMainTitle2 = "Feb-16"; strMainTitle3 = "Mar-16"; strMainTitle4 = "QTR 12";
        }
        else if (strQtr == "Q13") {
            strMainTitle1 = "Apr-16"; strMainTitle2 = "May-16"; strMainTitle3 = "Jun-16"; strMainTitle4 = "QTR 13";
        }
        else if (strQtr == "Q14") {
            strMainTitle1 = "Jul-16"; strMainTitle2 = "Aug-16"; strMainTitle3 = "Sep-16"; strMainTitle4 = "QTR 14";
        }
        else if (strQtr == "Q15") {
            strMainTitle1 = "Oct-16"; strMainTitle2 = "Nov-16"; strMainTitle3 = "Dec-16"; strMainTitle4 = "QTR 15";
        }
        else if (strQtr == "Q16") {
            strMainTitle1 = "Jan-17"; strMainTitle2 = "Feb-17"; strMainTitle3 = "Mar-17"; strMainTitle4 = "QTR 16";
        }
        else if (strQtr == "Q17") {
            strMainTitle1 = "Apr-17"; strMainTitle2 = "May-17"; strMainTitle3 = "Jun-17"; strMainTitle4 = "QTR 17";
        }
        else if (strQtr == "Q18") {
            strMainTitle1 = "Jul-17"; strMainTitle2 = "Aug-17"; strMainTitle3 = "Sep-17"; strMainTitle4 = "QTR 18";
        }
        else if (strQtr == "Q19") {
            strMainTitle1 = "Oct-17"; strMainTitle2 = "Nov-17"; strMainTitle3 = "Dec-17"; strMainTitle4 = "QTR 19";
        }
        else if (strQtr == "Q20") {
            strMainTitle1 = "Jan-18"; strMainTitle2 = "Feb-18"; strMainTitle3 = "Mar-18"; strMainTitle4 = "QTR 20";
        }
        else if (strQtr == "Q21") {
            strMainTitle1 = "Apr-18"; strMainTitle2 = "May-18"; strMainTitle3 = "Jun-18"; strMainTitle4 = "QTR 21";
        }

        if (strCheck == "1") {
            columns.push({ field: strField1a, aggregate: "sum" },
                { field: strField2a, aggregate: "sum" });

            columns.push({ field: strField1b, aggregate: "sum" },
                { field: strField2b, aggregate: "sum" });

            columns.push({ field: strField1c, aggregate: "sum" },
                { field: strField2c, aggregate: "sum" });

            columns.push({ field: strField1d, aggregate: "sum" },
                { field: strField2d, aggregate: "sum" });
        }
        else if (strCheck == "2") {
            columns.push({
                title: strMainTitle1,
                columns: [{ width: "100px", field: strField1a, title: strTitle1, groupFooterTemplate: "#= kendo.toString(sum, 'n0') #", format: "{0:n0}" },
                { width: "100px", field: strField2a, title: strTitle2, groupFooterTemplate: "#= kendo.toString(sum, 'n0') #", format: "{0:n0}" },
                { width: "100px", field: strField3a, title: strTitle3, groupFooterTemplate: " #= EPMO.Utils.doMath(data.Actual1.sum,data.Budget1.sum)  # %" }]
            });

            columns.push({
                title: strMainTitle2,
                columns: [{ width: "100px", field: strField1b, title: strTitle1, groupFooterTemplate: "#= kendo.toString(sum, 'n0') #", format: "{0:n0}" },
                { width: "100px", field: strField2b, title: strTitle2, groupFooterTemplate: "#= kendo.toString(sum, 'n0') #", format: "{0:n0}" },
                { width: "100px", field: strField3b, title: strTitle3, groupFooterTemplate: "  #= EPMO.Utils.doMath(data.Actual2.sum,data.Budget2.sum) # %" }]
            });

            columns.push({
                title: strMainTitle3,
                columns: [{ width: "100px", field: strField1c, title: strTitle1, groupFooterTemplate: "#= kendo.toString(sum, 'n0') #", format: "{0:n0}" },
                { width: "100px", field: strField2c, title: strTitle2, groupFooterTemplate: "#= kendo.toString(sum, 'n0') #", format: "{0:n0}" },
                { width: "100px", field: strField3c, title: strTitle3, groupFooterTemplate: "  #= EPMO.Utils.doMath(data.Actual3.sum,data.Budget3.sum)  # %" }]
            });

            columns.push({
                title: strMainTitle4,
                columns: [{ width: "100px", field: strField1d, title: strTitle1, groupFooterTemplate: "#= kendo.toString(sum, 'n0') #", format: "{0:n0}" },
                { width: "100px", field: strField2d, title: strTitle2, groupFooterTemplate: "#= kendo.toString(sum, 'n0') #", format: "{0:n0}" },
                { width: "100px", field: strField3d, title: strTitle3, groupFooterTemplate: "  #= EPMO.Utils.doMath(data.Actual4.sum,data.Budget4.sum)  # %" }]
            });
        }
        return columns;
    }

    function generateColumns(strQtr) {

        var columns = [{
            width: "100px",
            field: "WBSID",
            title: "WBSID",
        }, {
            width: "240px",
            field: "Title",
            title: "Project/ Activity/ Sub Activity/ Task",
            groupFooterTemplate: "Total:"
        }, {
            width: "120px",
            field: "TotalInitialBudget",
            title: "Total Budget",
            format: "{0:n0}",
            groupFooterTemplate: "#= kendo.toString(sum, 'n0') #"
        }, {
            width: "120px",
            field: "ActualtoDate",
            title: "Actual to Date",
            format: "{0:n0}",
            groupFooterTemplate: "#= kendo.toString(sum, 'n0') #"
        }, {
            width: "120px",
            field: "BalanceBudget",
            title: "Balance Budget",
            format: "{0:n0}",
            groupFooterTemplate: "#= kendo.toString(sum, 'n0') #"
        }, {
            width: "120px",
            field: "PercentComplete",
            title: "% Complete",
            groupFooterTemplate: " #= EPMO.Utils.doMath(data.ActualtoDate.sum,data.TotalInitialBudget.sum)  # %"
        }, {
            width: "120px",
            field: "AverageDisbursementNeededperMont",
            title: "Average Disbursement Needed / Month",
            format: "{0:n0}",
            groupFooterTemplate: "#= kendo.toString(sum, 'n0') #"
        }, {
            field: "Project",
            hidden: true,
            groupHeaderTemplate: "Project: #= value #",
            groupFooterTemplate: "Total Project"
        }, {
            field: "Activity",
            hidden: true,
            groupHeaderTemplate: "Activity: #= value #",
            groupFooterTemplate: "Total Activity"
        }, {
            field: "SubActivity",
            hidden: true,
            groupHeaderTemplate: "SubActivity: #= value #",
            groupFooterTemplate: "Total SubActivity"
        }];

        columns.push.apply(columns, getQtrMonth(strQtr, "2"));
        return columns;
    }

    function mydataSource(strQtr, data) {
        var mydata = [];

        if (data.d.results.length > 0) {
            var strMonth1, strYear1, strMonth2, strYear2, strMonth3, strYear3;
            var myresults = data.d.results;

            if (EPMO.Utils.isEmpty(strQtr) || strQtr == "Q01") {
                strMonth1 = "April"; strYear1 = "2013"; strMonth2 = "May"; strYear2 = "2013"; strMonth3 = "June"; strYear3 = "2013";
            }
            else if (strQtr == "Q02") {
                strMonth1 = "July"; strYear1 = "2013"; strMonth2 = "August"; strYear2 = "2013"; strMonth3 = "September"; strYear3 = "2013";
            }
            else if (strQtr == "Q03") {
                strMonth1 = "October"; strYear1 = "2013"; strMonth2 = "November"; strYear2 = "2013"; strMonth3 = "December"; strYear3 = "2013";
            }
            else if (strQtr == "Q04") {
                strMonth1 = "January"; strYear1 = "2014"; strMonth2 = "February"; strYear2 = "2014"; strMonth3 = "March"; strYear3 = "2014";
            }
            else if (strQtr == "Q05") {
                strMonth1 = "April"; strYear1 = "2014"; strMonth2 = "May"; strYear2 = "2014"; strMonth3 = "June"; strYear3 = "2014";
            }
            else if (strQtr == "Q06") {
                strMonth1 = "July"; strYear1 = "2014"; strMonth2 = "August"; strYear2 = "2014"; strMonth3 = "September"; strYear3 = "2014";
            }
            else if (strQtr == "Q07") {
                strMonth1 = "October"; strYear1 = "2014"; strMonth2 = "November"; strYear2 = "2014"; strMonth3 = "December"; strYear3 = "2014";
            }
            else if (strQtr == "Q08") {
                strMonth1 = "January"; strYear1 = "2015"; strMonth2 = "February"; strYear2 = "2015"; strMonth3 = "March"; strYear3 = "2015";
            }
            else if (strQtr == "Q09") {
                strMonth1 = "April"; strYear1 = "2015"; strMonth2 = "May"; strYear2 = "2015"; strMonth3 = "June"; strYear3 = "2015";
            }
            else if (strQtr == "Q10") {
                strMonth1 = "July"; strYear1 = "2015"; strMonth2 = "August"; strYear2 = "2015"; strMonth3 = "September"; strYear3 = "2015";
            }
            else if (strQtr == "Q11") {
                strMonth1 = "October"; strYear1 = "2015"; strMonth2 = "November"; strYear2 = "2015"; strMonth3 = "December"; strYear3 = "2015";
            }
            else if (strQtr == "Q12") {
                strMonth1 = "January"; strYear1 = "2016"; strMonth2 = "February"; strYear2 = "2016"; strMonth3 = "March"; strYear3 = "2016";
            }
            else if (strQtr == "Q13") {
                strMonth1 = "April"; strYear1 = "2016"; strMonth2 = "May"; strYear2 = "2016"; strMonth3 = "June"; strYear3 = "2016";
            }
            else if (strQtr == "Q14") {
                strMonth1 = "July"; strYear1 = "2016"; strMonth2 = "August"; strYear2 = "2016"; strMonth3 = "September"; strYear3 = "2016";
            }
            else if (strQtr == "Q15") {
                strMonth1 = "October"; strYear1 = "2016"; strMonth2 = "November"; strYear2 = "2016"; strMonth3 = "December"; strYear3 = "2016";
            }
            else if (strQtr == "Q16") {
                strMonth1 = "January"; strYear1 = "2017"; strMonth2 = "February"; strYear2 = "2017"; strMonth3 = "March"; strYear3 = "2017";
            }
            else if (strQtr == "Q17") {
                strMonth1 = "April"; strYear1 = "2017"; strMonth2 = "May"; strYear2 = "2017"; strMonth3 = "June"; strYear3 = "2017";
            }
            else if (strQtr == "Q18") {
                strMonth1 = "July"; strYear1 = "2017"; strMonth2 = "August"; strYear2 = "2017"; strMonth3 = "September"; strYear3 = "2017";
            }
            else if (strQtr == "Q19") {
                strMonth1 = "October"; strYear1 = "2017"; strMonth2 = "November"; strYear2 = "2017"; strMonth3 = "December"; strYear3 = "2017";
            }
            else if (strQtr == "Q20") {
                strMonth1 = "January"; strYear1 = "2018"; strMonth2 = "February"; strYear2 = "2018"; strMonth3 = "March"; strYear3 = "2018";
            }
            else if (strQtr == "Q21") {
                strMonth1 = "April"; strYear1 = "2018"; strMonth2 = "May"; strYear2 = "2018"; strMonth3 = "June"; strYear3 = "2018";
            }

            var tempWBSID = [];

            for (var i = 0; i < myresults.length; i++) {
                var strActivity, strSubActivity;
                var strWBSID, strtitle, strtotalbudget, stractualtodate, strbalancebudget, strpercent, straverage, strProject;
                var strBudget1 = 0, strBudget2 = 0, strBudget3 = 0, strBudget4 = 0;
                var strActual1 = 0, strActual2 = 0, strActual3 = 0, strActual4 = 0;
                var strComplete1 = 0, strComplete2 = 0, strComplete3 = 0, strComplete4 = 0;

                strWBSID = myresults[i].WBSID.WBS_x0020_ID;

                if (tempWBSID.length > 0) {

                    var queryID = Enumerable.From(tempWBSID)
                    .Where("$.WBSID == '" + strWBSID + "'")
                    .Select()
                    .ToArray();

                    if (queryID.length > 0) { continue; }
                }

                tempWBSID.push({ WBSID: strWBSID })

                var queryMonth1 = Enumerable.From(myresults)
                .Where("$.Month == '" + strMonth1 + "' && $.Year == '" + strYear1 + "' && $.WBSID.WBS_x0020_ID == '" + strWBSID + "'")
                .Select()
                .ToArray();

                if (queryMonth1.length > 0) {
                    strBudget1 = queryMonth1[0].Budget; strActual1 = queryMonth1[0].Actual;
                    strComplete1 = kendo.toString((queryMonth1[0].MonthPercentComplete), "p");
                    strBudget4 = EPMO.Utils.isEmptyZero(strBudget1);
                    strActual4 = EPMO.Utils.isEmptyZero(strActual1);

                    if (EPMO.Utils.isEmptyZero(strBudget4) == 0 || EPMO.Utils.isEmptyZero(strActual4) == 0) {
                        strComplete4 = kendo.toString(0, "p");
                    } else {
                        strComplete4 = kendo.toString((EPMO.Utils.isEmptyZero(strActual4) / EPMO.Utils.isEmptyZero(strBudget4)), "p");
                    }

                    strtitle = queryMonth1[0].WBSID.WBS_x0020_Description;
                    strActivity = queryMonth1[0].WBSID.Activity;
                    strSubActivity = queryMonth1[0].WBSID.Sub_x0020_Activity;
                    strtotalbudget = queryMonth1[0].TotalInitialBudget;
                    stractualtodate = queryMonth1[0].ActualtoDate;
                    strbalancebudget = queryMonth1[0].BalanceBudget;
                    strpercent = kendo.toString((queryMonth1[0].PercentComplete), "p");
                    straverage = queryMonth1[0].AverageDisbursementNeededperMont; strProject = queryMonth1[0].WBSID.Project;
                }

                var queryMonth2 = Enumerable.From(myresults)
                .Where("$.Month == '" + strMonth2 + "' && $.Year == '" + strYear2 + "' && $.WBSID.WBS_x0020_ID == '" + strWBSID + "'")
                .Select()
                .ToArray();

                if (queryMonth2.length > 0) {
                    strBudget2 = queryMonth2[0].Budget; strActual2 = queryMonth2[0].Actual;
                    strComplete2 = kendo.toString((queryMonth2[0].MonthPercentComplete), "p");
                    strBudget4 = EPMO.Utils.isEmptyZero(strBudget1) + EPMO.Utils.isEmptyZero(strBudget2);
                    strActual4 = EPMO.Utils.isEmptyZero(strActual1) + EPMO.Utils.isEmptyZero(strActual2);

                    if (EPMO.Utils.isEmptyZero(strBudget4) == 0 || EPMO.Utils.isEmptyZero(strActual4) == 0) {
                        strComplete4 = kendo.toString(0, "p");
                    } else {
                        strComplete4 = kendo.toString(EPMO.Utils.isEmptyZero(strActual4) / (EPMO.Utils.isEmptyZero(strBudget4)), "p");
                    }

                    strtitle = queryMonth2[0].WBSID.WBS_x0020_Description;
                    strActivity = queryMonth2[0].WBSID.Activity;
                    strSubActivity = queryMonth2[0].WBSID.Sub_x0020_Activity;
                    strtotalbudget = queryMonth2[0].TotalInitialBudget;
                    stractualtodate = queryMonth2[0].ActualtoDate;
                    strbalancebudget = queryMonth2[0].BalanceBudget;
                    strpercent = kendo.toString((queryMonth2[0].PercentComplete), "p");
                    straverage = queryMonth2[0].AverageDisbursementNeededperMont;
                    strProject = queryMonth2[0].WBSID.Project;

                }

                var queryMonth3 = Enumerable.From(myresults)
                .Where("$.Month == '" + strMonth3 + "' && $.Year == '" + strYear3 + "' && $.WBSID.WBS_x0020_ID == '" + strWBSID + "'")
                .Select()
                .ToArray();

                if (queryMonth3.length > 0) {
                    strBudget3 = queryMonth3[0].Budget; strActual3 = queryMonth3[0].Actual;
                    strComplete3 = kendo.toString((queryMonth3[0].MonthPercentComplete), "p");
                    strBudget4 = EPMO.Utils.isEmptyZero(strBudget1) + EPMO.Utils.isEmptyZero(strBudget2) + EPMO.Utils.isEmptyZero(queryMonth3[0].Budget);
                    strActual4 = EPMO.Utils.isEmptyZero(strActual1) + EPMO.Utils.isEmptyZero(strActual2) + EPMO.Utils.isEmptyZero(queryMonth3[0].Actual);

                    if (EPMO.Utils.isEmptyZero(strBudget4) == 0 || EPMO.Utils.isEmptyZero(strActual4) == 0) {
                        strComplete4 = kendo.toString(0, "p");
                    } else {
                        strComplete4 = kendo.toString((EPMO.Utils.isEmptyZero(strActual4) / EPMO.Utils.isEmptyZero(strBudget4)), "p");
                    }

                    strtitle = queryMonth3[0].WBSID.WBS_x0020_Description;
                    strActivity = queryMonth3[0].WBSID.Activity;
                    strSubActivity = queryMonth3[0].WBSID.Sub_x0020_Activity;
                    strtotalbudget = queryMonth3[0].TotalInitialBudget;
                    stractualtodate = queryMonth3[0].ActualtoDate;
                    strbalancebudget = queryMonth3[0].BalanceBudget;
                    strpercent = kendo.toString((queryMonth3[0].PercentComplete), "p");
                    straverage = queryMonth3[0].AverageDisbursementNeededperMont;
                    strProject = queryMonth3[0].WBSID.Project;
                }

                mydata.push({
                    Title: strtitle, TotalInitialBudget: strtotalbudget, Activity: strActivity, SubActivity: strSubActivity,
                    WBSID: strWBSID, ActualtoDate: stractualtodate,
                    BalanceBudget: strbalancebudget, PercentComplete: strpercent,
                    AverageDisbursementNeededperMont: straverage, Project: strProject,
                    Budget1: strBudget1, Actual1: strActual1, Complete1: strComplete1,
                    Budget2: strBudget2, Actual2: strActual2, Complete2: strComplete2,
                    Budget3: strBudget3, Actual3: strActual3, Complete3: strComplete3,
                    Budget4: strBudget4, Actual4: strActual4, Complete4: strComplete4
                });
                strBudget1 = "0"; strActual1 = "0"; strComplete1 = "0"; strBudget2 = "0"; strActual2 = "0"; strComplete2 = "0";
                strBudget3 = "0"; strActual3 = "0"; strComplete3 = "0"; strBudget4 = "0"; strActual4 = "0"; strComplete4 = "0";
                strWBSID = ""; strtitle = ""; strtotalbudget = ""; stractualtodate = ""; strbalancebudget = "";
                strpercent = ""; straverage = ""; strProject = "";
            }
            //batas
        }

        var dataSource = new kendo.data.DataSource({
            group: [{
                field: "Project",
                aggregates: getAgregate(strQtr)
            }, {
                field: "Activity",
                aggregates: getAgregate(strQtr)
            }, {
                field: "SubActivity",
                aggregates: getAgregate(strQtr)
            }],
            data: mydata
        });

        return dataSource;
    }

    function getAgregate(strQtr) {

        var columns = [{
            field: "TotalInitialBudget",
            aggregate: "sum"
        }, {
            field: "ActualtoDate",
            aggregate: "sum"
        }, {
            field: "BalanceBudget",
            aggregate: "sum"
        }, {
            field: "AverageDisbursementNeededperMont",
            aggregate: "sum"
        }];

        columns.push.apply(columns, getQtrMonth(strQtr, "1"));
        return columns;
    }

    function getData(_url) {
        var deferred = $.ajax({
            url: _url,
            type: "GET",
            headers: {
                "accept": "application/json;odata=verbose",
            },
            success: function (data) {
                return data;
            },
            error: function (err) {
                console.log(err);
            }
        });

        return deferred.promise()
    };

    function createGrid(divId, strQuarter, data) {
        if (EPMO.Utils.isEmpty(strQuarter) == false) {
            $("#" + divId).empty();
        }

        $("#" + divId).kendoGrid({
            toolbar: ["excel"],
            excel: {
                fileName: "Budget vs Actual Report.xlsx",
                allPages: true
            },
            dataSource: mydataSource(strQuarter, data),
            height: 550,
            width: 1250,
            scrollable: true,
            groupable: false,
            columns: generateColumns(strQuarter)
        });
    }

    var _url;
    var str = _spPageContextInfo.webAbsoluteUrl;
    var intLength = 0;
    var strQuery;
    if (strMain == "no") { intLength = 3 };

    if (EPMO.Utils.isEmpty(strProject) == false) {
        if (EPMO.Utils.isEmpty(strQuarter)) {
            strQuery = "&$filter= ((Quarter eq 'Q01') and (WBSID/Project eq '" + strProject + "'))";
        } else {
            strQuery = "&$filter= ((Quarter eq '" + strQuarter + "') and (WBSID/Project eq '" + strProject + "'))";
            $("#" + divId).empty();
        }

    }
    else {
        if (EPMO.Utils.isEmpty(strQuarter)) {
            strQuery = "&$filter= (Quarter eq 'Q01')";
        } else {
            strQuery = "&$filter= (Quarter eq '" + strQuarter + "')";
            $("#" + divId).empty();
        }
    }

    var tempList = EPMO.Utils.isEmpty(strList) == true ? "Budget%20VS%20Actual%20Commitment" : strList;
    _url = str.substring(0, str.length - intLength) + "/_api/web/lists/getbytitle('"
        + tempList + "')/Items?$select=*,WBSID/WBS_x0020_ID,WBSID/WBS_x0020_Description,WBSID/Activity,WBSID/Sub_x0020_Activity,WBSID/Project&$expand=WBSID&$top=10000&" + strQuery;

    getData(_url).done(
        function (response) {
            createGrid(divId, strQuarter, response);
        }
    );

};

EPMO.Grids.displayDDLType = function (divId) {

    function datatype() {
        var mydata = [{ text: "Commitment", value: "Budget%20VS%20Actual%20Commitment" },
        { text: "Disbursement", value: "Budget%20VS%20Actual%20Disbursement%20MCDR" }];
        return mydata;
    }

    $("#" + divId).kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: datatype()
    });
};


EPMO.Grids.displayDDLQuarter = function (divId) {
    $("#" + divId).kendoDropDownList({
        dataSource: {
            data: ["Q01", "Q02", "Q03", "Q04", "Q05", "Q06", "Q07", "Q08", "Q09", "Q10",
            "Q11", "Q12", "Q13", "Q14", "Q15", "Q16", "Q17", "Q18", "Q19", "Q20", "Q21"]
        },
        closed: onClosed
    });

    function onClosed() {
        var strQuarter = $("#quarter").val();
        var strType = $("#type").val();
        EPMO.Grids.displayGrid("GridBudget", strType, strQuarter);
    }
};

