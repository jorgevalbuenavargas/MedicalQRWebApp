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
    public class DoctorsController : ApiController
    {
        public HttpResponseMessage Get()
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var foundDoctors = dbContext.Doctors.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, foundDoctors);
            }
        }
        public HttpResponseMessage Get(Guid id)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.Doctors.FirstOrDefault(e => e.id == id);
                if (entity != null)
                {                    
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Doctor with ID " + id.ToString() + " not found");
                }
            }
        }

        public HttpResponseMessage GetByProvider(String providerId)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.Doctors.Where(e => e.GmailID == providerId || e.FacebookID == providerId).ToList();
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "User not found with ID " + providerId);
                }
            }
        }

        public HttpResponseMessage Post([FromBody] Doctor doctor)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.Doctors.Add(doctor);
                    dbContext.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.Created, doctor);
                    message.Headers.Location = new Uri(Request.RequestUri +
                        doctor.id.ToString());

                    dbContext.Configuration.ProxyCreationEnabled = false;
                    var foundAdmins = dbContext.Admins.ToList();
                    Request.CreateResponse(HttpStatusCode.OK, foundAdmins);
                    foreach (var admin in foundAdmins)
                    {
                        var apiKey = Environment.GetEnvironmentVariable("sendGridKey");
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(Environment.GetEnvironmentVariable("sendGridEmail"), Environment.GetEnvironmentVariable("sendGridUser"));
                        var subject = "Nuevo Profesional de la Salud";
                        var plainTextContent = "Un nuevo Profesional de la Salud (Matricula:" + doctor.medicalLicense + ") ha sido registrado. Valídalo tan pronto como sea posible para que pueda empezar a utilizar la aplicación.";
                        var htmlContent = "<div><p>Estimado(a),</div>" + "<div><p>Un nuevo Profesional de la Salud (Matricula:" + doctor.medicalLicense + ") ha sido registrado. Valídalo tan pronto como sea posible para que pueda empezar a utilizar la aplicación.</p></div>" + "<div><p>Saludos</p></div>" + "<div><p>Medical QR</p></div>";
                        
                        var to = new EmailAddress(admin.email, "");
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = client.SendEmailAsync(msg);
                    }

                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(Guid id, [FromBody] Doctor doctor)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    var entity = dbContext.Doctors.FirstOrDefault(e => e.id == id);
                    var oldStatus = entity.Status; 
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Doctor with ID " + id.ToString() + " not found to update");
                    }
                    else
                    {
                        entity.name = doctor.name;
                        entity.lastName = doctor.lastName;
                        entity.medicalLicense = doctor.medicalLicense;
                        entity.email = doctor.email;
                        entity.GmailID = doctor.GmailID;
                        entity.FacebookID = doctor.FacebookID;

                        entity.Status = doctor.Status;
                        entity.creationDate = doctor.creationDate;
                        dbContext.SaveChanges();

                        var apiKey = Environment.GetEnvironmentVariable("sendGridKey");
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(Environment.GetEnvironmentVariable("sendGridEmail"), Environment.GetEnvironmentVariable("sendGridUser"));
                        var subject = "";
                        var plainTextContent = "";
                        var htmlContent = "";
                        if (oldStatus.Equals("Inactivo") && doctor.Status.Equals("Activo"))
                        {
                            subject = "Tu cuenta ha sido habilitada";
                            plainTextContent = "Te informamos que tu cuenta ha sido habilitada. Si tienes inconvenientes con el inicio de sesión, puedes contactarnos a la siguiente dirección de correo electrónico: admin@medicalqr.com.ar";
                            htmlContent = "<div><p>Estimado(a),</div>" + "<div><p>Te informamos que tu cuenta ha sido habilitada. Si tienes inconvenientes con el inicio de sesión, puedes contactarnos a la siguiente dirección de correo electrónico: admin@medicalqr.com.ar</p></div>" + "<div><p>Saludos</p></div>" + "<div><p>Medical QR</p></div>";
                        } else if(oldStatus.Equals("Activo") && doctor.Status.Equals("Inactivo"))
                        {
                            subject = "Tu cuenta ha sido deshabilitada";
                            plainTextContent = "Te informamos que tu cuenta ha sido deshabilitada. Para mayor información puedes contactarte a la siguiente dirección de correo electrónico: admin@medicalqr.com.ar";
                            htmlContent = "<div><p>Estimado(a),</div>" + "<div><p>Te informamos que tu cuenta ha sido deshabilitada. Para mayor información puedes contactarte a la siguiente dirección de correo electrónico: admin@medicalqr.com.ar</p></div>" + "<div><p>Saludos</p></div>" + "<div><p>Medical QR</p></div>";
                        }

                        var to = new EmailAddress(doctor.email, "");  
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
                    var entity = dbContext.Doctors.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Doctor with ID " + id.ToString() + " not found to delete");
                    }
                    else
                    {
                        dbContext.Doctors.Remove(entity);
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
