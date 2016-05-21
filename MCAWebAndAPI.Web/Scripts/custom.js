

function onFailureForm(e) {
    $('fieldset').prepend('<div class="alert alert-danger fade in">' + e.errorMessages + '</div>');
}

function onSuccessForm(data) {
    parent.postMessage("Success", data.urlToRedirect);
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


$(document).ready(function () {

    if ($('#print-button').length) {
        $('#print-button').click(printForm);
    }

});