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
        url: "/Operations/InvoicePayment/GetInvoiceHeader",
        data: { "invoiceHeaderId": $('#ddlInvoiceHeaders').val() },
        success: function (response) {
            UpdateTextBoxes(response);
        }
    });
}

function UpdateTextBoxes(response) {
    if (response != null) {
        if (response.netAmountToPay != null) $('#txtInvoiceHeaderNetAmountToPay').val(formatNumber(response.netAmountToPay));
        if (response.advancedAmount != null) $('#txtInvoiceHeaderAdvancedAmount').val(formatNumber(response.advancedAmount));
        if (response.remainingAmountToPay != null) $('#txtInvoiceHeaderRemainingAmountToPay').val(formatNumber(response.remainingAmountToPay));
        if (response.customer != null) $('#txtInvoiceHeaderCustomerName').val(response.customer.name);
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
