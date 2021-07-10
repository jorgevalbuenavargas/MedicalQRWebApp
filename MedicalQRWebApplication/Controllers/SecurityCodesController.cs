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
    public class SecurityCodesController : ApiController
    {
        public HttpResponseMessage Get()
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var foundSecurityCodes = dbContext.SecurityCodes.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, foundSecurityCodes);
            }
        }

        public HttpResponseMessage GetByDoctor(Guid doctorId)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.SecurityCodes.Where(e => e.doctorId == doctorId).ToList();
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Security Codes not found for doctor with ID " + doctorId.ToString());
                }
            }
        }
        public HttpResponseMessage Get(Guid id)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.SecurityCodes.FirstOrDefault(e => e.id == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Security Code with ID " + id.ToString() + " not found");
                }
            }
        }

        public HttpResponseMessage GetNotifySecurityCode(Guid id, String email)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.SecurityCodes.FirstOrDefault(e => e.id == id);
                if (entity != null)
                {
                    //Generar Contenido de Email y Enviar

                    var apiKey = Environment.GetEnvironmentVariable("sendGridKey");
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress(Environment.GetEnvironmentVariable("sendGridEmail"), Environment.GetEnvironmentVariable("sendGridUser"));
                    var subject = "Este es tu Código de Seguridad";
                    var to = new EmailAddress(email, "");
                    var plainTextContent = "Tu código de seguridad vigente es: " + entity.securityNumber;
                    var htmlContent = "<div><p>Estimado(a),</div>" + "<div><p>Tu código de seguridad vigente es: " + entity.securityNumber + "</p></div>" + "<div><p>Saludos</p></div>" + "<div><p>Medical QR</p></div>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    var response = client.SendEmailAsync(msg);

                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Security Code with ID " + id.ToString() + " not found");
                }
            }
        }

        public HttpResponseMessage Post([FromBody] SecurityCode securityCode)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.SecurityCodes.Add(securityCode);
                    dbContext.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.Created, securityCode);
                    message.Headers.Location = new Uri(Request.RequestUri +
                        securityCode.id.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(Guid id, [FromBody] SecurityCode securityCode)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    var entity = dbContext.SecurityCodes.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Security Code with ID " + id.ToString() + " not found to update");
                    }
                    else
                    {
                        entity.securityNumber = securityCode.securityNumber;
                        entity.expirationDate = securityCode.expirationDate;
                        entity.doctorId = securityCode.doctorId;
                        dbContext.SaveChanges();
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
                    var entity = dbContext.SecurityCodes.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Security Code with ID " + id.ToString() + " not found to delete");
                    }
                    else
                    {
                        dbContext.SecurityCodes.Remove(entity);
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
