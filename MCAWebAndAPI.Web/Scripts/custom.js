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
    if (e == null || e.successMessage == null)
        onFailureForm(e);

    $('#modal-html-content').html('<div class="alert alert-success alert-block"><h4 class="alert-heading">Success!</h4>'
        + e.successMessage + '</div>');
}

// It is only used if embedded in SharePoint
function onSuccessFormEmbed(e) {
    if (e == null || e.successMessage == null)
        onFailureForm(e);

    $('#modal-html-content').html('<div class="alert alert-success alert-block"><h4 class="alert-heading">Success!</h4>'
        + e.successMessage + '</div>');

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

function onEditKendoDetail(e) {
    if (!e.model.isNew()) {
        var container = e.container;
        var tr = container.closest('tr');
        var hiddenInput = tr.find("input[name*='EditMode']");

        // 1 is Item.Mode.UPDATED
        hiddenInput.val(1);
    }
}

function onDeleteKendoDetail(e) {
    var result = confirm("Are you sure you want to delete this item?");
    if (!result) return;

    var target = e.target || e.srcElement;
    var button = $(target);
    var tr = button.closest('tr');

    // Set IsModified flag to True
    hiddenInput = tr.find("input[name*='EditMode']");

    // -1 is Item.Mode.DELETED
    hiddenInput.val(-1);

    tr.hide();
}