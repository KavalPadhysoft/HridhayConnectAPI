using Microsoft.Data.SqlClient;
using System.Data;

namespace HridhayConnect_API.Infra
{
	public static class LogService
	{
		private static string prev_msg = "";

		private static void SetMSG(string msg) => prev_msg = msg;
		private static string GetMSG() => prev_msg;

		public static void LogInsert(string action, string message, Exception ex = null)
		{
			if (AppHttpContextAccessor.IsLogActive)
				try
				{
					var error = "";

					if (AppHttpContextAccessor.IsLogActive_Error && ex != null)
					{
						error = "Error : " + ex.Message.ToString() + Environment.NewLine;

						if (ex.InnerException != null)
						{
							try { error = error + " | InnerException: " + ex.InnerException.ToString().Substring(0, (ex.InnerException.ToString().Length > 1000 ? 1000 : ex.InnerException.ToString().Length)); } catch { error = error + "InnerException: " + ex.InnerException?.ToString(); }
						}

						if (ex.StackTrace != null)
						{
							try { error = error + " | StackTrace: " + ex.StackTrace.ToString().Substring(0, (ex.StackTrace.ToString().Length > 1000 ? 1000 : ex.StackTrace.ToString().Length)); } catch { error = error + "InnerException: " + ex.StackTrace?.ToString(); }
						}

						if (ex.Source != null)
						{
							try { error = error + " | Source: " + ex.Source.ToString().Substring(0, (ex.Source.ToString().Length > 1000 ? 1000 : ex.Source.ToString().Length)); } catch { error = error + "InnerException: " + ex.Source?.ToString(); }
						}

						if (ex.StackTrace == null && ex.Source == null)
						{
							try { error = error + " | Exception: " + ex.ToString().Substring(0, (ex.Source.ToString().Length > 3000 ? 3000 : ex.Source.ToString().Length)); } catch { error = error + "Exception: " + ex?.ToString(); }
						}
					}

					Write_Log((action + " | " + message + " | " + error));

					if (true)
					{
						DataTable dt = new DataTable();

						try
						{
							List<SqlParameter> oParams = new List<SqlParameter>();
							oParams.Add(new SqlParameter("P_MESSAGE", SqlDbType.NVarChar) { Value = action + " | " + message + " | " + error });

							using (SqlConnection conn = new SqlConnection(AppHttpContextAccessor.DataConnectionString))
							{
								using (SqlCommand cmd = new SqlCommand("PC_LOG_INSERT", conn))
								{
									cmd.CommandType = CommandType.StoredProcedure;

									if (oParams != null)
										foreach (SqlParameter param in oParams)
											cmd.Parameters.Add(param);

									SqlDataAdapter da = new SqlDataAdapter(cmd);

									da.Fill(dt);

								}
							}
						}
						catch { }

					}

				}
				catch { }
		}

		public static void Write_Log(string text)
		{
			try
			{
				string filePath = Convert.ToString(AppHttpContextAccessor.AppConfiguration.GetSection("Log_File_Path").Value);

				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(filePath))
				{
					filePath = filePath.Replace("<YYYYMMDD>", DateTime.Now.ToString("yyyyMMdd"));
					filePath = filePath.Replace("<HH>", DateTime.Now.ToString("HH"));

					if (!System.IO.Directory.Exists(Path.GetDirectoryName(filePath)))
						System.IO.Directory.CreateDirectory(Path.GetDirectoryName(filePath));

					if (!System.IO.File.Exists(filePath))
						System.IO.File.Create(filePath).Dispose();

					using (StreamWriter sw = System.IO.File.AppendText(filePath))
						sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + " || " + text + System.Environment.NewLine);
				}
			}
			catch { }
		}
	}
}
