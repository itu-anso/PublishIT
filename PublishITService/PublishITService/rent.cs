//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PublishITService
{
    using System;
    using System.Collections.Generic;
    
    public partial class rent
    {
        public int rent_id { get; set; }
        public int user_id { get; set; }
        public int media_id { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
    
        public virtual media media { get; set; }
        public virtual user user { get; set; }
    }
}