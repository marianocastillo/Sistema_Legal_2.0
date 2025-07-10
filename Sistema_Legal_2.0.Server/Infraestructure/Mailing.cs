using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using webapi.ViewModels;

namespace webapi.Comun
{
    public static class Mailing
    {
        private static readonly IConfiguration configuration;

        static Mailing()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private static string GetHtmlTemplateCode()
        {
            try
            {
                string route = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MailLayout.html");
                if (!File.Exists(route))
                {
                    throw new FileNotFoundException("No se encontró la plantilla de correo en: " + route);
                }
                return File.ReadAllText(route);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la plantilla de correo: " + ex.Message);
                return "<html><body><p>Error cargando plantilla de correo.</p></body></html>";
            }
        }

       public static async Task SendMailAsync(CorreoVM correo)
        {
            try
            {
                if (correo.recipients == null || correo.recipients.Length == 0)
                {
                    throw new ArgumentException("No se han proporcionado destinatarios para el correo.");
                }

                string logoRoute = configuration["EmailSettings:LogoPath"];
                if (!File.Exists(logoRoute))
                {
                    Console.WriteLine("Advertencia: No se encontró la imagen del logo en " + logoRoute);
                    logoRoute = null;
                }

                using (MailMessage mail = new MailMessage())
                {
                    foreach (var recipient in correo.recipients)
                    {
                        mail.To.Add(recipient);
                    }

                    mail.From = new MailAddress(configuration["EmailSettings:FromEmail"], configuration["EmailSettings:FromName"]);
                    mail.Subject = configuration["EmailSettings:SubjectPrefix"] + correo.subject;

                    string body = GetHtmlTemplateCode();
                    body = body.Replace("$body", correo.messageHtml);
                    body = body.Replace("$servicio", correo.servicio);
                    body = body.Replace("$systemUrl", configuration["EmailSettings:SystemUrl"]);

                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

                    if (logoRoute != null)
                    {
                        LinkedResource linkedImage = new LinkedResource(logoRoute, MediaTypeNames.Image.Jpeg)
                        {
                            ContentId = "ContraloriaLogo"
                        };
                        avHtml.LinkedResources.Add(linkedImage);
                        body = body.Replace("$img", "<img src='cid:ContraloriaLogo'>");
                    }

                    mail.AlternateViews.Add(avHtml);

                    using (SmtpClient smtp = new SmtpClient(configuration["EmailSettings:Host"], int.Parse(configuration["EmailSettings:Port"])))
                    {
                        smtp.EnableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"]);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(configuration["EmailSettings:User"], configuration["EmailSettings:Password"]);

                        await smtp.SendMailAsync(mail);
                        Console.WriteLine("Correo enviado correctamente a: " + string.Join(", ", correo.recipients));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar correo: " + ex.Message);
            }
        }
       
    }
}
