using System;
using System.Net;
using System.Collections.Generic;
using Tomand.ClientTokenManager;

namespace OauthClientConsumerApp
{
    class Program
    {
        public const string SecuredWebsite1 = "http://127.0.0.1:5000";
        public const string SecuredWebsite2 = "http://127.0.0.2:5000";

        static void Main(string[] args)
        {
            Console.WriteLine("App started!");
            
            // Get instance of the token manager
            var tokenManager = Manager.GetInstance;

            // Client 1
            var setting = new ClientSetting(){
                UniqueId = 1,
                GrantType = GrantType.ClientCredentials,
                ClientId = "qnQg4357i29NnZQCxq14PytdZFuuMqgB",
                ClientSecret = "LZAc1jDHtQCTjcw4XrYkYwRV4l6-ZyRcUf31UyAC_I7BIlL7gAFMzl598WaVfyI1",
                Audience = "http://mysite.com",
                Scope = new string[] { "scheme:list", "scheme:create", "scheme:assessor:list", "scheme:assessor:fetch", "scheme:assessor:update", "assessment:fetch", "assessment:lodge", "assessment:search", "address:search", "migrate:assessment", "migrate:assessor", "migrate:address", "report:assessor:status" }
            };
            setting.AuthServer = new Uri("https://dev-mqauy-jb.auth0.com/oauth/token");

            // Client 2
            var setting2 = new ClientSetting(){
                UniqueId = 2,
                GrantType = GrantType.ClientCredentials,
                ClientId = "qnQg4357i29NnZQCxq14PytdZFuuMqgB",
                ClientSecret = "ALZAc1jDHtQCTjcw4XrYkYwRV4l6-ZyRcUf31UyAC_I7BIlL7gAFMzl598WaVfyI1",
                Audience = "http://somemistakehere.com",
                Scope = new string[] { "scheme:list", "scheme:create", "scheme:assessor:list", "scheme:assessor:fetch", "scheme:assessor:update", "assessment:fetch", "assessment:lodge", "assessment:search", "address:search", "migrate:assessment", "migrate:assessor", "migrate:address", "report:assessor:status" }
            };
            setting2.AuthServer = new Uri("https://dev-mqauy-jb.auth0.com/oauth/token");

            // Create a list of settings
            var settings = new List<ClientSetting>();
            settings.Add(setting);
            settings.Add(setting2);

            // Initialize the manager prior to use
            tokenManager.InitializeManager(settings);

            try
            {
                // Request 1 - Client 1 - For the very first time authorized request successfully. Demonstration of issuing a token
                HttpWebRequest webrequest = (HttpWebRequest) WebRequest.Create(SecuredWebsite1);
                tokenManager.AuthorizeRequest(1, webrequest);
                Console.WriteLine("\n Request1 authorized with token: " + webrequest.Headers["Authorization"]);
                
                // Request 2 - Client 1 - For the second time authorized request successfully. Demonstration of using the existing token
                HttpWebRequest webrequest2 = (HttpWebRequest) WebRequest.Create(SecuredWebsite1);
                tokenManager.AuthorizeRequest(1, webrequest2);
                Console.WriteLine("\n Request2 authorized with token: " + webrequest2.Headers["Authorization"]);
                
                // Request 3 - Client 2 - Demonstration of an error due to wrong credentials (settings).
                HttpWebRequest webrequest3 = (HttpWebRequest) WebRequest.Create(SecuredWebsite2);
                tokenManager.AuthorizeRequest(2, webrequest3);
                Console.WriteLine("\n Request3 authorized with token: " + webrequest3.Headers["Authorization"]);
            }
            catch(Exception ex)
            {
                Console.WriteLine("\n Exception occured: " + ex.Message);
            }
            
            Console.ReadLine();
        }
    }
}
