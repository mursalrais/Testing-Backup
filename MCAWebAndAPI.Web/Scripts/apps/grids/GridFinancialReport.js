"use strict";

var EPMO = window.EPMO || {};
EPMO.Grids = EPMO.Grids || {};

var tempList;

function isEmpty(str) {
    return (!str || 0 === str.length);
}

function getIntLoop(strQtr) {
    var intLoop = 0;
    if (isEmpty(strQtr) || strQtr == "Q01") {
        intLoop = 1;
    }
    else if (strQtr == "Q02") {
        intLoop = 2;
    }
    else if (strQtr == "Q03") {
        intLoop = 3;
    }
    else if (strQtr == "Q04") {
        intLoop = 4;
    }
    else if (strQtr == "Q05") {
        intLoop = 5;
    }
    else if (strQtr == "Q06") {
        intLoop = 6;
    }
    else if (strQtr == "Q07") {
        intLoop = 7;
    }
    else if (strQtr == "Q08") {
        intLoop = 8;
    }
    else if (strQtr == "Q09") {
        intLoop = 9;
    }
    else if (strQtr == "Q10") {
        intLoop = 10;
    }
    else if (strQtr == "Q11") {
        intLoop = 11;
    }
    else if (strQtr == "Q12") {
        intLoop = 12;
    }
    else if (strQtr == "Q13") {
        intLoop = 13;
    }
    else if (strQtr == "Q14") {
        intLoop = 14;
    }
    else if (strQtr == "Q15") {
        intLoop = 15;
    }
    else if (strQtr == "Q16") {
        intLoop = 16;
    }
    else if (strQtr == "Q17") {
        intLoop = 17;
    }
    else if (strQtr == "Q18") {
        intLoop = 18;
    }
    else if (strQtr == "Q19") {
        intLoop = 19;
    }
    else if (strQtr == "Q20") {
        intLoop = 20;
    }
    else if (strQtr == "Q21") {
        intLoop = 21;
    }
    return intLoop
}

function getCurrentPeriod(strQtr, strCheck) {
    var columns = [];
    var strField1, strField2, strField3, strField4;
    var strTitle1, strTitle2, strTitle3, strTitle4;


    if (isEmpty(strQtr) || strQtr == "Q01" || strQtr == "Q02") {
        strField1 = "OData__x0041_pr13"; strField2 = "OData__x004d_ay13"; strField3 = "OData__x004a_un13"; strField4 = "Q01Target";
        strTitle1 = "Apr-13"; strTitle2 = "May-13"; strTitle3 = "Jun-13"; strTitle4 = "QTR01";

    }
    else if (strQtr == "Q03") {
        strField1 = "OData__x004a_ul13"; strField2 = "OData__x0041_ug13"; strField3 = "OData__x0053_ep13"; strField4 = "Q02Target";
        strTitle1 = "Jul-13"; strTitle2 = "Aug-13"; strTitle3 = "Sep-13"; strTitle4 = "QTR02";
    }
    else if (strQtr == "Q04") {
        strField1 = "OData__x004f_ct13"; strField2 = "OData__x004e_ov13"; strField3 = "OData__x0044_ec13"; strField4 = "Q03Target";
        strTitle1 = "Oct-13"; strTitle2 = "Nov-13"; strTitle3 = "Dec-13"; strTitle4 = "QTR03";
    }
    else if (strQtr == "Q05") {
        strField1 = "OData__x004a_an14"; strField2 = "OData__x0046_eb14"; strField3 = "OData__x004d_ar14"; strField4 = "Q04Target";
        strTitle1 = "Jan-14"; strTitle2 = "Feb-14"; strTitle3 = "Mar-14"; strTitle4 = "QTR04";
    }
    else if (strQtr == "Q06") {
        strField1 = "OData__x0041_pr14"; strField2 = "OData__x004d_ay14"; strField3 = "OData__x004a_un14"; strField4 = "Q05Target";
        strTitle1 = "Apr-14"; strTitle2 = "May-14"; strTitle3 = "Jun-14"; strTitle4 = "QTR05";
    }
    else if (strQtr == "Q07") {
        strField1 = "OData__x004a_ul14"; strField2 = "OData__x0041_ug14"; strField3 = "OData__x0053_ep14"; strField4 = "Q06Target";
        strTitle1 = "Jul-14"; strTitle2 = "Aug-14"; strTitle3 = "Sep-14"; strTitle4 = "QTR06";
    }
    else if (strQtr == "Q08") {
        strField1 = "OData__x004f_ct14"; strField2 = "OData__x004e_ov14"; strField3 = "OData__x0044_ec14"; strField4 = "Q07Target";
        strTitle1 = "Oct-14"; strTitle2 = "Nov-14"; strTitle3 = "Dec-14"; strTitle4 = "QTR07";
    }
    else if (strQtr == "Q09") {
        strField1 = "OData__x004a_an15"; strField2 = "OData__x0046_eb15"; strField3 = "OData__x004d_ar15"; strField4 = "Q08Target";
        strTitle1 = "Jan-15"; strTitle2 = "Feb-15"; strTitle3 = "Mar-15"; strTitle4 = "QTR08";
    }
    else if (strQtr == "Q10") {
        strField1 = "OData__x0041_pr15"; strField2 = "OData__x004d_ay15"; strField3 = "OData__x004a_un15"; strField4 = "Q09Target";
        strTitle1 = "Apr-15"; strTitle2 = "May-15"; strTitle3 = "Jun-15"; strTitle4 = "QTR09";
    }
    else if (strQtr == "Q11") {
        strField1 = "OData__x004a_ul15"; strField2 = "OData__x0041_ug15"; strField3 = "OData__x0053_ep15"; strField4 = "Q10Target";
        strTitle1 = "Jul-15"; strTitle2 = "Aug-15"; strTitle3 = "Sep-15"; strTitle4 = "QTR10";
    }
    else if (strQtr == "Q12") {
        strField1 = "OData__x004f_ct15"; strField2 = "OData__x004e_ov15"; strField3 = "OData__x0044_ec15"; strField4 = "Q11Target";
        strTitle1 = "Oct-15"; strTitle2 = "Nov-15"; strTitle3 = "Dec-15"; strTitle4 = "QTR11";
    }
    else if (strQtr == "Q13") {
        strField1 = "OData__x004a_an16"; strField2 = "OData__x0046_eb16"; strField3 = "OData__x004d_ar16"; strField4 = "Q12Target";
        strTitle1 = "Jan-16"; strTitle2 = "Feb-16"; strTitle3 = "Mar-16"; strTitle4 = "QTR12";
    }
    else if (strQtr == "Q14") {
        strField1 = "OData__x0041_pr16"; strField2 = "OData__x004d_ay16"; strField3 = "OData__x004a_un16"; strField4 = "Q13Target";
        strTitle1 = "Apr-16"; strTitle2 = "May-16"; strTitle3 = "Jun-16"; strTitle4 = "QTR13";
    }
    else if (strQtr == "Q15") {
        strField1 = "OData__x004a_ul16"; strField2 = "OData__x0041_ug16"; strField3 = "OData__x0053_ep16"; strField4 = "Q14Target";
        strTitle1 = "Jul-16"; strTitle2 = "Aug-16"; strTitle3 = "Sep-16"; strTitle4 = "QTR14";
    }
    else if (strQtr == "Q16") {
        strField1 = "OData__x004f_ct16"; strField2 = "OData__x004e_ov16"; strField3 = "OData__x0044_ec16"; strField4 = "Q15Target";
        strTitle1 = "Oct-16"; strTitle2 = "Nov-16"; strTitle3 = "Dec-16"; strTitle4 = "QTR15";
    }
    else if (strQtr == "Q17") {
        strField1 = "OData__x004a_an17"; strField2 = "OData__x0046_eb17"; strField3 = "OData__x004d_ar17"; strField4 = "Q16Target";
        strTitle1 = "Jan-17"; strTitle2 = "Feb-17"; strTitle3 = "Mar-17"; strTitle4 = "QTR16";
    }
    else if (strQtr == "Q18") {
        strField1 = "OData__x0041_pr17"; strField2 = "OData__x004d_ay17"; strField3 = "OData__x004d_un17"; strField4 = "Q17Target";
        strTitle1 = "Apr-17"; strTitle2 = "May-17"; strTitle3 = "Jun-17"; strTitle4 = "QTR17";
    }
    else if (strQtr == "Q19") {
        strField1 = "OData__x004a_ul17"; strField2 = "OData__x0041_ug17"; strField3 = "OData__x0053_ep17"; strField4 = "Q18Target";
        strTitle1 = "Jul-17"; strTitle2 = "Aug-17"; strTitle3 = "Sep-17"; strTitle4 = "QTR18";
    }
    else if (strQtr == "Q20") {
        strField1 = "OData__x004f_ct17"; strField2 = "OData__x004e_ov17"; strField3 = "OData__x0044_ec17"; strField4 = "Q19Target";
        strTitle1 = "Oct-17"; strTitle2 = "Nov-17"; strTitle3 = "Dec-17"; strTitle4 = "QTR19";
    }
    else if (strQtr == "Q21") {
        strField1 = "OData__x004a_an18"; strField2 = "OData__x0046_eb18"; strField3 = "OData__x004a_un18"; strField4 = "Q20Target";
        strTitle1 = "Jan-18"; strTitle2 = "Feb-18"; strTitle3 = "Mar-18"; strTitle4 = "QTR20";
    }



    if (strCheck == "1") {
        columns.push({ field: strField1, aggregate: "sum" },
            { field: strField2, aggregate: "sum" }, { field: strField3, aggregate: "sum" }, { field: strField4, aggregate: "sum" })
    } else if (strCheck == "2") {
        columns.push({
            title: "Current Period",
            columns: [
                { width: "100px", field: strField1, title: strTitle1, groupFooterTemplate: "#=sum#" },
                { width: "100px", field: strField2, title: strTitle2, groupFooterTemplate: "#=sum#" },
                { width: "100px", field: strField3, title: strTitle3, groupFooterTemplate: "#=sum#" },
                { width: "100px", field: strField4, title: strTitle4, groupFooterTemplate: "#=sum#" }
            ]
        });
    }


    return columns;
}

function getNextPeriod(strQtr, strCheck) {
    var columns = [];
    var strField1, strField2, strField3, strField4;
    var strTitle1, strTitle2, strTitle3, strTitle4;

    if (isEmpty(strQtr) || strQtr == "Q01") {
        strField1 = "OData__x0041_pr13"; strField2 = "OData__x004d_ay13"; strField3 = "OData__x004a_un13"; strField4 = "Q01Target";
        strTitle1 = "Apr-13"; strTitle2 = "May-13"; strTitle3 = "Jun-13"; strTitle4 = "QTR01";
    }
    else if (strQtr == "Q02") {
        strField1 = "OData__x004a_ul13"; strField2 = "OData__x0041_ug13"; strField3 = "OData__x0053_ep13"; strField4 = "Q02Target";
        strTitle1 = "Jul-13"; strTitle2 = "Aug-13"; strTitle3 = "Sep-13"; strTitle4 = "QTR02";
    }
    else if (strQtr == "Q03") {
        strField1 = "OData__x004f_ct13"; strField2 = "OData__x004e_ov13"; strField3 = "OData__x0044_ec13"; strField4 = "Q03Target";
        strTitle1 = "Oct-13"; strTitle2 = "Nov-13"; strTitle3 = "Dec-13"; strTitle4 = "QTR03";
    }
    else if (strQtr == "Q04") {
        strField1 = "OData__x004a_an14"; strField2 = "OData__x0046_eb14"; strField3 = "OData__x004d_ar14"; strField4 = "Q04Target";
        strTitle1 = "Jan-14"; strTitle2 = "Feb-14"; strTitle3 = "Mar-14"; strTitle4 = "QTR04";
    }
    else if (strQtr == "Q05") {
        strField1 = "OData__x0041_pr14"; strField2 = "OData__x004d_ay14"; strField3 = "OData__x004a_un14"; strField4 = "Q05Target";
        strTitle1 = "Apr-14"; strTitle2 = "May-14"; strTitle3 = "Jun-14"; strTitle4 = "QTR05";
    }
    else if (strQtr == "Q06") {
        strField1 = "OData__x004a_ul14"; strField2 = "OData__x0041_ug14"; strField3 = "OData__x0053_ep14"; strField4 = "Q06Target";
        strTitle1 = "Jul-14"; strTitle2 = "Aug-14"; strTitle3 = "Sep-14"; strTitle4 = "QTR06";
    }
    else if (strQtr == "Q07") {
        strField1 = "OData__x004f_ct14"; strField2 = "OData__x004e_ov14"; strField3 = "OData__x0044_ec14"; strField4 = "Q07Target";
        strTitle1 = "Oct-14"; strTitle2 = "Nov-14"; strTitle3 = "Dec-14"; strTitle4 = "QTR07";
    }
    else if (strQtr == "Q08") {
        strField1 = "OData__x004a_an15"; strField2 = "OData__x0046_eb15"; strField3 = "OData__x004d_ar15"; strField4 = "Q08Target";
        strTitle1 = "Jan-15"; strTitle2 = "Feb-15"; strTitle3 = "Mar-15"; strTitle4 = "QTR08";
    }
    else if (strQtr == "Q09") {
        strField1 = "OData__x0041_pr15"; strField2 = "OData__x004d_ay15"; strField3 = "OData__x004a_un15"; strField4 = "Q09Target";
        strTitle1 = "Apr-15"; strTitle2 = "May-15"; strTitle3 = "Jun-15"; strTitle4 = "QTR09";
    }
    else if (strQtr == "Q10") {
        strField1 = "OData__x004a_ul15"; strField2 = "OData__x0041_ug15"; strField3 = "OData__x0053_ep15"; strField4 = "Q10Target";
        strTitle1 = "Jul-15"; strTitle2 = "Aug-15"; strTitle3 = "Sep-15"; strTitle4 = "QTR10";
    }
    else if (strQtr == "Q11") {
        strField1 = "OData__x004f_ct15"; strField2 = "OData__x004e_ov15"; strField3 = "OData__x0044_ec15"; strField4 = "Q11Target";
        strTitle1 = "Oct-15"; strTitle2 = "Nov-15"; strTitle3 = "Dec-15"; strTitle4 = "QTR11";
    }
    else if (strQtr == "Q12") {
        strField1 = "OData__x004a_an16"; strField2 = "OData__x0046_eb16"; strField3 = "OData__x004d_ar16"; strField4 = "Q12Target";
        strTitle1 = "Jan-16"; strTitle2 = "Feb-16"; strTitle3 = "Mar-16"; strTitle4 = "QTR12";
    }
    else if (strQtr == "Q13") {
        strField1 = "OData__x0041_pr16"; strField2 = "OData__x004d_ay16"; strField3 = "OData__x004a_un16"; strField4 = "Q13Target";
        strTitle1 = "Apr-16"; strTitle2 = "May-16"; strTitle3 = "Jun-16"; strTitle4 = "QTR13";
    }
    else if (strQtr == "Q14") {
        strField1 = "OData__x004a_ul16"; strField2 = "OData__x0041_ug16"; strField3 = "OData__x0053_ep16"; strField4 = "Q14Target";
        strTitle1 = "Jul-16"; strTitle2 = "Aug-16"; strTitle3 = "Sep-16"; strTitle4 = "QTR14";
    }
    else if (strQtr == "Q15") {
        strField1 = "OData__x004f_ct16"; strField2 = "OData__x004e_ov16"; strField3 = "OData__x0044_ec16"; strField4 = "Q15Target";
        strTitle1 = "Oct-16"; strTitle2 = "Nov-16"; strTitle3 = "Dec-16"; strTitle4 = "QTR15";
    }
    else if (strQtr == "Q16") {
        strField1 = "OData__x004a_an17"; strField2 = "OData__x0046_eb17"; strField3 = "OData__x004d_ar17"; strField4 = "Q16Target";
        strTitle1 = "Jan-17"; strTitle2 = "Feb-17"; strTitle3 = "Mar-17"; strTitle4 = "QTR16";
    }
    else if (strQtr == "Q17") {
        strField1 = "OData__x0041_pr17"; strField2 = "OData__x004d_ay17"; strField3 = "OData__x004d_un17"; strField4 = "Q17Target";
        strTitle1 = "Apr-17"; strTitle2 = "May-17"; strTitle3 = "Jun-17"; strTitle4 = "QTR17";
    }
    else if (strQtr == "Q18") {
        strField1 = "OData__x004a_ul17"; strField2 = "OData__x0041_ug17"; strField3 = "OData__x0053_ep17"; strField4 = "Q18Target";
        strTitle1 = "Jul-17"; strTitle2 = "Aug-17"; strTitle3 = "Sep-17"; strTitle4 = "QTR18";
    }
    else if (strQtr == "Q19") {
        strField1 = "OData__x004f_ct17"; strField2 = "OData__x004e_ov17"; strField3 = "OData__x0044_ec17"; strField4 = "Q19Target";
        strTitle1 = "Oct-17"; strTitle2 = "Nov-17"; strTitle3 = "Dec-17"; strTitle4 = "QTR19";
    }
    else if (strQtr == "Q20") {
        strField1 = "OData__x004a_an18"; strField2 = "OData__x0046_eb18"; strField3 = "OData__x004a_un18"; strField4 = "Q20Target";
        strTitle1 = "Jan-18"; strTitle2 = "Feb-18"; strTitle3 = "Mar-18"; strTitle4 = "QTR20";
    }
    else if (strQtr == "Q21") {
        strField1 = "OData__x0041_pr18"; strField2 = "OData__x004d_ay18"; strField3 = "OData__x004a_un18"; strField4 = "Q01Target";
        strTitle1 = "Apr-18"; strTitle2 = "May-18"; strTitle3 = "Jun-18"; strTitle4 = "QTR21";
    }


    if (strCheck == "1") {
        columns.push({ field: strField1, aggregate: "sum" },
            { field: strField2, aggregate: "sum" }, { field: strField3, aggregate: "sum" }, { field: strField4, aggregate: "sum" })
    } else if (strCheck == "2") {
        columns.push({
            title: "Next Period",
            columns: [
                { width: "100px", field: strField1, title: strTitle1, groupFooterTemplate: "#=sum#" },
                { width: "100px", field: strField2, title: strTitle2, groupFooterTemplate: "#=sum#" },
                { width: "100px", field: strField3, title: strTitle3, groupFooterTemplate: "#=sum#" },
                { width: "100px", field: strField4, title: strTitle4, groupFooterTemplate: "#=sum#" }
            ]
        });
    }




    return columns;
}

function generateColumns(strQtr) {
    var columns =
    [
         {
             width: "100px",
             field: "Project",
             title: "Project"
         },
          {
              width: "100px",
              field: "Activity",
              title: "Activity"
          },
           {
               width: "100px",
               field: "Sub_x0020_Activity",
               title: "Sub Activity"
           },
        {
            width: "100px",
            field: "WBSIDId",
            title: "WBSID",
        }, {
            width: "240px",
            field: "Title",
            title: "Project/ Activity/ Sub Activity/ Task",
        }, {
            width: "120px",
            field: "ActualCumulativeDisbursements",
            title: "Actual Cumulative at the Beginning of Current Period",
            groupFooterTemplate: "#=sum#"
        }
    ];
    columns.push.apply(columns, getCurrentPeriod(strQtr, "2"));
    columns.push.apply(columns, getNextPeriod(strQtr, "2"));
    columns.push.apply(columns, getQTR(strQtr, "2"));

    var strcolumnForecast = tempList == "QDR%20Commitment" ? "Commitment as Currently Forecasted" : "Disbursement as Currently Projected";

    var lastcolumns =
    [
       {
           width: "130px",
           field: "DisbursementsasCurrentlyProjecte",
           title: strcolumnForecast,
           groupFooterTemplate: "#=sum#"
       }, {
           width: "130px",
           field: "ApprovedMulti_x002d_YearFinancia",
           title: "Approved Multi-Year Financial Plan(Should match with Schedule B)",
           groupFooterTemplate: "#=sum#"
       }, {
           width: "130px",
           field: "ProjectionsVsApprovedPlanUnder_x",
           title: "Projections Vs Approved Plan Under/Over Budget Difference",
           groupFooterTemplate: "#=sum#"
       }
    ];

    columns.push.apply(columns, lastcolumns);

    return columns;
}

function getQTR(strQtr, strCheck) {
    var columns = [];
    var intLoop = getIntLoop(strQtr);

    if (strCheck == "1") {
        for (var x = intLoop ; x < 21; x++) {
            columns.push({ field: "Q" + (x + 1) + "Target", aggregate: "sum" })
        }
    } else if (strCheck == "2") {
        for (var i = intLoop ; i < 21; i++) {
            columns.push({ width: "100px", field: "Q" + (i + 1) + "Target", title: "QTR " + (i + 1), groupFooterTemplate: "#=sum#" })
        }
    }

    return columns;
}

function getAgregate(strQtr) {

    var columns =
   [
      {
          field: "ActualCumulativeDisbursements",
          aggregate: "sum"
      },
      {
          field: "DisbursementsasCurrentlyProjecte",
          aggregate: "sum"
      },
      {
          field: "ApprovedMulti_x002d_YearFinancia",
          aggregate: "sum"
      }, {
          field: "ProjectionsVsApprovedPlanUnder_x",
          aggregate: "sum"
      }
   ];

    columns.push.apply(columns, getCurrentPeriod(strQtr, "1"));
    columns.push.apply(columns, getNextPeriod(strQtr, "1"));
    columns.push.apply(columns, getQTR(strQtr, "1"));
    return columns;
}


function mydataSource(_url, strQtr) {



    var dataSource = new kendo.data.DataSource({
        group: [
        {
            field: "Project", aggregates: getAgregate(strQtr)
        },
        {
            field: "Activity", aggregates: getAgregate(strQtr)
        },
        {
            field: "Sub_x0020_Activity", aggregates: getAgregate(strQtr)
        }],
        type: "json",
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
            },
            model: {
                fields: {
                    Project: { type: "string" },
                    Title: { type: "string" },
                    ActualCumulativeDisbursements: { type: "number" },
                    DisbursementsasCurrentlyProjecte: { type: "number" },
                    ApprovedMulti_x002d_YearFinancia: { type: "number" },
                    ProjectionsVsApprovedPlanUnder_x: { type: "number" }
                }
            }
        }

    });

    return dataSource;
}


EPMO.Grids.displayGrid = function (divId, strList, strQuarter, strProject, strMain) {


    var _url;
    var str = _spPageContextInfo.webAbsoluteUrl;
    var intLength = 0;
    var strQuery;
    if (strMain == "no") { intLength = 3 };

    if (isEmpty(strProject) == false) {
        if (isEmpty(strQuarter)) {
            strQuery = "&$filter= ((Quarter eq 'Q01') and (Project eq '" + strProject + "'))";
        } else {
            strQuery = "&$filter= ((Quarter eq '" + strQuarter + "') and (Project eq '" + strProject + "'))";
            $("#" + divId).empty();
        }

    }
    else {

        if (isEmpty(strQuarter)) {
            strQuery = "&$filter= (Quarter eq 'Q01')";
        } else {
            strQuery = "&$filter= (Quarter eq '" + strQuarter + "')";
            $("#" + divId).empty();
        }


    }

    tempList = isEmpty(strList) == true ? "QDR%20Commitment" : strList;



    //_url = str.substring(0, str.length - intLength) + "/_api/web/lists/getbytitle('"
    //    + tempList + "')/Items?$top=10000&" + strQuery;

    _url = str.substring(0, str.length - intLength) + "/_api/web/lists/getbytitle('"
       + tempList + "')/Items?$select=*,WBSID/ID&$expand=WBSID";


    $("#" + divId).kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: "Detailed Financial Plan Report.xlsx",
            allPages: true
        },
        dataSource: mydataSource(_url, strQuarter),
        height: 550,
        width: 1250,
        sortable: true,
        scrollable: true,
        resizable: true,
        reorderable: true,
        groupable: false,
        pageable: {
            refresh: true,
            pageSizes: true,
            buttonCount: 5,
        },
        filterable: true,
        columns: generateColumns(strQuarter),
    });

};



function datatype() {

    var mydata = [{ text: "Commitment", value: "QDR%20Commitment" },
            { text: "Disbursement", value: "QDR%20Disbursement" }];


    return mydata;
}

EPMO.Grids.displayDDLType = function (divId) {
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

    });
};

