using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace HridhayConnect_API.Infra
{
    public class Common
    {
        private static string EncrKey = AppHttpContextAccessor.EncrKey;

        public static string Encrypt(string strText)
        {
            try
            {
                if (!string.IsNullOrEmpty(strText))
                {
                    byte[] byKey = { };
                    byte[] IV = {
                            0x12,
                            0x34,
                            0x56,
                            0x78,
                            0x90,
                            0xab,
                            0xcd,
                            0xef
                        };

                    //byKey = System.Text.Encoding.UTF8.GetBytes(Strings.Left(strEncrKey, 8));
                    byKey = System.Text.Encoding.UTF8.GetBytes(EncrKey.Substring(0, 8));
                    //byKey = System.Text.Encoding.UTF8.GetBytes(Strings.Left(strEncrKey, 8));
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
                    MemoryStream ms = new MemoryStream();
                    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());

                }
            }
            catch (ExecutionEngineException ex) { }

            return "";
        }

        public static string Decrypt(string strText)
        {
            byte[] byKey = { };
            byte[] IV = {
                            0x12,
                            0x34,
                            0x56,
                            0x78,
                            0x90,
                            0xab,
                            0xcd,
                            0xef
                        };
            byte[] inputByteArray = new byte[strText.Length + 1];
            try
            {
                //byKey = System.Text.Encoding.UTF8.GetBytes(Strings.Left(sDecrKey, 8));
                byKey = System.Text.Encoding.UTF8.GetBytes(EncrKey.Substring(0, 8));
                using (System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider())
                {
                    inputByteArray = Convert.FromBase64String(strText);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write))
                        {
                            cs.Write(inputByteArray, 0, inputByteArray.Length);
                            cs.FlushFinalBlock();
                            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                            return encoding.GetString(ms.ToArray());
                        }
                    }
                }

            }
            catch (ExecutionEngineException ex)
            {
                return ex.Message;
            }
        }





        public static async Task<(bool IsSuccess, string Message)> SendEmail(string subject, string recipient_mails, bool isBodyHtml = false, string? body = null, string? templateFile = null, string? templateData = null)
        {
            //LogService.LogInsert("Common - SendEmail", $"Send Email => Starting at {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace("-", "/")} | Subject : {subject} | To : {string.Join(", ", recipient_mails)}", null);

            (bool IsSuccess, string Message) result = (false, "Sending Mail service is stop.");

            try
            {
                if (AppHttpContextAccessor.IsSendMail && recipient_mails != null && recipient_mails.Length > 0)
                {

                    if (string.IsNullOrEmpty(subject))
                    {
                        result.Message = $"Please enter valid subject.";
                        return result;
                    }

                    if (string.IsNullOrEmpty(recipient_mails))
                    {
                        result.Message = $"Please enter valid recipient mail address(s).";
                        return result;
                    }

                    subject = subject.Replace("<DD-MM-YYYY>", DateTime.Now.ToString("dd-MM-yyyy"));
                    subject = subject.Replace("<DD/MM/YYYY>", DateTime.Now.ToString("dd/MM/yyyy").Replace("-", "/"));
                    subject = subject.Replace("<YYYYMMDD>", DateTime.Now.ToString("yyyyMMdd"));
                    subject = subject.Replace("<DDMMYYYY>", DateTime.Now.ToString("yyyyMMdd"));
                    subject = subject.Replace("<HH:MM TT>", DateTime.Now.ToString("hh:mm tt"));
                    subject = subject.Replace("<HH:MM>", DateTime.Now.ToString("HH:mm"));
                    subject = subject.Replace("<HH>", DateTime.Now.ToString("HH"));
                    subject = subject.Replace("<MM>", DateTime.Now.ToString("mm"));

                    if (isBodyHtml == true && !string.IsNullOrEmpty(templateFile))
                    {
                        using (StreamReader reader = new StreamReader(System.IO.Path.Combine(AppHttpContextAccessor.WebRootPath, "Email_Templates", templateFile + "_enquiry.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        if (!string.IsNullOrEmpty(templateData))
                        {
                            JObject data = JObject.Parse(templateData);

                            foreach (var prop in data.Properties())
                            {
                                string key = "{{" + prop.Name.ToLower() + "}}";
                                string value = prop.Value?.ToString() ?? "";

                                if (prop.Name.Equals("email", StringComparison.OrdinalIgnoreCase))
                                {
                                    value = $"<a href='mailto:{value}'>{value}</a>";
                                }



                                body = body.Replace(key, value);
                            }
                        }

                        body = System.Text.RegularExpressions.Regex.Replace(body, "{{.*?}}", "");
                    }


                    using (System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage())
                    {
                        mailMessage.From = new System.Net.Mail.MailAddress(AppHttpContextAccessor.AdminFromMail, AppHttpContextAccessor.DisplayName);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = isBodyHtml;

                        foreach (string item in recipient_mails.Split(",").ToArray())
                            mailMessage.To.Add(new System.Net.Mail.MailAddress(item.Trim()));

                        System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(AppHttpContextAccessor.Host, AppHttpContextAccessor.Port)
                        {
                            EnableSsl = AppHttpContextAccessor.EnableSsl,
                            Credentials = new System.Net.NetworkCredential(AppHttpContextAccessor.AdminFromMail, AppHttpContextAccessor.MailPassword),
                            DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = AppHttpContextAccessor.DefaultCredentials
                        };

                        await smtpClient.SendMailAsync(mailMessage);

                        result.IsSuccess = true;
                        result.Message = "Mail Sent successfully.";
                    }

                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Sending Mail service is stop.";
                }

            }
            catch (Exception ex)
            {

                try
                {
                    var ctx = AppHttpContextAccessor.AppHttpContext;
                    string requestUrl = string.Empty;
                    string userAgent = string.Empty;
                    string clientIp = string.Empty;

                    if (ctx?.Request != null)
                    {
                        try
                        {
                            var scheme = ctx.Request.Scheme ?? "http";
                            var host = ctx.Request.Host.HasValue ? ctx.Request.Host.Value : string.Empty;
                            var path = ctx.Request.Path.HasValue ? ctx.Request.Path.Value : string.Empty;
                            var qs = ctx.Request.QueryString.HasValue ? ctx.Request.QueryString.Value : string.Empty;
                            requestUrl = $"{scheme}://{host}{path}{qs}";
                        }
                        catch { requestUrl = string.Empty; }

                        try { userAgent = ctx.Request.Headers["User-Agent"].ToString(); } catch { userAgent = string.Empty; }
                    }

                    try { clientIp = ctx?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty; } catch { clientIp = string.Empty; }

                    long? userId = null;
                    try { userId = AppHttpContextAccessor.JwtUserId; } catch { userId = null; }

                    var log = new ErrorLog
                    {
                        ApplicationName = AppHttpContextAccessor.ApplicationName,
                        ControllerName = "Common_SendEmail",
                        ErrorMessage = ex?.Message ?? "Unknown",
                        ErrorType = ex?.GetType().FullName ?? string.Empty,
                        StackTrace = ex?.ToString() ?? string.Empty,
                        RequestUrl = requestUrl,
                        RequestPayload = $"subject:{(subject?.Length > 200 ? subject.Substring(0, 200) : subject)}, recipients:{(recipient_mails?.Length > 200 ? recipient_mails.Substring(0, 200) : recipient_mails)}",
                        UserAgent = userAgent,
                        UserId = userId,
                        ClientIP = clientIp,
                        CreatedBy = AppHttpContextAccessor.JwtUserName ?? AppHttpContextAccessor.AdminFromMail
                    };

                    LogEntry.InsertLogEntry(log);

                }
                catch
                {
                    try { System.Diagnostics.Trace.TraceError($"Common.SendEmail logging failed: {ex}"); } catch { }
                }

                result.IsSuccess = false;
                result.Message = "Error: " + ex.Message;
                //throw; for go to other ex
            }

            return result;





            //	result.IsSuccess = false;

            //	if (ex != null)
            //	{
            //		result.Message = "Error : " + ex.Message.ToString() + Environment.NewLine;

            //		if (ex.InnerException != null)
            //		{
            //			try { result.Message = result.Message + " | InnerException: " + ex.InnerException.ToString().Substring(0, (ex.InnerException.ToString().Length > 1000 ? 1000 : ex.InnerException.ToString().Length)); } catch { result.Message = result.Message + "InnerException: " + ex.InnerException?.ToString(); }
            //		}

            //		if (ex.StackTrace != null)
            //		{
            //			try { result.Message = result.Message + " | StackTrace: " + ex.StackTrace.ToString().Substring(0, (ex.StackTrace.ToString().Length > 1000 ? 1000 : ex.StackTrace.ToString().Length)); } catch { result.Message = result.Message + "InnerException: " + ex.StackTrace?.ToString(); }
            //		}

            //		if (ex.Source != null)
            //		{
            //			try { result.Message = result.Message + " | Source: " + ex.Source.ToString().Substring(0, (ex.Source.ToString().Length > 1000 ? 1000 : ex.Source.ToString().Length)); } catch { result.Message = result.Message + "InnerException: " + ex.Source?.ToString(); }
            //		}

            //		if (ex.StackTrace == null && ex.Source == null)
            //		{
            //			try { result.Message = result.Message + " | Exception: " + ex.ToString().Substring(0, (ex.Source.ToString().Length > 3000 ? 3000 : ex.Source.ToString().Length)); } catch { result.Message = result.Message + "Exception: " + ex?.ToString(); }
            //		}
            //	}
            //}

            //LogService.LogInsert("Common - SendEmail", $"Send Email => Completed at {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace("-", "/")} | IsSuccess : {result.IsSuccess} | Message : {result.Message} | Subject : {subject} | To : {string.Join(", ", recipient_mails)}", null);

            //return result;
        }


        public static async Task<(bool IsSuccess, string Message)> SendEmailwithAttachment(string subject, string recipient_mails, bool isBodyHtml = false, string? body = null, string? templateFile = null, string? templateData = null, IFormFile? attachfile = null)
        {
            LogService.LogInsert("Common - SendEmail", $"Send Email => Starting at {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace("-", "/")} | Subject : {subject} | To : {string.Join(", ", recipient_mails)}", null);

            (bool IsSuccess, string Message) result = (false, "Sending Mail service is stop.");

            try
            {
                if (AppHttpContextAccessor.IsSendMail && recipient_mails != null && recipient_mails.Length > 0)
                {

                    if (string.IsNullOrEmpty(subject))
                    {
                        result.Message = $"Please enter valid subject.";
                        return result;
                    }

                    if (string.IsNullOrEmpty(recipient_mails))
                    {
                        result.Message = $"Please enter valid recipient mail address(s).";
                        return result;
                    }

                    subject = subject.Replace("<DD-MM-YYYY>", DateTime.Now.ToString("dd-MM-yyyy"));
                    subject = subject.Replace("<DD/MM/YYYY>", DateTime.Now.ToString("dd/MM/yyyy").Replace("-", "/"));
                    subject = subject.Replace("<YYYYMMDD>", DateTime.Now.ToString("yyyyMMdd"));
                    subject = subject.Replace("<DDMMYYYY>", DateTime.Now.ToString("yyyyMMdd"));
                    subject = subject.Replace("<HH:MM TT>", DateTime.Now.ToString("hh:mm tt"));
                    subject = subject.Replace("<HH:MM>", DateTime.Now.ToString("HH:mm"));
                    subject = subject.Replace("<HH>", DateTime.Now.ToString("HH"));
                    subject = subject.Replace("<MM>", DateTime.Now.ToString("mm"));

                    if (isBodyHtml == true && !string.IsNullOrEmpty(templateFile))
                    {
                        using (StreamReader reader = new StreamReader(System.IO.Path.Combine(AppHttpContextAccessor.ContentRootPath + "/Content/Email_Templates", templateFile + "_enquiry.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        if (!string.IsNullOrEmpty(templateData))
                        {
                            JObject data = JObject.Parse(templateData);

                            foreach (var prop in data.Properties())
                            {
                                string key = "{{" + prop.Name.ToLower() + "}}";
                                string value = prop.Value?.ToString() ?? "";

                                if (prop.Name.Equals("email", StringComparison.OrdinalIgnoreCase))
                                {
                                    value = $"<a href='mailto:{value}'>{value}</a>";
                                }



                                body = body.Replace(key, value);
                            }
                        }

                        body = System.Text.RegularExpressions.Regex.Replace(body, "{{.*?}}", "");
                    }


                    using (System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage())
                    {
                        mailMessage.From = new System.Net.Mail.MailAddress(AppHttpContextAccessor.AdminFromMail, AppHttpContextAccessor.DisplayName);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = isBodyHtml;

                        foreach (string item in recipient_mails.Split(",").ToArray())
                            mailMessage.To.Add(new System.Net.Mail.MailAddress(item.Trim()));
                        if (attachfile != null && attachfile.Length > 0)
                        {
                            var ms = new MemoryStream();
                            await attachfile.CopyToAsync(ms);
                            ms.Position = 0;

                            var attachment = new Attachment(ms, attachfile.FileName, attachfile.ContentType);
                            mailMessage.Attachments.Add(attachment);
                        }
                        System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(AppHttpContextAccessor.Host, AppHttpContextAccessor.Port)
                        {
                            EnableSsl = AppHttpContextAccessor.EnableSsl,
                            Credentials = new System.Net.NetworkCredential(AppHttpContextAccessor.AdminFromMail, AppHttpContextAccessor.MailPassword),
                            DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = AppHttpContextAccessor.DefaultCredentials
                        };

                        await smtpClient.SendMailAsync(mailMessage);

                        result.IsSuccess = true;
                        result.Message = "Mail Sent successfully.";
                    }

                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Sending Mail service is stop.";
                }

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;

                if (ex != null)
                {
                    result.Message = "Error : " + ex.Message.ToString() + Environment.NewLine;

                    if (ex.InnerException != null)
                    {
                        try { result.Message = result.Message + " | InnerException: " + ex.InnerException.ToString().Substring(0, (ex.InnerException.ToString().Length > 1000 ? 1000 : ex.InnerException.ToString().Length)); } catch { result.Message = result.Message + "InnerException: " + ex.InnerException?.ToString(); }
                    }

                    if (ex.StackTrace != null)
                    {
                        try { result.Message = result.Message + " | StackTrace: " + ex.StackTrace.ToString().Substring(0, (ex.StackTrace.ToString().Length > 1000 ? 1000 : ex.StackTrace.ToString().Length)); } catch { result.Message = result.Message + "InnerException: " + ex.StackTrace?.ToString(); }
                    }

                    if (ex.Source != null)
                    {
                        try { result.Message = result.Message + " | Source: " + ex.Source.ToString().Substring(0, (ex.Source.ToString().Length > 1000 ? 1000 : ex.Source.ToString().Length)); } catch { result.Message = result.Message + "InnerException: " + ex.Source?.ToString(); }
                    }

                    if (ex.StackTrace == null && ex.Source == null)
                    {
                        try { result.Message = result.Message + " | Exception: " + ex.ToString().Substring(0, (ex.Source.ToString().Length > 3000 ? 3000 : ex.Source.ToString().Length)); } catch { result.Message = result.Message + "Exception: " + ex?.ToString(); }
                    }
                }
            }

            LogService.LogInsert("Common - SendEmail", $"Send Email => Completed at {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace("-", "/")} | IsSuccess : {result.IsSuccess} | Message : {result.Message} | Subject : {subject} | To : {string.Join(", ", recipient_mails)}", null);

            return result;
        }

    }
}
