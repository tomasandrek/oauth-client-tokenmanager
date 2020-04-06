using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace Tomand.ClientTokenManager
{
	/// <summary>
	/// This client token manager provides functionality to authorize your request against OAuth server using tokens.
	/// </summary>
	public class Manager
	{   
		private static readonly object InstanceLock = new object();
		private static readonly object TokenCheckLock = new object();
		private static Dictionary<int, Client> clients = new Dictionary<int, Client>();

		private Manager()
		{
		}

		/// <summary>
		/// Internal singleton instance
		/// </summary>
		private static Manager instance = null;

		public static Manager GetInstance
		{
			get
			{
				if (instance == null) // Double-checked locking mechanism due to performance.
				{
					lock (InstanceLock)
					{
						if (instance == null)
						{
							instance = new Manager();
						}
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// Initializes the settings/parameters of each client. Needs to be called before using the manager.
		/// </summary>
		/// <param name="settings">Settings for each client</param>
		public void InitializeManager(IEnumerable<ClientSetting> settings)
		{
			clients.Clear();

			foreach(var setting in settings)
			{
				clients.Add(setting.UniqueId, new Client(){ TokenInfo = new TokenInfo(), ClientSetting = setting });
			}
		}

		/// <summary>
		/// Initializes the settings/parameters of each client. Needs to be called before using the manager.
		/// </summary>
		/// <param name="setting">Client setting</param>
		public void InitializeManager(ClientSetting setting)
		{
			clients.Clear();

			clients.Add(setting.UniqueId, new Client(){ TokenInfo = new TokenInfo(), ClientSetting = setting });
		}

		/// <summary>
		/// Adds the authorization header of the request and fills it with a token.
		/// </summary>
		/// <param name="clientUniqueId">Client unique ID from Setting class</param>
		/// <param name="request"></param>
		public void AuthorizeRequest(int clientUniqueId, HttpWebRequest request)
		{
			var client = GetToken(clientUniqueId);

			request.Headers[HttpRequestHeader.Authorization] = client.TokenInfo.token_type + " " + client.TokenInfo.access_token;
		}

		/// <summary>
		/// Adds the authorization header of the request and fills it with a token.
		/// </summary>
		/// <param name="clientUniqueId">Client unique ID from Setting class</param>
		/// <param name="request"></param>
		public void AuthorizeRequest(int clientUniqueId, System.Net.Http.HttpClient httpClient)
		{
			var client = GetToken(clientUniqueId);

			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(client.TokenInfo.token_type, client.TokenInfo.access_token);
		}

		/// <summary>
		/// Adds the authorization header of the request and fills it with a token.
		/// </summary>
		/// <param name="clientUniqueId">Client unique ID from Setting class</param>
		/// <param name="request"></param>
		public void AuthorizeRequest(int clientUniqueId, WebClient webClient)
		{
			var client = GetToken(clientUniqueId);

			if(webClient.Headers[HttpRequestHeader.Authorization] == null)
				webClient.Headers.Add(HttpRequestHeader.Authorization,client.TokenInfo.token_type + " " + client.TokenInfo.access_token);
			else
				webClient.Headers.Set(HttpRequestHeader.Authorization,client.TokenInfo.token_type + " " + client.TokenInfo.access_token);
		}

		/// <summary>
		/// Provides all information about the client along with the token info
		/// </summary>
		/// <param name="clientUniqueId">Client unique ID from Setting class</param>
		public Client GetToken(int clientUniqueId)
		{
			var client = clients[clientUniqueId];

			// Lock to prevent multiple threads refresh the token at the same time.
			lock(TokenCheckLock)
			{
				// Refresh token or use existing one.
				if (string.IsNullOrEmpty(client.TokenInfo.access_token) || client.TokenInfo.expity_date < DateTime.Now.AddSeconds(client.ClientSetting.MinimumTokenLifeTime))
				{
					// Refresh the access token if it expires and if its lifetime is too short to be of use.
					RefreshAuthToken(client);
				}
			}

			return client;
		}

		protected void RefreshAuthToken(Client client)
		{
			var webClient = new WebClient();
			webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

			var response = webClient.UploadValues(client.ClientSetting.AuthServer, client.ClientSetting.AsFormCollection);

			var tokenInfo = JsonSerializer.Deserialize<TokenInfo>(System.Text.Encoding.UTF8.GetString(response));

			client.TokenInfo = tokenInfo;
			client.TokenInfo.expity_date = DateTime.Now.AddSeconds(tokenInfo.expires_in);
		}

	}
}
