// import-data-handler.js
// Script xử lý cho ImportData View

(function ($) {
    'use strict';

    // Global variables
    var ownerIndex = 0;
    var buyerSellerIndex = 0;

    // Initialize on document ready
    $(document).ready(function () {
        initializeForm();
        bindEvents();
        initializeExistingData();
    });

    // Initialize form
    function initializeForm() {
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
    }

    // Initialize existing data indexes
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

        console.log('Initialized - Owners: ' + ownerIndex + ', BuyersSellers: ' + buyerSellerIndex);
    }

    // Bind all events
    function bindEvents() {
        // Load from Infomation button
        $('#btnLoadFromInfomation').on('click', loadFromInfomation);

        // Add owner button
        $('#btnAddOwner').on('click', function () {
            addPersonForm('owner');
        });

        // Add buyer/seller button
        $('#btnAddBuyerSeller').on('click', function () {
            addPersonForm('buyerSeller');
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

    // Load data from Infomation table
    function loadFromInfomation() {
        var hosoId = $('[name="HoSo.id"]').val();

        if (!hosoId) {
            showNotification('error', 'Không tìm thấy ID hồ sơ!');
            return;
        }

        if (!confirm('Load dữ liệu từ bảng Infomation sẽ GHI ĐÈ dữ liệu hiện tại. Bạn có chắc chắn?')) {
            return;
        }

        $.ajax({
            url: '/HoSoes/DupplicateInfomation',
            type: 'GET',
            data: { id: hosoId },
            dataType: 'json',
            beforeSend: function () {
                $('#btnLoadFromInfomation').prop('disabled', true).text('Đang tải...');
            },
            success: function (response) {
                if (response.error) {
                    showNotification('error', response.error);
                } else if (response) {
                    processInfomationData(response);
                    showNotification('success', 'Đã load dữ liệu từ bảng Infomation thành công!');
                } else {
                    showNotification('warning', 'Không tìm thấy dữ liệu trong bảng Infomation!');
                }
            },
            error: function (xhr, status, error) {
                showNotification('error', 'Lỗi khi load dữ liệu: ' + error);
            },
            complete: function () {
                $('#btnLoadFromInfomation').prop('disabled', false)
                    .html('<i class="fa fa-download"></i> Load từ Infomation cũ');
            }
        });
    }
    

    // Add person form
    function addPersonForm(type, data) {
        var template = $('#person-info-template').html();
        var index, prefix, title, container;

        if (type === 'owner') {
            index = ownerIndex++;
            prefix = 'Owners';
            title = 'Chủ sở hữu #' + (index + 1);
            container = '#owners-container';
        } else {
            index = buyerSellerIndex++;
            prefix = 'BuyersOrSellers';
            title = 'Bên liên quan #' + (index + 1);
            container = '#buyers-sellers-container';
        }

        // Replace placeholders
        template = template.replace(/{role}/g, type === 'owner' ? 'Owner' : 'BuyerOrSeller')
            .replace(/{index}/g, index)
            .replace(/{prefix}/g, prefix)
            .replace(/{title}/g, title);

        // Append to container
        $(container).append(template);

        // Fill data if provided
        if (data) {
            fillPersonData(prefix, index, data);
        }
    }

    // Fill person data
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
        if (role === 'Owner') {
            reindexForms('#owners-container', 'Owners', 'Chủ sở hữu');
            ownerIndex = $('#owners-container .person-info-card').length;
        } else {
            reindexForms('#buyers-sellers-container', 'BuyersOrSellers', 'Bên liên quan');
            buyerSellerIndex = $('#buyers-sellers-container .person-info-card').length;
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
        ownerIndex = 0;
        buyerSellerIndex = 0;

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

    // Format date helper
    function formatDate(jsonDate) {
        if (!jsonDate) return '';

        // Handle /Date(123456789)/ format
        if (typeof jsonDate === 'string' && jsonDate.indexOf('/Date(') === 0) {
            var timestamp = parseInt(jsonDate.replace('/Date(', '').replace(')/', ''));
            var date = new Date(timestamp);
            return date.toISOString().split('T')[0];
        }

        // Handle Date object
        if (jsonDate instanceof Date) {
            return jsonDate.toISOString().split('T')[0];
        }

        // Return as is if already formatted
        return jsonDate;
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

})(jQuery);