$('#form-modal').on('shown.bs.modal', function () {
    RegisterEvents();
    SetFocus();
})

function SetFocus() {
    if ($('#txtNumber').val() == '')
        $('#txtNumber').trigger('focus');
}

function RegisterEvents() {
    $('#ddlLandTitles').on('change', function () {
        GetLandTitle();
    });
}

function GetLandTitle() {
    $.ajax({
        type: "GET",
        url: "/Admin/Parcel/GetLandTitle",
        data: { "landTitleId": $('#ddlLandTitles').val() },
        success: function (response) {
            LandTitleChange(response);
        }
    });
}

function LandTitleChange(response) {
    $('#ddlLocalities').val(response.localityId);
}
