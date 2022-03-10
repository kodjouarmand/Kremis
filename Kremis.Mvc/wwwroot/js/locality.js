$('#form-modal').on('shown.bs.modal', function () {
    SetFocus();
})

function SetFocus() {
    if ($('#txtNumber').val() == '')
        $('#txtNumber').trigger('focus');
}

