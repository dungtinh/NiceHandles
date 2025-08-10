namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;

    // Request model cho search
    public class ContractSearchRequest
    {
        public string Term { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 25;
        public string Type { get; set; }
        public int? AddressId { get; set; }
        public int? PartnerId { get; set; }
        public int? ServiceId { get; set; }
        public int? AccountId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Status { get; set; }
        public string SortBy { get; set; } = "name"; // name, date, amount
        public string SortOrder { get; set; } = "asc"; // asc, desc
        public bool LoadFinancialInfo { get; set; } = false; // New property
    }

    // Response model
    public class ContractSearchResponse
    {
        public List<ContractItemDto> Items { get; set; }
        public int TotalCount { get; set; }
        public PaginationInfo Pagination { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }

    // Contract item DTO
    public class ContractItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Text { get; set; } // For Select2 compatibility
        public string DisplayText { get; set; }
        public string Address { get; set; }
        public int AddressId { get; set; }
        public string Service { get; set; }
        public int ServiceId { get; set; }
        public string Partner { get; set; }
        public int PartnerId { get; set; }
        public long Amount { get; set; }
        public string FormattedAmount { get; set; }
        public long Consuming { get; set; }
        public string FormattedConsuming { get; set; }
        public DateTime Time { get; set; }
        public string FormattedDate { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string StatusClass { get; set; }
        public int Type { get; set; }
        public string TypeText { get; set; }

        // Financial summary
        public long TotalThu { get; set; }
        public long TotalChi { get; set; }
        public long Balance { get; set; }

        // Additional metadata
        public string AccountName { get; set; }
        public int Priority { get; set; }
        public string Note { get; set; }

        // HTML for rich display in Select2
        public string Html { get; set; }
    }

    // Pagination info
    public class PaginationInfo
    {
        public bool More { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}