﻿using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AliceCLI.Authentication.Microsoft.Minecraft.OAuth2
{
    /// <summary>
    /// https://github.com/Azure-Samples/ms-identity-dotnet-desktop-tutorial/blob/master/4-DeviceCodeFlow/Console-DeviceCodeFlow-v2/Program.cs
    /// </summary>
    internal class AccessToken
    {
        private static PublicClientApplicationOptions appConfiguration = null;

        private static IConfiguration configuration;

        private static IPublicClientApplication application;

        string[] scopes = new[] { "user.read" };

        public AccessToken()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            configuration = builder.Build();

            appConfiguration = configuration.Get<PublicClientApplicationOptions>();
        }

        public async Task<string> Create()
        {
            return await SignInUserAndGetTokenUsingMSAL(appConfiguration, scopes);
        }

        private static async Task<string> SignInUserAndGetTokenUsingMSAL(PublicClientApplicationOptions configuration, string[] scopes)
        {
            Console.WriteLine("1");
            // build the AAd authority Url
            string authority = string.Concat(configuration.Instance, configuration.TenantId);

            // Initialize the MSAL library by building a public client application
            application = PublicClientApplicationBuilder.Create(configuration.ClientId)
                                                    .WithAuthority(authority)
                                                    .WithDefaultRedirectUri()
                                                    .Build();

            Console.WriteLine("2");
            AuthenticationResult result;

            try
            {
                Console.WriteLine("3");
                var accounts = await application.GetAccountsAsync();
                // Try to acquire an access token from the cache. If device code is required, Exception will be thrown.
                result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                result = await application.AcquireTokenWithDeviceCode(scopes, deviceCodeResult =>
                {
                    Console.WriteLine(deviceCodeResult.Message);
                    return Task.FromResult(0);
                }).ExecuteAsync();
            }
            Console.WriteLine("4");
            return result.AccessToken;
        }

    }
}
