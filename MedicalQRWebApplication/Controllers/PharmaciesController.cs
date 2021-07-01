using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using MedicalQRWebApplication.Models;

using SendGrid;
using SendGrid.Helpers.Mail;

namespace MedicalQRWebApplication.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PharmaciesController : ApiController
    {
        
        public HttpResponseMessage Get()
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var foundPharmacies = dbContext.Pharmacies.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, foundPharmacies);
            }
        }
        public HttpResponseMessage Get(Guid id)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.Pharmacies.FirstOrDefault(e => e.id == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Pharmacy with ID " + id.ToString() + " not found");
                }
            }
        }

        public HttpResponseMessage Post([FromBody] Pharmacy pharmacy)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.Pharmacies.Add(pharmacy);
                    dbContext.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.Created, pharmacy);
                    message.Headers.Location = new Uri(Request.RequestUri +
                        pharmacy.id.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(Guid id, [FromBody] Pharmacy pharmacy)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    var entity = dbContext.Pharmacies.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Pharmacy with ID " + id.ToString() + " not found to update");
                    }
                    else
                    {
                        entity.cuit = pharmacy.cuit;
                        entity.company_name = pharmacy.company_name;
                        entity.business_name = pharmacy.business_name;
                        entity.email = pharmacy.email;
                        entity.Status = pharmacy.Status;
                        entity.creationDate = pharmacy.creationDate;
                        dbContext.SaveChanges();

                        var apiKey = Environment.GetEnvironmentVariable("sendGridKey");
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(Environment.GetEnvironmentVariable("sendGridEmail"), Environment.GetEnvironmentVariable("sendGridUser"));
                        var subject = "";
                        var plainTextContent = "";
                        var htmlContent = "";
                        if (pharmacy.Status.Equals("Activo"))
                        {
                            subject = "Tu cuenta ha sido habilitada";
                            plainTextContent = "Te informamos que tu cuenta ha sido habilitada. Si tienes inconvenientes con el inicio de sesión, puedes contactarnos a la siguiente dirección de correo electrónico: admin@medicalqr.com.ar";
                            htmlContent = "<div><p>Estimado(a),</div>" + "<div><p>Te informamos que tu cuenta ha sido habilitada. Si tienes inconvenientes con el inicio de sesión, puedes contactarnos a la siguiente dirección de correo electrónico: admin@medicalqr.com.ar</p></div>" + "<div><p>Saludos</p></div>" + "<div><p>Medical QR</p></div>";
                        }
                        else
                        {
                            subject = "Tu cuenta ha sido deshabilitada";
                            plainTextContent = "Te informamos que tu cuenta ha sido deshabilitada. Para mayor información puedes contactarte a la siguiente dirección de correo electrónico: admin@medicalqr.com.ar";
                            htmlContent = "<div><p>Estimado(a),</div>" + "<div><p>Te informamos que tu cuenta ha sido deshabilitada. Para mayor información puedes contactarte a la siguiente dirección de correo electrónico: admin@medicalqr.com.ar</p></div>" + "<div><p>Saludos</p></div>" + "<div><p>Medical QR</p></div>";
                        }
                        var to = new EmailAddress(pharmacy.email, "");
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = client.SendEmailAsync(msg);

                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Delete(Guid id)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    var entity = dbContext.Pharmacies.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Pharmacy with ID " + id.ToString() + " not found to delete");
                    }
                    else
                    {
                        dbContext.Pharmacies.Remove(entity);
                        dbContext.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
