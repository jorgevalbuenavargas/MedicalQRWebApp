
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------


namespace MedicalQRWebApplication.Models
{

using System;
    using System.Collections.Generic;
    
public partial class UniqueIdentifierCode
{

    public System.Guid id { get; set; }

    public string status { get; set; }

    public System.DateTime creationDate { get; set; }

    public System.Guid doctorId { get; set; }

    public Nullable<System.DateTime> modificationDate { get; set; }

    public string image { get; set; }



    public virtual Doctor Doctor { get; set; }

}

}
