
function onBeginForm() {
    showLoading();
}

function onFailureForm(e) {
    $('#modal-html-content').html('<div class="alert alert-danger fade in">' + e.errorMessage + '</div>');
}

// Do not use this if embedded in SharePoint
function onCompleteForm() {
    hideLoading();
    $("#remoteModal").modal('show');
}

function onSuccessForm(e) {
    $('#modal-html-content').html('<div class="alert alert-success alert-block"><h4 class="alert-heading">Success!</h4>'
        + e.successMessage + '</div>');
}

// It is only used if embedded in SharePoint
function onSuccessFormEmbed(data) {
    parent.postMessage("Success", data.urlToRedirect);
}

function showLoading() {
    $body = $("body");
    $body.addClass("loading");
}

function hideLoading() {
    $body = $("body");
    $body.removeClass("loading");
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


$('#remoteModal').on('hidden.bs.modal', function () {
    location.reload();
})

// To put all necessary functions triggerd when document is loaded 
$(document).ready(function () {

    if ($('#print-button').length) {
        $('#print-button').click(printForm);
    }


});