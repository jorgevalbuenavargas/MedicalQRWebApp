using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MedicalQRWebApplication.Providers;
using Microsoft.Owin;
using Newtonsoft.Json;
using Owin;

[assembly: OwinStartup(typeof(MedicalQRWebApplication.Startup))]

namespace MedicalQRWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            DataModel.setEnvironmentVariables();
            ConfigureAuth(app);

        }
    }
}
