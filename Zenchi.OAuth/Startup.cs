﻿using Zenchi.OAuth.DAL;
using Zenchi.OAuth.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;

[assembly: OwinStartup(typeof(Zenchi.OAuth.Startup))]
namespace Zenchi.OAuth
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(AuthContext.Create);
            // The below line will be needed when email or text confirmation of registration is needed.
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                CookieName = "Zenchi.OAuth",
                CookieDomain = ConfigurationManager.AppSettings["AuthCookieDomain"],
                LoginPath = new PathString("/auth/login"),
                LogoutPath = new PathString("/auth/logout")
            });


            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

        }
    }
}