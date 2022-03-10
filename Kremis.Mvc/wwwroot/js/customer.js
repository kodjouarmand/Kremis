$('#form-modal').on('shown.bs.modal', function () {
    SetFocus();
})

function SetFocus() {
    if ($('#txtRefrence').val() == '')
        $('#txtRefrence').trigger('focus');
}


