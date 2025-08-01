namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GroupMember_Account
    {
        public int id { get; set; }

        public int account_id { get; set; }

        public int groupmember_id { get; set; }
    }
}
