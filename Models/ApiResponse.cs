// Models/ApiResponse.cs
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NiceHandles.Models // Replace YourProjectName
{
    public class ApiResponse
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<CardData> data { get; set; }
        public string filename { get; set; }
    }
}