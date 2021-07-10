using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using MedicalQRWebApplication.Models;

namespace MedicalQRWebApplication.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MedicalReceiptsController : ApiController
    {
        public HttpResponseMessage Get()
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var foundMedicalReceipts = dbContext.MedicalReceipts.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, foundMedicalReceipts);
            }
        }

        public HttpResponseMessage Get(Guid id)
        {
            using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
            {
                dbContext.Configuration.ProxyCreationEnabled = false;
                var entity = dbContext.MedicalReceipts.FirstOrDefault(e => e.id == id);
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Medical Receipt with ID " + id.ToString() + " not found");
                }
            }
        }

        public HttpResponseMessage Post([FromBody] MedicalReceipt medicalReceipt)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.MedicalReceipts.Add(medicalReceipt);
                    dbContext.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.Created, medicalReceipt);
                    message.Headers.Location = new Uri(Request.RequestUri +
                        medicalReceipt.id.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Put(Guid id, [FromBody] MedicalReceipt medicalReceipt)
        {
            try
            {
                using (MedicalQRDBContext dbContext = new MedicalQRDBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    var entity = dbContext.MedicalReceipts.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Medical Receipt with ID " + id.ToString() + " not found to update");
                    }
                    else
                    {
                        entity.scanDate = medicalReceipt.scanDate;
                        entity.validationResult = medicalReceipt.validationResult;
                        entity.pharmacyId = medicalReceipt.pharmacyId;
                        entity.uicId = medicalReceipt.uicId;
                        entity.securityCodeId = medicalReceipt.securityCodeId;
                        entity.applicationMessage = medicalReceipt.applicationMessage;
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
                    var entity = dbContext.MedicalReceipts.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Medical Receipts with ID " + id.ToString() + " not found to delete");
                    }
                    else
                    {
                        dbContext.MedicalReceipts.Remove(entity);
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
