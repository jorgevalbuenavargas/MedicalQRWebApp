//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MedicalQRWebApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MedicalReceipt
    {
        public System.Guid id { get; set; }
        public System.DateTime scanDate { get; set; }
        public string validationResult { get; set; }
        public System.Guid pharmacyId { get; set; }
        public string uicId { get; set; }
        public string securityCodeId { get; set; }
    
        public virtual Pharmacy Pharmacy { get; set; }
    }
}
