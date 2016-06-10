﻿// To put all necessary functions triggerd when document is loaded 
$(document).ready(function () {

    if ($('#print-button').length) {
        $('#print-button').click(printForm);
    }

});

function showModalWindow(data) {
    $("#remoteModal").modal('show');

    if (data.success == null) {
        $('#modal-html-content').html(data);
    } else {
        if (data.success) {
            $('#modal-html-content').html('<div class="alert alert-success alert-block"><h4 class="alert-heading">Success!</h4>'
            + (data.successMessage != null ? data.successMessage : "") + '</div>');
        } else {
            $('#modal-html-content').html('<div class="alert alert-danger fade in"><h4 class="alert-heading">Failed</h4>'
                + data.errorMessage + '</div>');
        }
    }
}


function onUpload(e) {
    var files = e.files;

    $.each(files, function () {
        if (this.size > 1 * 1024 * 1024) {
            alert(this.name + " is too big!");
            e.preventDefault(); // This cancels the upload for the file
        }
    });
}

function onBeginForm() {
    showLoading();
}

function onFailureForm(e) {
    if (e.success)
        onSuccessFormEmbed(e);

    $('#modal-html-content').html('<div class="alert alert-danger fade in">'
        + e.responseJSON.errorMessage + '</div>');
}

// Do not use this if embedded in SharePoint
function onCompleteForm() {
    hideLoading();
    $("#remoteModal").modal('show');
}


function onSuccessForm(e) {

    $('#modal-html-content').html('<div class="alert alert-success alert-block"><h4 class="alert-heading">Success!</h4>'
        + ( e.successMessage != null ? e.successMessage : "" ) + '</div>');
}

// It is only used if embedded in SharePoint
function onSuccessFormEmbed(e) {

    $('#modal-html-content').html('<div class="alert alert-success alert-block"><h4 class="alert-heading">Success!</h4>'
        + (e.successMessage != null ? e.successMessage : "") + '</div>');

    setTimeout(function () {
        parent.postMessage(e, e.urlToRedirect);
    }, 3000);
}

function showLoading() {
    $body = $("body");
    $body.addClass("loading");
}

function hideLoading() {
    $body = $("body");
    $body.removeClass("loading");
}

function submitFormToPrint(url) {
    if (!$('form').valid()) {
        alert('Please make sure that all required fields are filled');
        return;
    }
    $('form').prop('action', url);
    $('form').submit();
}

function printForm(e) {

    if (!$('form').valid()) {
        alert('Please make sure that all required fields are filled');
        return;
    }   

    var actionUrl = $('form').attr("action");
    var newActionUrl = actionUrl.replace("Create", "Print");

    $('form').prop('action', newActionUrl);
    $('form').submit();
}

// Event when the modal window is closed
$('#remoteModal').on('hidden.bs.modal', function () {

})


function onEditKendoDetail(e) {
    if (!e.model.isNew()) {
        var container = e.container;
        var tr = container.closest('tr');
        var data = this.dataItem(tr); //get the row data so it can be referred later
        // 1 is Item.Mode.UPDATED
        data.set("EditMode", 1);
    }
}

function onDeleteKendoDetail(e) {
    var result = confirm("Are you sure you want to delete this item?");
    if (!result) return;

    var tr = $(e.target).closest("tr"); //get the row
    var data = this.dataItem(tr); //get the row data so it can be referred later

    // -1 is Item.Mode.DELETED
    data.set("EditMode", -1);
}

function hideDeletedRowKendoDetail(grid) {
    var gridData = grid.dataSource.view();

    for (var i = 0; i < gridData.length; i++) {
        var currentUid = gridData[i].uid;
        if ((gridData[i].EditMode == -1)) {
            var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
            currenRow.hide();
        }
    }
}

function getMonthName(date) {
    var month = new Array();
    month[0] = "January";
    month[1] = "February";
    month[2] = "March";
    month[3] = "April";
    month[4] = "May";
    month[5] = "June";
    month[6] = "July";
    month[7] = "August";
    month[8] = "September";
    month[9] = "October";
    month[10] = "November";
    month[11] = "December";
    return month[date.getMonth()];
}