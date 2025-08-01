// Models/AddressEntities.cs
using Newtonsoft.Json;

namespace NiceHandles.Models // Replace YourProjectName
{
    public class AddressEntities
    {
        public string province { get; set; }
        public string district { get; set; }
        public string ward { get; set; }
        public string street { get; set; }
    }
}