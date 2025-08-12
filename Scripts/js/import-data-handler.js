// import-data-handler.js
// Script xử lý cho ImportData View - chỉ dùng các bảng mới

(function ($) {
    'use strict';

    // Global variables
    var ownerIndex = 0;
    var buyerSellerIndex = 0;
    var variationPersonIndex = 0;
    var serviceCode = '';

    // Initialize on document ready
    $(document).ready(function () {
        initializeForm();
        bindEvents();
        initializeExistingData();
    });

    // Initialize form
    function initializeForm() {
        // Get service code
        serviceCode = $('#ServiceCode').val() || $('[name="ServiceCode"]').val() || '';

        // Initialize datepickers if available
        if ($.fn.datepicker) {
            $('input[type="date"]').datepicker({
                dateFormat: 'yy-mm-dd',
                changeMonth: true,
                changeYear: true,
                yearRange: '1900:2030'
            });
        }

        // Initialize tooltips if Bootstrap is available
        if ($.fn.tooltip) {
            $('[data-toggle="tooltip"]').tooltip();
        }

        // Initialize Select2 if available
        if ($.fn.select2) {
            $('.select2').select2({
                placeholder: "-- Chọn địa chỉ --",
                allowClear: true,
                width: '100%'
            });
        }
    }

    // Initialize existing data indexes from server-rendered forms
    function initializeExistingData() {
        // Count existing owners
        $('#owners-container .person-info-card').each(function () {
            var idx = parseInt($(this).data('index'));
            if (idx >= ownerIndex) {
                ownerIndex = idx + 1;
            }
        });

        // Count existing buyers/sellers  
        $('#buyers-sellers-container .person-info-card').each(function () {
            var idx = parseInt($(this).data('index'));
            if (idx >= buyerSellerIndex) {
                buyerSellerIndex = idx + 1;
            }
        });

        // Count existing variation persons
        $('#variation-persons-container .person-info-card').each(function () {
            var idx = parseInt($(this).data('index'));
            if (idx >= variationPersonIndex) {
                variationPersonIndex = idx + 1;
            }
        });

        console.log('Initialized - Owners: ' + ownerIndex + ', BuyersSellers: ' + buyerSellerIndex + ', VariationPersons: ' + variationPersonIndex);
    }

    // Bind all events
    function bindEvents() {
        // Add owner button
        $('#btnAddOwner').on('click', function () {
            addPersonForm('owner');
        });

        // Add buyer/seller button
        $('#btnAddBuyerSeller').on('click', function () {
            addPersonForm('buyerSeller');
        });

        // Add variation person button (chỉ khi service code là chuyennhuong)
        $('#btnAddVariationPerson').on('click', function () {
            addPersonForm('variationPerson');
        });

        // Remove person handler
        $(document).on('click', '.remove-person', function () {
            if (confirm('Bạn có chắc muốn xóa người này?')) {
                removePersonForm($(this));
            }
        });

        // Clear all button
        $('#btnClearAll').on('click', function () {
            if (confirm('Bạn có chắc muốn xóa TẤT CẢ dữ liệu đã nhập?')) {
                clearAllForms();
            }
        });

        // Form submit validation
        $('#importDataForm').on('submit', function (e) {
            if (!validateForm()) {
                e.preventDefault();
                return false;
            }
        });

        // Tab navigation
        $('.nav-tabs a').on('click', function (e) {
            e.preventDefault();
            $(this).tab('show');
        });
    }

    // Add person form dynamically
    function addPersonForm(type, data) {
        var template = getPersonTemplate(type);
        var index, prefix, title, container;

        if (type === 'owner') {
            index = ownerIndex++;
            prefix = 'Owners';
            title = 'Chủ sở hữu #' + (index + 1);
            container = '#owners-container';
        } else if (type === 'buyerSeller') {
            index = buyerSellerIndex++;
            prefix = 'BuyersOrSellers';
            title = 'Bên liên quan #' + (index + 1);
            container = '#buyers-sellers-container';
        } else if (type === 'variationPerson') {
            index = variationPersonIndex++;
            prefix = 'VariationPersons';
            title = 'Người nhận chuyển nhượng #' + (index + 1);
            container = '#variation-persons-container';
        }

        // Replace placeholders
        template = template.replace(/{role}/g, type)
            .replace(/{index}/g, index)
            .replace(/{prefix}/g, prefix)
            .replace(/{title}/g, title);

        // Append to container
        $(container).append(template);

        // Initialize Select2 for new form
        $(container).find('.person-info-card:last').find('.select2').select2({
            placeholder: "-- Chọn địa chỉ --",
            allowClear: true,
            width: '100%'
        });

        // Fill data if provided
        if (data) {
            fillPersonData(prefix, index, data);
        }
    }

    // Get person form template
    function getPersonTemplate(type) {
        var template = `
            <div class="person-info-card" data-role="{role}" data-index="{index}">
                <div class="card-header">
                    <span>{title}</span>
                    <button type="button" class="btn btn-sm btn-danger remove-person" data-index="{index}">
                        <i class="fa fa-times"></i> Xóa
                    </button>
                </div>
                <div class="card-body">
                    <div class="form-row">
                        <div class="form-group col-md-4">
                            <label>Loại giấy tờ</label>
                            <input name="{prefix}[{index}].DocumentType" class="form-control" />
                        </div>
                        <div class="form-group col-md-4">
                            <label>Số giấy tờ</label>
                            <input name="{prefix}[{index}].DocumentNumber" class="form-control" />
                        </div>
                        <div class="form-group col-md-4">
                            <label>Họ và tên <span class="required-field">*</span></label>
                            <input name="{prefix}[{index}].FullName" class="form-control" required />
                        </div>
                        <div class="form-group col-md-4">
                            <label>Ngày sinh</label>
                            <input name="{prefix}[{index}].BirthDate" type="date" class="form-control" />
                        </div>
                        <div class="form-group col-md-4">
                            <label>Giới tính</label>
                            <select name="{prefix}[{index}].Gender" class="form-control">
                                <option value="">-- Chọn --</option>
                                <option value="Nam">Nam</option>
                                <option value="Nữ">Nữ</option>
                            </select>
                        </div>
                        <div class="form-group col-md-4">
                            <label>Địa chỉ</label>
                            <select name="{prefix}[{index}].address_id" class="form-control select2">
                                <option value="">-- Chọn địa chỉ --</option>
                            </select>
                        </div>
                        <div class="form-group col-md-4">
                            <label>Ngày cấp GT</label>
                            <input name="{prefix}[{index}].IssueDate" type="date" class="form-control" />
                        </div>
                        <div class="form-group col-md-4">
                            <label>Nơi cấp GT</label>
                            <input name="{prefix}[{index}].Issuer" class="form-control" />
                        </div>
                        <div class="form-group col-md-4">
                            <label>Mã số thuế</label>
                            <input name="{prefix}[{index}].TaxCode" class="form-control" />
                        </div>`;

        // Add additional fields for variation persons
        if (type === 'variationPerson') {
            template += `
                        <div class="form-group col-md-4">
                            <label>Ngày mất</label>
                            <input name="{prefix}[{index}].DeathDate" type="date" class="form-control" />
                        </div>
                        <div class="form-group col-md-4">
                            <label>Giấy chứng tử</label>
                            <input name="{prefix}[{index}].DeathDocument" class="form-control" />
                        </div>
                        <div class="form-group col-md-4">
                            <label>ID người thừa kế</label>
                            <input name="{prefix}[{index}].HeirId" type="number" class="form-control" />
                        </div>`;
        }

        template += `
                    </div>
                </div>
            </div>`;

        return template;
    }

    // Fill person data into form
    function fillPersonData(prefix, index, data) {
        $.each(data, function (key, value) {
            var selector = '[name="' + prefix + '[' + index + '].' + key + '"]';
            $(selector).val(value);
        });
    }

    // Remove person form
    function removePersonForm(button) {
        var card = button.closest('.person-info-card');
        var container = card.parent();
        var role = card.data('role');

        card.remove();

        // Reindex remaining forms
        if (role === 'owner') {
            reindexForms('#owners-container', 'Owners', 'Chủ sở hữu');
            ownerIndex = $('#owners-container .person-info-card').length;
        } else if (role === 'buyerSeller') {
            reindexForms('#buyers-sellers-container', 'BuyersOrSellers', 'Bên liên quan');
            buyerSellerIndex = $('#buyers-sellers-container .person-info-card').length;
        } else if (role === 'variationPerson') {
            reindexForms('#variation-persons-container', 'VariationPersons', 'Người nhận chuyển nhượng');
            variationPersonIndex = $('#variation-persons-container .person-info-card').length;
        }
    }

    // Reindex forms after deletion
    function reindexForms(container, prefix, titlePrefix) {
        $(container + ' .person-info-card').each(function (idx) {
            $(this).attr('data-index', idx);
            $(this).find('.card-header span').text(titlePrefix + ' #' + (idx + 1));
            $(this).find('.remove-person').attr('data-index', idx);

            // Update all input names
            $(this).find('input, select, textarea').each(function () {
                var name = $(this).attr('name');
                if (name) {
                    var newName = name.replace(/\[\d+\]/, '[' + idx + ']');
                    $(this).attr('name', newName);
                }
            });
        });
    }

    // Clear all forms
    function clearAllForms() {
        $('#owners-container').empty();
        $('#buyers-sellers-container').empty();
        $('#variation-persons-container').empty();
        ownerIndex = 0;
        buyerSellerIndex = 0;
        variationPersonIndex = 0;

        // Add one empty owner form
        addPersonForm('owner');

        // Clear LandParcel
        $('[name^="LandParcel."]').val('');

        // Clear VariationInfo
        $('[name^="VariationInfo."]').val('');

        showNotification('info', 'Đã xóa tất cả dữ liệu!');
    }

    // Validate form
    function validateForm() {
        var errors = [];

        // Check at least one owner
        if ($('#owners-container .person-info-card').length === 0) {
            errors.push('Phải có ít nhất một chủ sở hữu');
        }

        // Check required fields
        $('input[required]:visible').each(function () {
            if (!$(this).val()) {
                var label = $(this).closest('.form-group').find('label').text().replace('*', '').trim();
                var card = $(this).closest('.person-info-card');
                var cardTitle = card.find('.card-header span').text();
                errors.push(cardTitle + ' - ' + label + ' là bắt buộc');
            }
        });

        if (errors.length > 0) {
            showNotification('error', 'Vui lòng kiểm tra:\n' + errors.join('\n'));
            return false;
        }

        return true;
    }

    // Load addresses for select2 dropdown
    function loadAddresses() {
        $.ajax({
            url: '/api/addresses', // Update với endpoint thực tế
            type: 'GET',
            dataType: 'json',
            success: function (addresses) {
                var options = '<option value="">-- Chọn địa chỉ --</option>';
                $.each(addresses, function (index, address) {
                    options += '<option value="' + address.id + '">' + address.name + '</option>';
                });

                // Update all address dropdowns
                $('.select2[name$=".address_id"]').each(function () {
                    var currentValue = $(this).val();
                    $(this).html(options).val(currentValue);
                });
            }
        });
    }

    // Show notification
    function showNotification(type, message) {
        var alertClass = type === 'success' ? 'alert-success' :
            type === 'warning' ? 'alert-warning' :
                type === 'info' ? 'alert-info' : 'alert-danger';

        var alert = $('<div class="alert ' + alertClass + ' alert-dismissible fade in" role="alert">' +
            '<button type="button" class="close" data-dismiss="alert"><span>&times;</span></button>' +
            message.replace(/\n/g, '<br>') +
            '</div>');

        $('#notification-area').html(alert);

        // Auto hide after 5 seconds
        setTimeout(function () {
            alert.fadeOut(function () {
                alert.remove();
            });
        }, 5000);
    }

    // Export functions for external use
    window.ImportDataHandler = {
        addPersonForm: addPersonForm,
        clearAllForms: clearAllForms,
        validateForm: validateForm,
        loadAddresses: loadAddresses
    };

})(jQuery);