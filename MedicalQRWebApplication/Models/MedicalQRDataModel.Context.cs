﻿

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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class MedicalQRDBContext : DbContext
{
    public MedicalQRDBContext()
        : base("name=MedicalQRDBContext")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Pharmacy> Pharmacies { get; set; }

    public virtual DbSet<SecurityCode> SecurityCodes { get; set; }

    public virtual DbSet<UniqueIdentifierCode> UniqueIdentifierCodes { get; set; }

    public virtual DbSet<MedicalReceipt> MedicalReceipts { get; set; }

}

}

