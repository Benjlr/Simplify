﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TestApp.Startup))]

namespace TestApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}