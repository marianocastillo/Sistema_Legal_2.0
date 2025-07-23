using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using webapi.ViewModels;

namespace Sistema_Legal_2._0.Server.Infraestructure
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

                    if (logoRoute != null)
                    {
                        body = body.Replace("$img", "<img src='cid:ContraloriaLogo'  alt='Logo' style='max-width:150px;height:auto;display:block;margin:0 auto;' />");
                    }
                    else
                    {
                        body = body.Replace("$img", ""); 
                    }

                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

                    if (logoRoute != null)
                    {
                        LinkedResource linkedImage = new LinkedResource(logoRoute, MediaTypeNames.Image.Jpeg)
                        {
                            ContentId = "ContraloriaLogo",
                            TransferEncoding = TransferEncoding.Base64
                        };
                        avHtml.LinkedResources.Add(linkedImage);
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
        public static class CorreoHelper
        {
            public static async Task EnviarCorreoAudiencia(int idAudiencia, SqlConnection conn)
            {
                using var cmdCorreo = new SqlCommand("sp_GetInfoAudienciaCorreo", conn)
                {
                    CommandType = CommandType.StoredProcedure
                    
                };
                cmdCorreo.Parameters.AddWithValue("@IdAudiencia", idAudiencia);
                
                using var reader = await cmdCorreo.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {

                    int id_Ltg = reader.GetInt32(reader.GetOrdinal("id_Ltg"));
                    string urlDetalle = $"https://localhost:5173/Detalles/{id_Ltg}";
                    var body = $@"
             <div style='border: 1px solid #dcdcdc; border-radius: 10px; padding: 25px; font-family: Arial, sans-serif; background-color: #ffffff; max-width: 700px; margin: auto; box-shadow: 0 2px 5px rgba(0,0,0,0.05);'>

        <div style='display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 20px;'>
            <div>
                <p style='margin: 0 0 5px 0;color: #083d7a;  font-size: 16px;'><strong>Número de Acto:</strong> <span style='color: #888;'>( {reader["NumeroActo"]})</span></p>
                <p style='margin: 0; font-size: 16px;color: #083d7a;'><strong>Fecha de Acto:</strong><span style='color: #888;'> ({((DateTime)reader["FechaActo"]):dd/MM/yyyy})</span></p>
            </div>
            <div style='text-align: right;'>
                <p style='margin: 0; font-size: 16px;color: #083d7a;'><strong>Estado actual:</strong><br /> {reader["NombreSentencia"]} <span style='color: #888;'>({reader["NombreEstatus"]})</span></p>
            </div>
        </div>
                    
                    </div>
              <div style='font-size: 15px; line-height: 1.6; color: #333;'>
            <p><strong style='color: #083d7a;'>Título:</strong> {reader["Numero"]}</p>
            <p><strong style='color: #083d7a;'>Modalidad:</strong> {reader["Tipo"]}</p>
            <p><strong style='color: #083d7a;'>Tipo de Demanda:</strong> {reader["TipoDemanda"]}</p>
            <p><strong style='color: #083d7a;'>Demandante:</strong> {reader["NombreDemandante"]}</p>
            <p><strong style='color: #083d7a;'>Tipo de Demandante:</strong> {reader["TipoDemandante"]}</p>
            <p><strong style='color: #083d7a;'>Representante:</strong> {reader["NombreRepresentante"]}</p>
            <p><strong style='color: #083d7a;'>Fecha de Audiencia:</strong> {((DateTime)reader["FechaAudiencia"]):f}</p>
        </div>
       <p>Para más detalles, le invitamos a consultar el caso directamente en <a href='{urlDetalle}' style='color: #2b6cb0; text-decoration: none;'><strong>SILEG 2.0 HAGA CLICK AQUI PARA ACCEDER</strong></a>.</p>

                </div>";

                    var correos = reader["UsuariosEmails"].ToString()
                        .Split(';', StringSplitOptions.RemoveEmptyEntries);

                    await Mailing.SendMailAsync(new CorreoVM
                    {
                        recipients = correos,
                        subject = "Nueva Actualizacion en el litigio",
                        servicio = "Sistema de Audiencias",
                        messageHtml = body
                    });
                }
            }


            public static async Task EnviarCorreoAudienciaActualizada(int idLitigio, SqlConnection conn)
            {
                using var cmd = new SqlCommand("sp_GetInfoUltimaAudienciaCorreo", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@IdLitigio", idLitigio);
                string urlDetalle = $"https://localhost:5173/Detalles/{idLitigio}";
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var body = $@"
                <div style='font-family: Arial; font-size: 15px; background-color: #fffbe6; padding: 20px; border-radius: 10px; border: 1px solid #ccc; max-width: 700px; margin: auto;'>           
                    <div style='display: flex; justify-content: space-between; margin-bottom: 15px;'>
                       <div>
                <p style='margin: 0 0 5px 0;color: #083d7a;  font-size: 16px;'><strong>Número de Acto:</strong> <span style='color: #888;'>( {reader["NumeroActo"]})</span></p>
                <p style='margin: 0; font-size: 16px;color: #083d7a;'><strong>Fecha de Acto:</strong><span style='color: #888;'> ({((DateTime)reader["FechaActo"]):dd/MM/yyyy})</span></p>
            </div>
            <div style='text-align: right;'>
                <p style='margin: 0; font-size: 16px;color: #083d7a;'><strong>Estado actual:</strong><br /> {reader["NombreSentencia"]} <span style='color: #888;'>({reader["NombreEstatus"]})</span></p>
            </div>
        </div>
                 <div style='font-size: 15px; line-height: 1.6; color: #333;'>
            <p><strong style='color: #083d7a;'>Título:</strong> {reader["Numero"]}</p>
            <p><strong style='color: #083d7a;'>Modalidad:</strong> {reader["Tipo"]}</p>
            <p><strong style='color: #083d7a;'>Tipo de Demanda:</strong> {reader["TipoDemanda"]}</p>
            <p><strong style='color: #083d7a;'>Demandante:</strong> {reader["NombreDemandante"]}</p>
            <p><strong style='color: #083d7a;'>Tipo de Demandante:</strong> {reader["TipoDemandante"]}</p>
            <p><strong style='color: #083d7a;'>Representante:</strong> {reader["NombreRepresentante"]}</p>
            <p><strong style='color: #083d7a;'>Fecha de Audiencia:</strong> {((DateTime)reader["FechaAudiencia"]):f}</p>
        </div>

       <p>Para más detalles, le invitamos a consultar el caso directamente en <a href='{urlDetalle}' style='color: #2b6cb0; text-decoration: none;'><strong>SILEG 2.0 HAGA CLICK AQUI PARA ACCEDER</strong></a>.</p>

                </div>";

                    var correos = reader["UsuariosEmails"].ToString()
                        .Split(';', StringSplitOptions.RemoveEmptyEntries);

                    await Mailing.SendMailAsync(new CorreoVM
                    {
                        recipients = correos,
                        subject = "Audiencia actualizada",
                        servicio = "Sistema de Audiencias",
                        messageHtml = body
                    });
                }
            }

            public static async Task EnviarCorreoAsignacionAbogado(int idUsuario, int idLitigio, SqlConnection conn)
            {
               

                using var cmd = new SqlCommand(@"
        SELECT u.Email, u.nombres,u.apellidos, l.ltg_acto AS NombreCaso,l.ltg_Nombre_Demandante,TD.Nombre
        FROM Usuarios u
        INNER JOIN Asignaciones_Litigios al ON al.IdUsuario = u.idUsuario
        INNER JOIN Litigios l ON l.id_Ltg = al.Id_Ltg
        LEFT JOIN Tipo_Demanda TD ON L.id_Tipo_Demanda = TD.id_demanda
        LEFT JOIN Estatus_Litigios E ON L.id_Estatus = E.ltg_estatus
        WHERE u.idUsuario = @idUsuario AND l.id_Ltg = @idLitigio", conn);

                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@idLitigio", idLitigio);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    string email = reader["Email"].ToString();
                    string nombreCaso = reader["NombreCaso"].ToString();
                    string NombreDemandante = reader["ltg_Nombre_Demandante"].ToString();
                    string NombreDemanda = reader["Nombre"].ToString();
                    string nombres = reader["nombres"].ToString();
                    string apellidos = reader["apellidos"].ToString();
                    string urlDetalle = $"https://localhost:5173/Detalles/{idLitigio}";

                    string body = $@"
    <div style='font-family: Arial, sans-serif; font-size: 16px; padding: 25px; border-radius: 10px; border: 1px solid #b2f5ea; max-width: 600px; margin: auto;'>
        <p>Estimado(a) Abogado,{nombres} {apellidos} </p>
        <p>Por medio de la presente, se le informa que ha sido asignado(a) como abogado litigante al caso:</p>
        <ul style='list-style-type: none; padding-left: 0; margin-top: 20px; margin-bottom: 20px;'>
            <li><strong>Nombre del caso: </strong> {nombreCaso}</li>
            <li><strong>Demandante: </strong>{NombreDemandante}</li>
            <li><strong>Tipo de demanda: </strong> {NombreDemanda}</li>
        </ul>
       <p>Para más detalles, le invitamos a consultar el caso directamente en <a href='{urlDetalle}' style='color: #2b6cb0; text-decoration: none;'><strong>SILEG 2.0 HAGA CLICK AQUI PARA ACCEDER</strong></a>.</p>

    </div>";


                    await Mailing.SendMailAsync(new CorreoVM
                    {
                        recipients = new[] { email },
                        subject = $"Asignación de caso no.: {nombreCaso}",
                        servicio = "Asignación de Litigio",
                        messageHtml = body
                    });
                }
            }

            public static async Task<bool> YaFueNotificada(SqlConnection conn, int idAudiencia, string tipo)
            {
                var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Notificaciones_Audiencias WHERE Id_audiencia = @id AND TipoNotificacion = @tipo", conn);
                checkCmd.Parameters.AddWithValue("@id", idAudiencia);
                checkCmd.Parameters.AddWithValue("@tipo", tipo);

                var existe = (int)await checkCmd.ExecuteScalarAsync();
                return existe > 0;
            }

            public static async Task RegistrarNotificacion(SqlConnection conn, int idAudiencia, string tipo)
            {
                var insertCmd = new SqlCommand(
                    "INSERT INTO Notificaciones_Audiencias (Id_audiencia, TipoNotificacion) VALUES (@id, @tipo)", conn);
                insertCmd.Parameters.AddWithValue("@id", idAudiencia);
                insertCmd.Parameters.AddWithValue("@tipo", tipo);

                await insertCmd.ExecuteNonQueryAsync();
            }


        }

    }

    }
