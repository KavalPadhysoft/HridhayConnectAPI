using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HridhayConnect_API.Infra
{
	public static class AppHttpContextAccessor
	{
		private static IHttpContextAccessor _httpContextAccessor;
		private static IDataProtector _dataProtector;
		private static IConfiguration _iConfig;
		private static IHttpClientFactory _clientFactory;
		private static string _contentRootPath;
		private static string _webRootPath;

		//public static void Configure(IHttpContextAccessor httpContextAccessor, IHostEnvironment env_Host, IWebHostEnvironment env_Web, IDataProtectionProvider provider, IConfiguration iConfig, IHttpClientFactory clientFactory)
		//{
		public static void Configure(IHttpContextAccessor httpContextAccessor, IHostEnvironment env_Host, IWebHostEnvironment env_Web, IDataProtectionProvider provider, IConfiguration iConfig, IHttpClientFactory clientFactory)
		{
			_httpContextAccessor = httpContextAccessor;
			_contentRootPath = env_Host.ContentRootPath;
			if (!string.IsNullOrWhiteSpace(env_Web?.WebRootPath))
			{
				_webRootPath = env_Web.WebRootPath;
			}
			else
			{
				_webRootPath = Path.Combine(_contentRootPath, "wwwroot");
				EnsureFolder(_webRootPath);
			}
			_webRootPath = env_Web.WebRootPath;
			_dataProtector = provider.CreateProtector("ErosAPI");
			_iConfig = iConfig;
			_clientFactory = clientFactory;
		}
		private static void EnsureFolder(string path)
		{
			try
			{
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
			}
			catch
			{


			}
		}


		public static IHttpContextAccessor HttpContextAccessor => _httpContextAccessor;
		public static HttpContext AppHttpContext => _httpContextAccessor.HttpContext;
		public static ISession AppSession => AppHttpContext.Session;
		public static string AppBaseUrl => $"{AppHttpContext.Request.Scheme}://{AppHttpContext.Request.Host}{AppHttpContext.Request.PathBase}";
		public static string ContentRootPath => $"{_contentRootPath}";
		public static string WebRootPath => $"{_webRootPath}";
		public static IConfiguration AppConfiguration => _iConfig;
		public static string CurrentLogedUser => $"{(AppHttpContext.Session.GetInt32("UserId") ?? 0)}";
		public static string EncrKey => "yZ49o5VuG2Zf";
		public static string DataConnectionString => Convert.ToString(AppHttpContextAccessor.AppConfiguration.GetSection("ConnectionStrings").GetSection("DataConnection").Value);
		public static bool IsLogActive => Convert.ToBoolean(AppHttpContextAccessor.AppConfiguration.GetSection("IsLogActive").Value);
		public static bool IsLogActive_Error => Convert.ToBoolean(AppHttpContextAccessor.AppConfiguration.GetSection("IsLogActive_Error").Value);

		public static bool IsSendMail => Convert.ToBoolean(AppHttpContextAccessor.AppConfiguration.GetSection("Email_Configuration").GetSection("IsSendMail").Value);
		public static string AdminFromMail => Convert.ToString(AppHttpContextAccessor.AppConfiguration.GetSection("Email_Configuration").GetSection("AdminFromMail").Value);
		public static string DisplayName => Convert.ToString(AppHttpContextAccessor.AppConfiguration.GetSection("Email_Configuration").GetSection("DisplayName").Value);
		public static string Host => Convert.ToString(AppHttpContextAccessor.AppConfiguration.GetSection("Email_Configuration").GetSection("Host").Value);
		public static int Port => Convert.ToInt32(AppHttpContextAccessor.AppConfiguration.GetSection("Email_Configuration").GetSection("Port").Value);
		public static bool DefaultCredentials => Convert.ToBoolean(AppHttpContextAccessor.AppConfiguration.GetSection("Email_Configuration").GetSection("DefaultCredentials").Value);
		public static bool EnableSsl => Convert.ToBoolean(AppHttpContextAccessor.AppConfiguration.GetSection("Email_Configuration").GetSection("EnableSsl").Value);
		public static string MailPassword => Convert.ToString(AppHttpContextAccessor.AppConfiguration.GetSection("Email_Configuration").GetSection("Password").Value);


		public static long JwtUserId => Convert.ToInt64(AppHttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
		public static long JwtRoleId => Convert.ToInt64(AppHttpContext?.User?.FindFirst("RoleId")?.Value ?? "0");

		public static string JwtUserName =>
			AppHttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "";

		public static string ApplicationName => Convert.ToString(AppHttpContextAccessor.AppConfiguration.GetSection("loginsert").GetSection("ApplicationName").Value);

		public static string Defaultpassword = "bgfjtgry";// for default password


		public static readonly string[] AllowedFileExtensions = { ".jpg", ".jpeg", ".png", };

		public static bool IsValidFileExtension(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return false;

			string extension = Path.GetExtension(fileName).ToLower();
			return AllowedFileExtensions.Contains(extension);
		}

	}

	public class StartsNumericConstraint : IRouteConstraint
	{
		public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
		{
			if (values.TryGetValue(routeKey, out object routeValue))
			{
				string qr_prefix = routeValue?.ToString();
				if (!string.IsNullOrEmpty(qr_prefix) && char.IsDigit(qr_prefix[0]))
				{
					return true; // Match if qr_prefix starts with a digit
				}
			}
			return false;
		}
	}
}
