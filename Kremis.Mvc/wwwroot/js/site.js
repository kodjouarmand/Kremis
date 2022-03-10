// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function formatNumber(num) {
    if (num != null)
        return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1 ')
}

$(function () {
    $("#loaderbody").addClass('hide');

    $(document).bind('ajaxStart', function () {
        $("#loaderbody").removeClass('hide');
    }).bind('ajaxStop', function () {
        $("#loaderbody").addClass('hide');
    });
});


showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (response) {
            $('#form-modal .modal-body').html(response);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
            // to make popup draggable
            $('.modal-dialog').draggable({
                handle: ".modal-header"
            });
        }
    })
}

jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.isValid) {
                    $('#parent-view').html(response.html)
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');
                    $.notify(response.message, {
                        globalPosition: 'bottom right',
                        className: 'success',
                        autoHideDelay: 10000
                    });
                }
                else {
                    $('#form-modal .modal-body').html(response.html);
                    $.notify(response.message, {
                        globalPosition: 'bottom right',
                        className: 'error',
                        autoHideDelay: 10000
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

function Delete(url) {
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
                        swal({
                            title: data.message,
                            icon: "success",
                            dangerMode: false,
                            timer: 5000
                        }).then(() => {
                            location.reload();
                        });
                    }
                    else {
                        swal({
                            title: data.message,
                            text: "Veuillez contacter l'administrateur!",
                            icon: "error",
                            dangerMode: true,
                            timer: 10000
                        });
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });
        }
    });
}

function DeleteDocument(url) {
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

