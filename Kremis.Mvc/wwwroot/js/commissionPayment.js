$('#form-modal').on('shown.bs.modal', function () {
    RegisterEvents();
})

$(document).ready(function () {
    RegisterEvents();
    GetInvoiceHeader();
    DisplayOrHideDivTransactionNumber();
});

function RegisterEvents() {
    $('#ddlInvoiceHeaders').on("change", function () {
        GetInvoiceHeader();
    });
    $('#ddlPaymentModes').on("change", function () {
        DisplayOrHideDivTransactionNumber();
    });
}

function SetFocus() {
    $('#txtAmountPaid').trigger('focus');
}

function GetInvoiceHeader() {
    $.ajax({
        type: "GET",
        url: "/Operations/CommissionPayment/GetInvoiceHeader",
        data: { "invoiceHeaderId": $('#ddlInvoiceHeaders').val() },
        success: function (response) {
            UpdateTextBoxes(response);
        }
    });
}

function UpdateTextBoxes(response) {
    if (response != null) {
        if (response.commissionToPay != null) $('#txtInvoiceHeaderCommissionToPay').val(formatNumber(response.commissionToPay));
        if (response.commissionPaid != null) $('#txtInvoiceHeaderCommissionPaid').val(formatNumber(response.commissionPaid));
        if (response.commissionRemainingToPay != null) $('#txtInvoiceHeaderCommissionRemainingToPay').val(formatNumber(response.commissionRemainingToPay));
        if (response.businessPartner != null) $('#txtInvoiceHeaderBusinessPartnerName').val(response.businessPartner.name);
    }
}

function DisplayOrHideDivTransactionNumber() {
    var paymentMode = $('#ddlPaymentModes option:selected').text();
    if (paymentMode == "Cash" || paymentMode == "Sélectionnez ...") {
        $('#divTransactionNumberText').css("display", "none");
        $('#divTransactionNumberLabel').css("display", "none");
    }
    else {
        $('#divTransactionNumberText').css("display", "block");
        $('#divTransactionNumberLabel').css("display", "block");
    }
}
