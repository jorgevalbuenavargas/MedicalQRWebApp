using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MedicalQRWebApplication.Providers
{
    public class DataModel
    {
        public string sendGridKey { get; set; }
        public string sendGridEmail { get; set; }
        public string sendGridUser { get; set; }

        public static void setEnvironmentVariables()
        {
            StreamReader r = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "file.json");
            string jsonString = r.ReadToEnd();
            DataModel m = JsonConvert.DeserializeObject<DataModel>(jsonString);

            Environment.SetEnvironmentVariable("sendGridKey", m.sendGridKey);
            Environment.SetEnvironmentVariable("sendGridEmail", m.sendGridEmail);
            Environment.SetEnvironmentVariable("sendGridUser", m.sendGridUser);

        }

    }
    
}