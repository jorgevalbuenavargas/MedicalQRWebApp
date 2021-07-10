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
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MedicalQRWebApplication.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UniqueIdentifierCodesController : ApiController
    {
        public HttpResponseMessage Get()
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var foundUniqueIdentifierCodes = dbContext.UniqueIdentifierCodes.ToList();
                foundUniqueIdentifierCodes.OrderByDescending(foundUniqueIdentifierCode => foundUniqueIdentifierCode.creationDate);
                return Request.CreateResponse(HttpStatusCode.OK, foundUniqueIdentifierCodes);
            }
        }

        public HttpResponseMessage GetByDoctor(Guid doctorId)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.UniqueIdentifierCodes.Where(e => e.doctorId == doctorId).ToList();
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "UIC not found for doctor with ID " + doctorId.ToString());
                }
            }
        }

        public HttpResponseMessage GetSendNotificationPendingUICByDoctor(Guid doctorId, String email)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.UniqueIdentifierCodes.Where(e => e.doctorId == doctorId).ToList();
                if (entity != null)
                {
                    var apiKey = Environment.GetEnvironmentVariable("sendGridKey");
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress(Environment.GetEnvironmentVariable("sendGridEmail"), Environment.GetEnvironmentVariable("sendGridUser"));
                    var subject = "Tienes códigos QR pendientes de habilitar";
                    var to = new EmailAddress(email, "");
                    var plainTextContent = "Tienes Códigos QR pendientes de habilitar, si ya has generado tu nuevo sello o libreta de prescripciones, puedes ingresar a la aplicación para habilitar tu CUI";
                    var htmlContent = "<div><p>Estimado(a),</div>" + "<div><p>Tienes Códigos QR pendientes de habilitar, si ya has generado tu nuevo sello o libreta de prescripciones, puedes ingresar a la aplicación para habilitar tu CUI</p></div>" + "<div><p>Saludos</p></div>" + "<div><p>Medical QR</p></div>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    var response = client.SendEmailAsync(msg);

                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "UIC not found for doctor with ID " + doctorId.ToString());
                }
            }
        }

        public HttpResponseMessage Get(Guid id)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.UniqueIdentifierCodes.FirstOrDefault(e => e.id == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "UIC with ID " + id.ToString() + " not found");
                }
            }
        }


        public HttpResponseMessage GetInformUIC(Guid id, String email)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.UniqueIdentifierCodes.FirstOrDefault(e => e.id == id);
                if (entity != null)
                {
                    //Generar QR Code
                    var qrImageFile = "C:\\temp\\" + entity.id + ".png";
                    string stringQRCode = entity.id.ToString().ToUpper();
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(stringQRCode, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    qrCodeImage.Save(qrImageFile, ImageFormat.Png);

                    //Generar Contenido de Email y Enviar

                    var apiKey = Environment.GetEnvironmentVariable("sendGridKey");
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress(Environment.GetEnvironmentVariable("sendGridEmail"), Environment.GetEnvironmentVariable("sendGridUser"));
                    var subject = "Información de tu código QR";
                    var to = new EmailAddress(email, "");
                    var plainTextContent = "¡Adjunto encontrarás tu código QR para que lo utilices en tu sello!";
                    var htmlContent = "<div><p>Estimado(a),</div>" + "<div><p>¡Adjunto encontrarás tu código QR para que lo utilices en tu sello!</p></div>" + "<div><p>Saludos</p></div>" + "<div><p>Medical QR</p></div>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    var bytes = File.ReadAllBytes(qrImageFile);
                    var file = Convert.ToBase64String(bytes);
                    msg.AddAttachment("codigoqr.png", file);
                    var response = client.SendEmailAsync(msg);
                    //var response = await client.SendEmailAsync(msg);
                    File.Delete(qrImageFile);

                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "UIC with ID " + id.ToString() + " not found");
                }
            }
        }

        public HttpResponseMessage Post([FromBody] UniqueIdentifierCode uniqueIdentifierCode)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.UniqueIdentifierCodes.Add(uniqueIdentifierCode);
                    dbContext.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.Created, uniqueIdentifierCode);
                    message.Headers.Location = new Uri(Request.RequestUri +
                        uniqueIdentifierCode.id.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(Guid id, [FromBody] UniqueIdentifierCode uniqueIdentifierCode)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    var entity = dbContext.UniqueIdentifierCodes.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "UIC with ID " + id.ToString() + " not found to update");
                    }
                    else
                    {
                        entity.creationDate = uniqueIdentifierCode.creationDate;
                        entity.doctorId = uniqueIdentifierCode.doctorId;
                        entity.status = uniqueIdentifierCode.status;
                        entity.image = uniqueIdentifierCode.image;
                        entity.modificationDate = uniqueIdentifierCode.modificationDate;
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
                    var entity = dbContext.UniqueIdentifierCodes.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "UIC with ID " + id.ToString() + " not found to delete");
                    }
                    else
                    {
                        dbContext.UniqueIdentifierCodes.Remove(entity);
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
