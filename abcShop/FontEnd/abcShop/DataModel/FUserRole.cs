//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace abcShop.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class FUserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string IdentityUser_Id { get; set; }
    
        public virtual FRole FRole { get; set; }
        public virtual FUser FUser { get; set; }
    }
}
