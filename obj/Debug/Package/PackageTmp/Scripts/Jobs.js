$(document).ready(function () {
    $(".datetime").datepicker({
        "autoclose": true,
        "todayHighlight": true,
        "dateFormat": "dd/mm/yyyy",
        "format": "dd/mm/yyyy",
    }).datepicker("setDate", new Date());

    var toggler = document.getElementsByClassName("caret");
    var i;

    for (i = 0; i < toggler.length; i++) {
        toggler[i].addEventListener("click", function () {
            this.parentElement.querySelector(".nested").classList.toggle("active");
            this.classList.toggle("caret-down");
        });
    }
    $(".task").change(function () {
        var id = $(this).attr("id");
        var task = this;
        var status = this.checked ? 1 : 0;
        $.ajax({
            url: '/Jobs/CheckTask',
            type: "POST",
            dataType: "JSON",
            data: { id: id, status: status },
            success: function (data) {
                $('.caret', $(task).parent().parent().parent()).html(data);
            }
        });
    });
    $(".edit").toggle();

});
function btnNew_click(id) {
    var isNew = $("#myUL" + id + " .new").length;
    if (isNew > 0) {
        $("#myUL" + id + " .new input").focus().select();
    }
    else {
        $("#myUL" + id).prepend(" <li class=\"new\"><input class=\"form-control\" /></li>");
        $("#myUL" + id + " .new input").blur(function () {
            if (this.value != null && this.value != "") {
                $.ajax({
                    url: '/Jobs/Add',
                    type: "POST",
                    dataType: "JSON",
                    data: { name: this.value, process_by: id },
                    success: function (data) {
                        $("#myUL" + id + " .new").remove();
                        $("#myUL" + id).prepend(
                            "<li id=j" + data.id + ">" +
                            "   <span class=\"caret\">" + data.name + "</span><a class=\"edit\" style='display: none;' onclick='DelJob_Click(" + data.id + ");'>X</a>" +
                            "   <ul class=\"nested\">" +
                            "       <li><a onclick='AddTask_Click(this);'>Thêm nhiệm vụ</a>&nbsp;|&nbsp; <a onclick='EditTask_Click(this);'>Xóa</a></li>" +
                            "   </ul>" +
                            "</li>");
                        $("#myUL" + id + " #j" + data.id + " .caret").off("click").click(function () {
                            $(".nested", $(this).parent()).toggleClass("active");
                            $(this).toggleClass("caret-down");
                        });
                    }
                });
            }
        });
    }
    $("#myUL" + id + " .new input").focus().select();
}
function AddTask_Click(ctr) {
    $(ctr).parent().parent().append(
        "       <li><input class='form-control' onblur='Task_OnBlur(this);'  /> </li>"
    );
}
function EditTask_Click(ctr) {
    var parent = $(ctr).parent().parent().parent();
    $(".edit", parent).toggle();
}

function Task_OnBlur(ctr) {
    var pid = $(ctr).parent().parent().parent().attr("id").replace("j", "");
    if ($(ctr).val() != null && $(ctr).val() != '') {
        $.ajax({
            url: '/Jobs/AddTask',
            type: "POST",
            dataType: "JSON",
            data: { name: $(ctr).val(), job_id: pid },
            success: function (data) {
                $(ctr).parent().append(
                    "<input type='checkbox' class='ntask' id=" + data.id + ">" +
                    "   <span>" + data.name + "</span><a class=\"edit\" style='display: none;' onclick='DelTask_Click(" + data.id + ");'>X</a>"
                );
                $(".ntask", $(ctr).parent()).change(function () {
                    var id = $(this).attr("id");
                    var task = this;
                    var status = this.checked ? 1 : 0;
                    $.ajax({
                        url: '/Jobs/CheckTask',
                        type: "POST",
                        dataType: "JSON",
                        data: { id: id, status: status },
                        success: function (data) {
                            $('.caret', $(task).parent().parent().parent()).html(data);
                        }
                    });
                });
                $('.caret', $(ctr).parent().parent().parent()).html(data.j);
                $(ctr).remove();
            }
        });
    }
}
function DelTask_Click(id) {
    $.ajax({
        url: '/Jobs/DelTask',
        type: "POST",
        dataType: "JSON",
        data: { id: id },
        success: function (data) {
            $('.caret', $("#j" + data.j)).html(data.name);
            $('#' + id, $("#j" + data.j)).parent().remove();
        }
    });
}
function DelJob_Click(id) {
    $.ajax({
        url: '/Jobs/DelJob',
        type: "POST",
        dataType: "JSON",
        data: { id: id },
        success: function (data) {
            $('#j' + id).remove();
        }
    });
}

function Print(id) {
    $.ajax({
        url: '/Jobs/Print',
        type: "POST",
        dataType: "JSON",
        data: { id: id },
        success: function (data) {
            document.getElementById('my_iframe').src = data;
        }
    });
}
function formatRepo(repo) {
    if (repo.loading) {
        return repo.text;
    }

    var $container = $(
        "<div class='select2-result-repository clearfix'>" +
        "   <div class='select2-result-repository-meta'>" +
        "       <div class='select2-result-repository-title'></div>" +
        "       <div class='select2-result-repository-description'></div>" +
        "   </div>" +
        "</div>"
    );

    $container.find(".select2-result-repository-title").text(repo.name);
    $container.find(".select2-result-repository-description").text(repo.text);

    return $container;
}

function formatRepoSelection(repo) {
    return repo.name || repo.text;
}
function Go() {

    $('.modal').on('shown.bs.modal', function () {
        $("#cboHopDong").select2({
            ajax: {
                url: "/Jobs/GetContract",
                dataType: 'json',
                type: "Get",
                delay: 500,
                data: function (params) {
                    return params;
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;
                    return {
                        results: data.items,
                        pagination: {
                            more: (params.page * 25) < data.total_count
                        }
                    };
                },
                cache: true
            },
            dropdownParent: $('.modal .modal-content'),
            placeholder: 'VD: Nguyễn Văn A',
            templateResult: formatRepo,
            templateSelection: formatRepoSelection
        });
    });
    // Select 2 Modal
    $('.modal').modal('show');

}
function OnGo() {
    var obj = new Object();
    obj.contract_id = $("#cboHopDong").val();
    obj.ngaynop = $("#txtNgayNop").val();
    obj.ngaytra = $("#txtNgayTraKQ").val();
    obj.canbo1cua = $("#txtCanBo1Cua").val();
    //obj.canbophutrach = $("#txtCanBoPhuTrach").val();
    obj.maphieuhen = $("#txtMaPhieuHen").val();

    $.ajax({
        url: '/di1cua/Create',
        type: "POST",
        dataType: "JSON",
        data: { di1cua: obj, ajax: true },
        success: function (data) {
            $('.modal').modal('hide');
            toastr.success("Đã chuyển đi nộp 1 cửa", "THÀNH CÔNG", { timeOut: 5000 });
        }
    });
}
function AlreadyGo() {
    location.href = "/di1cua";
}
function GoDiaChinh() {
    location.href = "/didiachinh";
}
function GoGiayTo() {
    location.href = "/giaonhangiaytoes";
}
