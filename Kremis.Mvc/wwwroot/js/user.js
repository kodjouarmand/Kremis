var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "25%" },
            { "data": "userName", "width": "20%" },
            { "data": "email", "width": "25%" },           
            { "data": "roleName", "width": "20%" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        //user is currently locked
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer;">
                                    <i class="fas fa-lock-open"></i>
                                </a>
                            </div>
                           `;
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-sm btn-success text-white" style="cursor:pointer;">
                                    <i class="fas fa-lock"></i>
                                </a>
                            </div>
                           `;
                    }
                    
                }, "width": "15%"
            }
        ]
    });
}

function LockUnlock(id) {
    
            $.ajax({
                url: '/Admin/User/LockUnlock?id='+id,
                data: JSON.stringify(id),
                contentType: "application/json",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
      
}