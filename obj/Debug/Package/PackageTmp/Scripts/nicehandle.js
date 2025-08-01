/*Hard File Update*/
$(document).ready(function () {
    $(".datetime").datepicker({
        "autoclose": true,
        "todayHighlight": true,
        "dateFormat": "dd/mm/yyyy",
        "format": "dd/mm/yyyy",
    });
    $('#frmHardFileUpload').on("submit", function (event) {
        var frm = this;
        var id = currentHardFileId;
        var formdata = new FormData(); //FormData object
        var fileInput = $(':file', this)[0];
        for (i = 0; i < fileInput.files.length; i++) {
            formdata.append(fileInput.files[i].name, fileInput.files[i]);
        }
        formdata.append("id", id);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/contracts/UploadHardFile');
        xhr.send(formdata);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                $("#contract" + id + " .aHardFile").removeClass("text-danger").prop("href", JSON.parse(xhr.responseText)).attr("target", "_blank").text("Xem file");
            }
        }
        $("#mdlHardFileUpdate").modal("hide");
        event.preventDefault();
    });
    $(".numberic").autoNumeric('init', { mDec: 0 });
});
var currentHardFileId;
function HardFileUpdate(id) {
    currentHardFileId = id;
    $("#mdlHardFileUpdate").modal("show");
}
function DuyetChi(contract_id, category_id) {
    $("#mdlCreateDuyetChi iframe").attr("src", "/Inouts/CreateContract?contract_id=" + contract_id + "&category_id=" + category_id);
    $("#mdlCreateDuyetChi").modal("show");
}
function SetSelect2(ctr, url, placeholder) {
    //var selected = $(":selected", ctr);
    $(ctr).select2({
        ajax: {
            url: url,
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
        allowClear: true,
        placeholder: placeholder,
        templateResult: formatRepo,
        templateSelection: formatRepoSelection
    });
    //if (selected) {
    //    var newOption = new Option($(selected).text(), $(selected).val(), true, true);
    //    $(ctr).append(newOption).trigger('change');
    //}
}
function formatRepo(repo) {
    if (repo.loading) {
        return repo.text;
    }

    var $container = $(
        "<div class='select2-result-repository clearfix'>" +
        "   <div class='select2-result-repository-meta'>" +
        "       <div class='select2-result-repository-title' style='font-weight: bold;'></div>" +
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
window.closeModal = function (ctr) {
    $(ctr).modal('hide');
    location.reload();
};
$(function () {
    $('.summernote').summernote({
        fontSizes: ['8', '9', '10', '11', '12', '14', '18'],
        fontNames: ['Arial', 'Tahoma'],
        toolbar: [
            ['style', ['bold', 'italic', 'underline']],
            ['fontsize', ['fontsize']],
            ['font', ['strikethrough']],
            ['para', ['ol', 'paragraph']],
            ['color', ['color']],
        ]
    });
    $('.summernote500').summernote({
        height: 500,
        fontSizes: ['8', '9', '10', '11', '12', '14', '18'],
        fontNames: ['Arial', 'Tahoma'],
        toolbar: [
            ['style', ['bold', 'italic', 'underline']],
            ['fontsize', ['fontsize']],
            ['font', ['strikethrough']],
            ['para', ['ol', 'paragraph']],
            ['color', ['color']],
        ]
    });
});
function UploadProgressFile(ctr, lst, hoso_id, progress_id, type) {
    var formdata = new FormData();
    var fileInput = $(ctr)[0];
    for (i = 0; i < fileInput.files.length; i++) {
        formdata.append(fileInput.files[i].name, fileInput.files[i]);
    }
    formdata.append("hoso_id", hoso_id);
    formdata.append("progress_id", progress_id);
    formdata.append("type", type);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Progresses/UploadProgressFile');
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            $(ctr).val(null);
            $(lst).append(JSON.parse(xhr.responseText));
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
        }
    }
}
function SaveHoSoNote(id, ctr) {
    var note = $(ctr).summernote('code');
    $.ajax({
        url: '/Progresses/SaveHoSoNote',
        data: {
            hoso_id: id,
            note: note
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
        }
    });
}
function SaveHopDongNote(id, ctr) {
    var note = $(ctr).summernote('code');
    $.ajax({
        url: '/Progresses/SaveHopDongNote',
        data: {
            contract_id: id,
            note: note
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
        }
    });
}
function ProgressSetting(progress_id, hoso_id) {
    //$("#mdlNH iframe").attr("src", "/progresses/setting/" + id);
    //$("#mdlNH").modal("show");
    $.ajax({
        url: '/progresses/ProgressSetting',
        type: 'GET',
        cache: false,
        data: { progress_id: progress_id, hoso_id: hoso_id }
    }).done(function (result) {
        $("#mdlNH .modal-body").html(result);
        $("#mdlNH").modal("show");
        $("#ss_progress form").submit(function (e) {
            e.preventDefault(); // avoid to execute the actual submit of the form.
            var form = $(this);
            var actionUrl = form.attr('action');
            $.ajax({
                type: "POST",
                url: actionUrl,
                data: form.serialize(), // serializes the form's elements.
                success: function (data) {
                    toastr.success("Đã lưu thành công", "CẤU HÌNH", { timeOut: 5000 });
                }
            });
        });
        $.ajax({
            url: '/Progresses/SettingLoadDocuments',
            data: {
                hoso_id: hoso_id,
                progress_id: progress_id
            },
            type: "GET",
            dataType: "JSON",
            success: function (rs) {
                $("#wrap_docs").html(rs);
                $("#ss_filetai :checkbox").change(function () {
                    var document_id = $(this).attr("id").replace("doc", "");
                    $.ajax({
                        url: '/Progresses/SettingDocumentCheckedChange',
                        data: {
                            document_id: document_id,
                            progress_id: progress_id,
                            ischecked: this.checked
                        },
                        type: "GET",
                        dataType: "JSON",
                        success: function (rs) {
                            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
                        }
                    });
                });
            }
        });
    });
}
function ProgressDownload(progress_id, hoso_id) {

    $.ajax({
        url: '/progresses/ProgressDownload',
        type: 'GET',
        cache: false,
        data: { progress_id: progress_id, hoso_id: hoso_id }
    }).done(function (result) {
        $("#mdlNH .modal-body").html(result);
        $("#mdlNH").modal("show");
    });
}
function Download(document_id, hoso_id) {
    $.ajax({
        url: '/Infomations/Download',
        data: { document_id: document_id, hoso_id: hoso_id },
        type: "GET",
        dataType: "JSON",
        success: function (url) {
            document.getElementById('my_iframe').src = url;
        }
    });
}
function ProgressFileStatus(hoso_id, progress_id, progress_type, ctr) {
    var checked = ctr.checked ? 1 : 0;
    $.ajax({
        url: '/Progresses/ProgressFileStatus',
        data: {
            hoso_id: hoso_id,
            progress_id: progress_id,
            status: checked
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
            ReloadViecPhaiLam(hoso_id, progress_type);
        }
    });
}
function TaoLich(hoso_id, progress_type) {
    var name = $("#ss_" + progress_type + hoso_id + " .alarm [name='viecphailam_name']").val().trim();
    var time_progress = $("#ss_" + progress_type + hoso_id + " .alarm [name='viecphailam_time']").val();
    var account_id = $("#ss_" + progress_type + hoso_id + " .alarm [name='viecphailam_account_id']").val();
    var bell_type = $("#ss_" + progress_type + hoso_id + " .alarm [name='viecphailam_bell_type']").val();

    if (!name || !time_progress) {
        toastr.warning("Lỗi chưa nhập thông tin", "Lịch nhắc nhớ", { timeOut: 5000 });
        return false;
    }
    $.ajax({
        url: '/Progresses/TaoLich',
        data: {
            hoso_id: hoso_id,
            progress_type: progress_type,
            name: name,
            time_progress: time_progress,
            account_id: account_id,
            bell_type: bell_type
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Lịch nhắc nhớ", { timeOut: 5000 });
            $("#ss_" + progress_type + hoso_id + " .alarm [name='viecphailam_name']").val("");
            $("#ss_" + progress_type + hoso_id + " .alarm [name='viecphailam_time']").val("");
            ReloadViecPhaiLam(hoso_id, progress_type);
        }
    });
}
function ProgressSaveResult(id, hoso_id, progress_type) {
    var result = $("#ss_" + progress_type + hoso_id + " [name='viecphailam_result']").val();
    $.ajax({
        url: '/Progresses/LuuKetQua',
        data: {
            id: id,
            hoso_id: hoso_id,
            progress_type: progress_type,
            result: result
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
            ReloadViecPhaiLam(hoso_id, progress_type);
        }
    });
}
function ReloadViecPhaiLam(hoso_id, progress_type) {
    $.ajax({
        url: '/Progresses/ReloadViecPhaiLam',
        data: {
            hoso_id: hoso_id,
            progress_type: progress_type,
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            $("#ss_" + progress_type + hoso_id + " [name='viecphailam']").html(rs);
        }
    });
}
function CopyText(ctr) {
    $(ctr).select();
    document.execCommand("copy");
    toastr.success($(ctr).val(), "COPY", { timeOut: 5000 });
}
function LuuDinhViThuaDat(hoso_id, ctr) {
    var googlemap = $(ctr).val();
    $.ajax({
        url: '/Hosoes/LuuDinhViThuaDat',
        data: {
            hoso_id: hoso_id,
            googlemap: googlemap,
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
        }
    });
}
function LuuDoveNote(dove_id, ctr) {
    var note = $(ctr).val();
    $.ajax({
        url: '/Hosoes/LuuDoveNote',
        data: {
            dove_id: dove_id,
            note: note,
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
        }
    });
}
function LuuFileAutoCad(hoso_id, ctr) {
    var link_filecad = $(ctr).val();
    $.ajax({
        url: '/Hosoes/LuuFileAutoCad',
        data: {
            hoso_id: hoso_id,
            link_filecad: link_filecad,
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
        }
    });
}
function UpdateProcessing(hoso_id, ctr) {
    var note = $(ctr).val();
    $.ajax({
        url: '/Hosoes/UpdateProcessing',
        data: {
            hoso_id: hoso_id,
            note: note,
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
            ReloadProcessing(hoso_id);
        }
    });
}
function ReloadProcessing(hoso_id) {
    $.ajax({
        url: '/Hosoes/ReloadProcessing',
        data: {
            hoso_id: hoso_id
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            $("#ulProcessing").html(rs);
        }
    });
}
function ShowHoSoSession(hoso_id, progress_type) {
    $.ajax({
        url: '/Hosoes/ShowHoSoSession',
        type: 'GET',
        cache: false,
        data: { hoso_id: hoso_id, progress_type: progress_type }
    }).done(function (result) {
        $("#mdlNH .modal-body").html(result);
        $("#mdlNH").modal("show");
        ReloadViecPhaiLam(hoso_id, progress_type);
        ReloadDonThu(hoso_id, progress_type);
        $(".fa-angle-down").click();
        $('.summernote').summernote({
            fontSizes: ['8', '9', '10', '11', '12', '14', '18'],
            fontNames: ['Arial', 'Tahoma'],
            toolbar: [
                ['style', ['bold', 'italic', 'underline']],
                ['fontsize', ['fontsize']],
                ['font', ['strikethrough']],
                ['para', ['ol', 'paragraph']],
                ['color', ['color']],
            ]
        });
        $(".datetime").datepicker({
            "autoclose": true,
            "todayHighlight": true,
            "dateFormat": "dd/mm/yyyy",
            "format": "dd/mm/yyyy",
        });
    });
}
function HoSoCheckNotice(hoso_id, progress_type) {
    var result = "";
    $.ajax({
        url: '/Hosoes/HoSoCheckNotice',
        type: 'GET',
        dataType: "JSON", data: { hoso_id: hoso_id, progress_type: progress_type },
        success: function (rs) {
            result = rs;
        }
    });
    return result;
}
function ShowDi1CuaSession(di1cua_id, hoso_id, progressfile_type, progress_type) {
    $.ajax({
        url: '/di1cua/ShowDi1CuaSession',
        type: 'GET',
        cache: false,
        data: { di1cua_id: di1cua_id, progressfile_type: progressfile_type }
    }).done(function (result) {
        $("#mdlNH .modal-body").html(result);
        $("#mdlNH").modal("show");
        ReloadViecPhaiLam(hoso_id, progress_type);
        $('.summernote').summernote({
            fontSizes: ['8', '9', '10', '11', '12', '14', '18'],
            fontNames: ['Arial', 'Tahoma'],
            toolbar: [
                ['style', ['bold', 'italic', 'underline']],
                ['fontsize', ['fontsize']],
                ['font', ['strikethrough']],
                ['para', ['ol', 'paragraph']],
                ['color', ['color']],
            ]
        });
        $(".datetime").datepicker({
            "autoclose": true,
            "todayHighlight": true,
            "dateFormat": "dd/mm/yyyy",
            "format": "dd/mm/yyyy",
        });
    });
}
function ShowTienDo(hoso_id) {
    $.ajax({
        url: '/di1cua/ShowTienDo',
        type: 'GET',
        cache: false,
        data: { hoso_id: hoso_id }
    }).done(function (result) {
        $("#mdlNH .modal-body").html(result);
        $("#mdlNH").modal("show");
    });
}
function DailyReport() {
    $.ajax({
        url: '/Jobs/DailyReport',
        type: "POST",
        dataType: "JSON",
        data: {},
        success: function (data) {
            document.getElementById('my_iframe').src = data;
        }
    });
}
function DSCongViec() {
    $.ajax({
        url: '/Hosoes/DSCongViec',
        type: "POST",
        dataType: "JSON",
        data: {},
        success: function (url) {
            document.getElementById('my_iframe').src = url;
        },
        error: function () {
            toastr.success("Phát sinh lỗi", "Không lấy được danh sách", { timeOut: 5000 });
        }
    });
}
function UploadDonThu(parent, hoso_id, progress_type) {
    var formdata = new FormData();
    var fileInput = $("#txtDonThuFile", parent)[0];
    var name = $("#txtDonThuName", parent).val();
    var type = $("#cboDonThuLoai", parent).val();

    var cachgui = $("#cboDonThuCachGui", parent).val();
    var lydo = $("#txtDonThuLyDo", parent).val();

    var time = $("#txtDonThuNgayGui", parent).val();
    for (i = 0; i < fileInput.files.length; i++) {
        formdata.append(fileInput.files[i].name, fileInput.files[i]);
    }
    formdata.append("hoso_id", hoso_id);
    formdata.append("name", name);
    formdata.append("type", type);
    formdata.append("time", time);

    formdata.append("cachgui", cachgui);
    formdata.append("lydo", lydo);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Progresses/UploadDonThu');
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            $("#txtDonThuFile", parent).val(null);
            ReloadDonThu(hoso_id, progress_type);
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
        }
    }
}
function DonThuAddHoiAm(donthu_id) {
    $.ajax({
        url: '/Progresses/ShowDonThuModal',
        type: 'GET',
        cache: false,
        data: { donthu_id: donthu_id }
    }).done(function (result) {
        $("#mdlNH .modal-body").html(result);
        $("#mdlNH").modal("show");
        $(".datetime").datepicker({
            "autoclose": true,
            "todayHighlight": true,
            "dateFormat": "dd/mm/yyyy",
            "format": "dd/mm/yyyy",
        });
    });
}
function UploadVBHoiAm(ctr, donthu_id, progress_type) {
    var formdata = new FormData();
    var fileInput = $(ctr)[0];
    for (i = 0; i < fileInput.files.length; i++) {
        formdata.append(fileInput.files[i].name, fileInput.files[i]);
    }
    var tieude = $("#txtVBHATieuDe", "#vbHoiAm").val();
    var ngaynhan = $("#txtVBHANgayNhan", "#vbHoiAm").val();
    var noidung = $("#txtVBHANoiDung", "#vbHoiAm").val();

    formdata.append("donthu_id", donthu_id);
    formdata.append("tieude", tieude);
    formdata.append("ngaynhan", ngaynhan);
    formdata.append("noidung", noidung);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Progresses/UploadDonThuAction');
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var obj = JSON.parse(xhr.response);
            ReloadDonThu(obj.hoso_id, progress_type);
            $("#mdlNH").modal("hide");
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
        }
    }
}
function ReloadDonThu(hoso_id, progress_type) {
    $.ajax({
        url: '/Progresses/ReloadDonThu',
        data: {
            hoso_id: hoso_id,
            progress_type: progress_type
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            $("#ss_" + progress_type + hoso_id + " tbody").html(rs);
            $(".datetime").datepicker({
                "autoclose": true,
                "todayHighlight": true,
                "dateFormat": "dd/mm/yyyy",
                "format": "dd/mm/yyyy",
            });
        }
    });
}
function AddNhatKyDonThu(donthu_id, hoso_id, progress_type) {
    var note = $("#txtNKDT" + donthu_id).val();
    var time_exp = $("#txtNKDTTime" + donthu_id).val();
    if (!note || !time_exp) {
        toastr.error("Yêu cầu nhập nội dung và thời gian nhắc nhớ", "Đơn Thư", { timeOut: 5000 });
        return false;
    }
    $.ajax({
        url: '/Progresses/AddNhatKyDonThu',
        data: {
            donthu_id: donthu_id,
            note: note,
            time_exp: time_exp
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            ReloadDonThu(hoso_id, progress_type)
        }
    });
}
function UpDownPanel(ctr) {
    $(ctr).toggleClass("fa-angle-up fa-angle-down");
}
//  Setting
function SettingAddFile(type, ctr) {
    var formdata = new FormData();
    var fileInput = $(ctr)[0];
    for (i = 0; i < fileInput.files.length; i++) {
        formdata.append(fileInput.files[i].name, fileInput.files[i]);
    }
    formdata.append("name", $("#txtName").val());
    formdata.append("type", type);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Progresses/SettingAddFile');
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            $("#txtName").val(null);
            var obj = JSON.parse(xhr.response);
            var newRow =
                "<tr><td><a href='" +
                obj.link
                + "'>" +
                obj.name +
                "</a></td><td>" +
                '<button class="fa fa-trash text-danger" onclick="SettingDelFile(' +
                obj.id +
                ', this);"></button>' +
                "</td></tr>";
            $("#ss_filetai .table").append(newRow);
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
        }
    }
}
function SettingDelFile(id, ctr) {
    $.ajax({
        url: '/Progresses/SettingDelFile',
        data: {
            tailieu_id: id
        },
        type: "POST",
        dataType: "JSON",
        success: function (rs) {
            toastr.success("Đã lưu thành công", "Hồ sơ", { timeOut: 5000 });
            $(ctr).parent().parent().remove();
        }
    });
}
function SubmitNhatKyDoVe() {
    $("#createdovenhatky").off('submit').submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            dataType: "JSON",
            data: new FormData(this),
            processData: false,
            contentType: false,
            success: function (data, status) {
                $.ajax({
                    url: '/Doves/ShowNhatKyDoVe',
                    type: 'GET',
                    cache: false,
                    data: { hoso_id: data.hoso_id }
                }).done(function (result) {
                    toastr.success("Đã chuyển thành công", "ĐO VẼ", { timeOut: 5000 });
                    $("#phongdove").html(result);
                });
            },
            error: function (xhr, desc, err) {
            }
        });
    });
}
function ShowInOut(contract_id, type) {
    $("#mdInOutDetail iframe").attr("src", "/InOuts/InOutDetail?contract_id=" + contract_id + "&type=" + type);
    $("#mdInOutDetail").modal("show");
}
function DuyetAllTrongHDById(contract_id) {
    var lstioinct = [];
    $(".byid" + contract_id).each(function () {
        var per = this.id.replace("ioinct", "");
        lstioinct.push(per);
    });
    $.ajax({
        url: '/InOuts/DuyetAll',
        type: "POST",
        dataType: "JSON",
        data: { ids: lstioinct },
        success: function (b) {
            document.location.reload();
        }
    });
}
function parseDate(dateString) {
    if (!dateString) {
        return null; // Or handle the empty/null case as you see fit
    }

    // Split the string by '/'
    var parts = dateString.split('/');

    // Check if we have exactly 3 parts (day, month, year)
    if (parts.length !== 3) {
        return null; // Or throw an error, or return a default value
    }

    // Parse the parts as integers.  parseInt(string, radix) is important!
    var day = parseInt(parts[0], 10);
    var month = parseInt(parts[1], 10);
    var year = parseInt(parts[2], 10);


    // Check if the parsed values are valid numbers.
    if (isNaN(day) || isNaN(month) || isNaN(year)) {
        return null;
    }

    // Months are 0-indexed in JavaScript Date objects, so subtract 1 from the month.
    month = month - 1;

    // Validate date (check for valid month and day for that month)
    if (month < 0 || month > 11) {
        return null;
    }

    var testDate = new Date(year, month, 1);
    if (testDate.getMonth() !== month) {
        return null; // The date is invalid
    }

    if (day < 1 || day > 31) return null; //Basic Check
    testDate = new Date(year, month, day);
    if (testDate.getMonth() !== month) return null; //Check for valid day of month



    // Create a new Date object.  Note: month is 0-indexed (0=Jan, 11=Dec)
    var date = new Date(year, month, day);


    return date;
}

//$(function () {
//    $.validator.methods.date = function (value, element) {
//        return this.optional(element) || moment(value, "DD/MM/YYYY", true).isValid();
//    }
//});
