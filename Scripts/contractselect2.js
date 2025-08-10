
// Contract Search Manager - Quản lý tìm kiếm hợp đồng
    var ContractSearchManager = (function() {
        'use strict';

    // Private variables
    var _config = {
        searchUrl: '/Contracts/GetContractAdvanced',
    pageSize: 25,
    delay: 250,
    minimumInputLength: 0,
    cacheEnabled: true
    };

    // Cache manager
    var _cache = {
        data: { },
    set: function(key, value, ttl) {
            var expiry = new Date().getTime() + (ttl || 300000); // Default 5 minutes
    this.data[key] = {
        value: value,
    expiry: expiry
            };
        },
    get: function(key) {
            var item = this.data[key];
    if (!item) return null;
            
            if (new Date().getTime() > item.expiry) {
        delete this.data[key];
    return null;
            }

    return item.value;
        },
    clear: function() {
        this.data = {};
        }
    };

    // Initialize Select2 with advanced options
    function initSelect2(selector, options) {
        var defaultOptions = {
        ajax: {
        url: _config.searchUrl,
    dataType: 'json',
    delay: _config.delay,
    data: function(params) {
                    return buildSearchRequest(params, options);
                },
    processResults: function(data, params) {
                    return processSearchResults(data, params);
                },
    transport: function(params, success, failure) {
                    // Custom transport with caching
                    if (_config.cacheEnabled) {
                        var cacheKey = JSON.stringify(params.data);
    var cachedData = _cache.get(cacheKey);

    if (cachedData) {
        success(cachedData);
    return;
                        }
                    }

    // Make AJAX request
    var $request = $.ajax(params);

    $request.then(function(data) {
                        if (_config.cacheEnabled) {
        _cache.set(JSON.stringify(params.data), data);
                        }
    success(data);
                    });

    $request.fail(failure);

    return $request;
                }
            },
    placeholder: options.placeholder || "-- Chọn hợp đồng --",
    allowClear: options.allowClear !== false,
    minimumInputLength: options.minimumInputLength || _config.minimumInputLength,
    language: {
        inputTooShort: function() {
                    return "Nhập để tìm kiếm...";
                },
    noResults: function() {
                    return "Không tìm thấy kết quả";
                },
    searching: function() {
                    return "Đang tìm kiếm...";
                },
    loadingMore: function() {
                    return "Đang tải thêm...";
                }
            },
    escapeMarkup: function(markup) { return markup; },
    templateResult: options.templateResult || formatContractResult,
    templateSelection: options.templateSelection || formatContractSelection,
    width: '100%',
    theme: options.theme || 'bootstrap'
        };

    // Merge với custom options
    var finalOptions = $.extend(true, { }, defaultOptions, options.select2Options || { });

    // Initialize
    var $element = $(selector);
    $element.select2(finalOptions);

    // Bind additional events
    bindEvents($element, options);

    return $element;
    }

    // Build search request
    function buildSearchRequest(params, options) {
        var request = {
        Term: params.term || '',
    Page: params.page || 0,
    PageSize: _config.pageSize,
    Type: options.type || null,
    AddressId: options.addressId || null,
    PartnerId: options.partnerId || null,
    ServiceId: options.serviceId || null,
    AccountId: options.accountId || null,
    Status: options.status || null,
    SortBy: options.sortBy || 'name',
    SortOrder: options.sortOrder || 'asc'
        };

    // Add dynamic filters
    if (options.getFilters && typeof options.getFilters === 'function') {
            var dynamicFilters = options.getFilters();
    request = $.extend(request, dynamicFilters);
        }

    return request;
    }

    // Process search results
    function processSearchResults(data, params) {
        params.page = params.page || 0;

    if (!data.Success) {
        console.error('Contract search error:', data.Message);
    return {
        results: [],
    pagination: {more: false }
            };
        }

    // Update UI with pagination info if needed
    if (data.Pagination) {
        updatePaginationInfo(data.Pagination);
        }

    return {
        results: data.Items || data.items,
    pagination: {
        more: data.Pagination ? data.Pagination.More : false
            }
        };
    }

    // Format contract for display
    function formatContractResult(contract) {
        if (!contract || contract.loading) {
            return contract.text || 'Đang tải...';
        }

    // If HTML is provided, use it
    if (contract.Html) {
            return contract.Html;
        }

    // Build custom HTML
    var html = '<div class="select2-contract-result">';

        // Priority indicator
        if (contract.Priority > 0) {
            html += '<i class="fa fa-star text-warning" title="Ưu tiên"></i> ';
        }

        // Main info
        html += '<div class="contract-main-info">';
            html += '<strong>' + (contract.Name || contract.name || '') + '</strong>';

            // Status badge
            if (contract.StatusText) {
                html += ' <span class="label label-' + (contract.StatusClass || 'default') + '">' + contract.StatusText + '</span>';
        }
            html += '</div>';

        // Details
        html += '<div class="contract-sub-info">';
            html += '<small class="text-muted">';

                if (contract.Code) {
                    html += '<span class="contract-code">[' + contract.Code + ']</span> ';
        }

                if (contract.Address || contract.text) {
                    html += '<i class="fa fa-map-marker"></i> ' + (contract.Address || contract.text) + ' ';
        }

                if (contract.Service) {
                    html += '| <i class="fa fa-briefcase"></i> ' + contract.Service + ' ';
        }

                if (contract.Partner) {
                    html += '| <i class="fa fa-handshake-o"></i> ' + contract.Partner + ' ';
        }

                html += '</small>';
            html += '</div>';

        // Financial info
        if (contract.FormattedAmount || contract.Balance !== undefined) {
            html += '<div class="contract-financial">';
        html += '<small>';

            if (contract.FormattedAmount) {
                html += '<span class="text-primary"><i class="fa fa-coins"></i> ' + contract.FormattedAmount + '</span> ';
            }

            if (contract.Balance !== undefined) {
                var balanceClass = contract.Balance >= 0 ? 'text-success' : 'text-danger';
            var balanceFormatted = formatMoney(contract.Balance);
            html += '<span class="' + balanceClass + '">| Còn lại: ' + balanceFormatted + '</span>';
            }

            html += '</small>';
        html += '</div>';
        }

    // Date
    if (contract.FormattedDate) {
        html += '<div class="contract-date">';
    html += '<small class="text-muted"><i class="fa fa-calendar"></i> ' + contract.FormattedDate + '</small>';
    html += '</div>';
        }

html += '</div>';

return html;
    }

// Format selected contract
function formatContractSelection(contract) {
    if (!contract || !contract.id) {
        return contract.text || '';
    }

    // Build display text
    var text = contract.Name || contract.name || contract.text || '';

    if (contract.Code) {
        text = '[' + contract.Code + '] ' + text;
    }

    if (contract.Address) {
        text += ' - ' + contract.Address;
    }

    return text;
}

// Bind additional events
function bindEvents($element, options) {
    // On select event
    $element.on('select2:select', function (e) {
        var data = e.params.data;

        if (options.onSelect && typeof options.onSelect === 'function') {
            options.onSelect(data);
        }

        // Load contract financial info if configured
        if (options.loadFinancialInfo) {
            loadContractFinancialInfo(data.Id || data.id);
        }
    });

    // On clear event
    $element.on('select2:clear', function (e) {
        if (options.onClear && typeof options.onClear === 'function') {
            options.onClear();
        }
    });

    // On open event
    $element.on('select2:open', function (e) {
        // Add custom classes or behaviors
        $('.select2-dropdown').addClass('contract-dropdown');
    });
}

// Load contract financial info
function loadContractFinancialInfo(contractId) {
    if (!contractId) return;

    // Show loading indicator
    showFinancialLoading();

    $.ajax({
        url: '/InOuts/GetContractFinancialInfo',
        type: 'GET',
        data: { contractId: contractId },
        dataType: 'json',
        success: function (response) {
            if (response && response.success) {
                displayFinancialInfo(response);
            } else {
                showFinancialError(response ? response.message : 'Lỗi không xác định');
            }
        },
        error: function (xhr, status, error) {
            showFinancialError('Lỗi kết nối: ' + error);
        }
    });
}

// Display financial info
function displayFinancialInfo(data) {
    // Implement based on your UI structure
    if ($('#contractFinancialInfo').length) {
        $('#contractFinancialInfo').show();
        updateContractFinancialDisplay(data);
    }
}

// Show loading for financial info
function showFinancialLoading() {
    if ($('#contractContent').length) {
        $('#contractContent').html('<div class="text-center"><i class="fa fa-spinner fa-spin"></i> Đang tải thông tin...</div>');
    }
}

// Show error for financial info
function showFinancialError(message) {
    if ($('#contractContent').length) {
        $('#contractContent').html(
            '<div class="alert alert-danger">' +
            '<i class="fa fa-exclamation-triangle"></i> ' + message +
            '</div>'
        );
    }
}

// Update pagination info in UI
function updatePaginationInfo(pagination) {
    if ($('#contractPaginationInfo').length) {
        var text = 'Hiển thị ' + pagination.From + ' - ' + pagination.To +
            ' trong tổng số ' + pagination.TotalRecords + ' hợp đồng';
        $('#contractPaginationInfo').text(text);
    }
}

// Utility: Format money
function formatMoney(amount) {
    try {
        return new Intl.NumberFormat('vi-VN').format(amount || 0) + ' VNĐ';
    } catch (e) {
        return (amount || 0).toLocaleString() + ' VNĐ';
    }
}

// Public API
return {
    init: initSelect2,
    clearCache: function () {
        _cache.clear();
    },
    setConfig: function (config) {
        _config = $.extend(_config, config);
    },
    loadFinancialInfo: loadContractFinancialInfo,
    formatMoney: formatMoney
};
}) ();

// ===== USAGE EXAMPLES =====

$(document).ready(function () {
    // Example 1: Basic usage for Thu tab
    ContractSearchManager.init('#thuContractId', {
        placeholder: "-- Chọn hợp đồng để thu --",
        type: 'normal',
        onSelect: function (contract) {
            console.log('Selected contract for Thu:', contract);
            // Update UI or load related data
        }
    });

    // Example 2: Advanced usage for Chi tab with financial info
    ContractSearchManager.init('#chiContractId', {
        placeholder: "-- Chọn hợp đồng để chi --",
        type: 'normal',
        loadFinancialInfo: true,
        onSelect: function (contract) {
            console.log('Selected contract for Chi:', contract);

            // Update categories based on contract
            if (contract.Id || contract.id) {
                loadCategoriesByContract(contract.Id || contract.id);
            }
        },
        onClear: function () {
            $('#contractFinancialInfo').slideUp();
            loadCategories(1, '#chiCategoryId', 1); // Reset to default categories
        }
    });

    // Example 3: Custom filters with dynamic values
    ContractSearchManager.init('#advancedContractSearch', {
        placeholder: "Tìm kiếm hợp đồng...",
        minimumInputLength: 2,
        getFilters: function () {
            // Get dynamic filter values from UI
            return {
                AddressId: $('#filterAddress').val(),
                PartnerId: $('#filterPartner').val(),
                ServiceId: $('#filterService').val(),
                FromDate: $('#filterFromDate').val(),
                ToDate: $('#filterToDate').val(),
                Status: $('#filterStatus').val()
            };
        },
        templateResult: function (contract) {
            // Custom template
            if (!contract.id) return contract.text;

            return $('<div>')
                .addClass('custom-contract-item')
                .append($('<div>').addClass('contract-name').text(contract.Name))
                .append($('<div>').addClass('contract-info')
                    .append($('<span>').text(contract.Service))
                    .append($('<span>').text(' | '))
                    .append($('<span>').text(contract.FormattedAmount))
                );
        },
        onSelect: function (contract) {
            // Custom action
            window.location.href = '/Contracts/Details/' + contract.Id;
        }
    });

    // Example 4: Multiple select for batch operations
    ContractSearchManager.init('#batchContractSelect', {
        placeholder: "Chọn nhiều hợp đồng...",
        select2Options: {
            multiple: true,
            maximumSelectionLength: 10
        },
        onSelect: function (contract) {
            updateBatchOperationUI();
        }
    });
});

// ===== INTEGRATION WITH EXISTING CODE =====

// Update the existing loadCategoriesByContract function
function loadCategoriesByContract(contractId) {
    var parentId = 1; // XCategory.eParent.KhachHang

    $.ajax({
        url: '/Categories/FillCategoryContract',
        type: "GET",
        dataType: "JSON",
        data: {
            type: 1, // XCategory.eType.Chi
            parent_id: parentId
        },
        success: function (categories) {
            $('#chiCategoryId').html('<option value="">-- Chọn loại chi --</option>');
            $.each(categories, function (i, cate) {
                var bgColor = cate.duyet == 0 ? 'style="background-color:#d4edda;"' : '';
                $('#chiCategoryId').append($('<option ' + bgColor + '></option>').val(cate.id).html(cate.name));
            });
        },
        error: function (xhr, status, error) {
            console.log('Error loading categories:', error);
            $('#chiCategoryId').html('<option value="">Lỗi tải dữ liệu</option>');
        }
    });
}

// Update the existing updateContractFinancialDisplay function
function updateContractFinancialDisplay(data) {
    if (!data || !data.contractInfo) return;

    const contract = data.contractInfo;
    const summary = data.summary;

    // Update contract name
    $('#contractName').text(contract.name || 'N/A');

    // Build HTML content
    var htmlContent = `
        <div class="financial-summary">
            <div class="row">
                <div class="col-md-6">
                    <div class="info-box bg-aqua">
                        <span class="info-box-icon"><i class="fa fa-arrow-down"></i></span>
                        <div class="info-box-content">
                            <span class="info-box-text">Tổng Thu</span>
                            <span class="info-box-number">${ContractSearchManager.formatMoney(summary.totalThu)}</span>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="info-box bg-red">
                        <span class="info-box-icon"><i class="fa fa-arrow-up"></i></span>
                        <div class="info-box-content">
                            <span class="info-box-text">Tổng Chi</span>
                            <span class="info-box-number">${ContractSearchManager.formatMoney(summary.totalChi)}</span>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="row">
                <div class="col-md-12">
                    <div class="info-box bg-green">
                        <span class="info-box-icon"><i class="fa fa-balance-scale"></i></span>
                        <div class="info-box-content">
                            <span class="info-box-text">Còn Lại</span>
                            <span class="info-box-number">${ContractSearchManager.formatMoney(summary.conLai)}</span>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="chi-details">
                <h4>Chi tiết các khoản chi</h4>
                <table class="table table-bordered table-striped">
                    <thead>
                        <tr>
                            <th>Loại chi</th>
                            <th class="text-right">Dự kiến</th>
                            <th class="text-right">Đã chi</th>
                            <th class="text-right">Còn lại</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><i class="fa fa-users"></i> Hoa hồng đối tác</td>
                            <td class="text-right">${ContractSearchManager.formatMoney(contract.outrose)}</td>
                            <td class="text-right text-danger">${ContractSearchManager.formatMoney(summary.chiHoaHongDoiTac)}</td>
                            <td class="text-right">${ContractSearchManager.formatMoney(contract.outrose - summary.chiHoaHongDoiTac)}</td>
                        </tr>
                        <tr>
                            <td><i class="fa fa-user"></i> Hoa hồng nhân viên</td>
                            <td class="text-right">${ContractSearchManager.formatMoney(contract.rose)}</td>
                            <td class="text-right text-danger">${ContractSearchManager.formatMoney(summary.chiHoaHongNhanVien)}</td>
                            <td class="text-right">${ContractSearchManager.formatMoney(contract.rose - summary.chiHoaHongNhanVien)}</td>
                        </tr>
                        <tr>
                            <td><i class="fa fa-cogs"></i> ĐM thực hiện</td>
                            <td class="text-right">${ContractSearchManager.formatMoney(contract.remunerate)}</td>
                            <td class="text-right text-danger">${ContractSearchManager.formatMoney(summary.dmThucHien)}</td>
                            <td class="text-right">${ContractSearchManager.formatMoney(contract.remunerate - summary.dmThucHien)}</td>
                        </tr>
                        <tr>
                            <td><i class="fa fa-map"></i> Tiền đo vẽ</td>
                            <td class="text-right">${ContractSearchManager.formatMoney(contract.dove)}</td>
                            <td class="text-right text-danger">${ContractSearchManager.formatMoney(summary.tienDoVe)}</td>
                            <td class="text-right">${ContractSearchManager.formatMoney(contract.dove - summary.tienDoVe)}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    `;

    // Add recent transactions if available
    if (data.transactions && data.transactions.length > 0) {
        htmlContent += `
            <div class="recent-transactions">
                <h4>Giao dịch gần nhất</h4>
                <div class="table-responsive">
                    <table class="table table-hover">
                        <thead>
                            <tr>
                                <th>Thời gian</th>
                                <th>Loại</th>
                                <th>Danh mục</th>
                                <th class="text-right">Số tiền</th>
                                <th>Trạng thái</th>
                            </tr>
                        </thead>
                        <tbody>
        `;

        data.transactions.forEach(function (trans) {
            const typeClass = trans.type === 0 ? 'success' : 'danger';
            const typeSign = trans.type === 0 ? '+' : '-';
            const statusClass = trans.status === 0 ? 'warning' :
                trans.status === 1 ? 'info' : 'success';

            htmlContent += `
                <tr>
                    <td>${formatDateTime(trans.time)}</td>
                    <td><span class="label label-${typeClass}">${trans.typeName}</span></td>
                    <td>${trans.categoryName}</td>
                    <td class="text-right text-${typeClass}">${typeSign}${ContractSearchManager.formatMoney(trans.amount)}</td>
                    <td><span class="label label-${statusClass}">${trans.statusName}</span></td>
                </tr>
            `;
        });

        htmlContent += `
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    }

    // Update the content
    $('#contractContent').html(htmlContent);
    $('#contractFinancialInfo').slideDown();
}

// Utility function for date formatting
function formatDateTime(dateString) {
    try {
        const date = new Date(dateString);
        const dateOptions = { day: '2-digit', month: '2-digit', year: 'numeric' };
        const timeOptions = { hour: '2-digit', minute: '2-digit' };

        return date.toLocaleDateString('vi-VN', dateOptions) + ' ' +
            date.toLocaleTimeString('vi-VN', timeOptions);
    } catch (e) {
        return dateString;
    }
}
