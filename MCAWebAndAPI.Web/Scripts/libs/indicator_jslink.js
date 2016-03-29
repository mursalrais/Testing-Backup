//modified date : 25 Feb 16
//modified by : iMa

(function () {

    // Create object that have the context information about the field that we want to change it's output render 
    var percentCompleteFiledContext = {};
    percentCompleteFiledContext.Templates = {};
    percentCompleteFiledContext.Templates.Fields = {
        // Apply the new rendering for PercentComplete field on List View, Display, New and Edit forms
        "PercentComplete": { 
            "View": percentCompleteViewFiledTemplate,
            "DisplayForm": percentCompleteViewFiledTemplate,
            "NewForm": percentCompleteEditFiledTemplate,
            "EditForm": percentCompleteEditFiledTemplate
        }
    , "_x0025__x0020_Complete" : { 
            "View": percentCompleteViewFiledTemplate
            }
     };
	
	var statusFieldCtx = {};
    statusFieldCtx.Templates = {};
    statusFieldCtx.Templates.Fields = {
        "Schedule_x0020_Status": {
            "View": StatusFieldViewTemplate
        }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(percentCompleteFiledContext);
	SPClientTemplates.TemplateManager.RegisterTemplateOverrides(statusFieldCtx);

})();

// This function provides the rendering logic for View and Display form
function percentCompleteViewFiledTemplate(ctx) {

    var percentComplete = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    return "<div style='background-color: #e5e5e5; width: 100px;  display:inline-block;'> \
            <div style='width: " + percentComplete.replace(/\s+/g, '') + "; background-color: #0094ff;'> \
            &nbsp;</div></div>&nbsp;" + percentComplete;

}

// This function provides the rendering logic for New and Edit forms
function percentCompleteEditFiledTemplate(ctx) {

    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);

    // Register a callback just before submit.
    formCtx.registerGetValueCallback(formCtx.fieldName, function () {
        return document.getElementById('inpPercentComplete').value;
    });

    return "<input type='range' id='inpPercentComplete' name='inpPercentComplete' min='0' max='100' \
            oninput='outPercentComplete.value=inpPercentComplete.value' value='" + formCtx.fieldValue + "' /> \
            <output name='outPercentComplete' for='inpPercentComplete' >" + formCtx.fieldValue + "</output>%";

}


function StatusFieldViewTemplate(ctx) {

    var _statusValue = ctx.CurrentItem.Schedule_x0020_Status;
console.log("cek... : "+_statusValue );
https://eceos2.sharepoint.com/sites/mca-dev/_catalogs/masterpage/zz_mcatheme/img/Indicators/blue.png

     if (_statusValue == 'Completed' || _statusValue == 'Closed')
     {
        return "<img src='../../../_catalogs/masterpage/zz_mcatheme/img/Indicators/darkgreen.png'/>";
     }
    
     if (_statusValue == 'Incomplete')
     {
        return "<img src='../../../_catalogs/masterpage/zz_mcatheme/img/Indicators/green.png'/>";
     } 

     if (_statusValue == 'Future')
     {
        return "<img src='../../../_catalogs/masterpage/zz_mcatheme/img/Indicators/blue.png'/>";
     }   
     
     if (_statusValue == 'On Schedule')
     {
        return "<img src='../../../_catalogs/masterpage/zz_mcatheme/img/Indicators/green.png'/>";
     }
     
     if (_statusValue == 'Significantly Behind Schedule')
     {
        return "<img src='../../../_catalogs/masterpage/zz_mcatheme/img/Indicators/red.png'/>";
     }

}
