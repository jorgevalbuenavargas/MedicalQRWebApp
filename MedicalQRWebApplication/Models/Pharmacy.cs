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
    
    public partial class Pharmacy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pharmacy()
        {
            this.MedicalReceipts = new HashSet<MedicalReceipt>();
        }
    
        public System.Guid id { get; set; }
        public string cuit { get; set; }
        public string company_name { get; set; }
        public string business_name { get; set; }
        public string email { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> creationDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedicalReceipt> MedicalReceipts { get; set; }
    }
}
