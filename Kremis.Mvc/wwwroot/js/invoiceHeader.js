$('#form-modal').on('shown.bs.modal', function () {
    RegisterEvents();
})

$(document).ready(function () {
    RegisterEvents();
});

function RegisterEvents() {
    $('#ddlCommissionTypes').on("change", function () {
        $('#txtCommissionUnitValue').val('');
    });

    $('#ddlParcels').on("change", function () {
        GetParcel();
    });

    $('#txtSurface').on('change', function () {
        CalculateTotal();
    });

    $('#txtUnitPrice').on('change', function () {
        CalculateTotal();
    })
}

function GetParcel() {
    $.ajax({
        type: "GET",
        url: "/Operations/InvoiceHeader/GetParcel",
        data: { "parcelId": $('#ddlParcels').val() },
        success: function (response) {
            UpdateTextBoxes(response);
        }
    });
}

function UpdateTextBoxes(response) {
    $('#txtSurface').val(response.surface);
    $('#txtUnitPrice').val(response.unitPrice);
    CalculateTotal();
}

function CalculateTotal() {
    try {
        var total = $('#txtSurface').val() * $('#txtUnitPrice').val();
        $('#txtTotal').val(formatNumber(total));
    }
    catch { $('#txtTotal').val('0'); }
}

function DeleteInvoiceDetail(url) {
    swal({
        title: "Êtes vous certain de vouloir supprimer ?",
        text: "Vous ne pourrez plus restaurer l'enregistrement supprimé!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        location.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}

function DeleteInvoiceDetailWhithoutAlert(url) {
    $.ajax({
        url: url,
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                location.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    });
}

