using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using webapi.ViewModels;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Sistema_Legal_2._0.Server.Infraestructure;
using static Sistema_Legal_2._0.Server.Infraestructure.Mailing;

public class RecordatorioJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly string _cadenaSQL;

    public RecordatorioJob(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _cadenaSQL = configuration.GetConnectionString("Sistema_Legal");
    }


    /// <summary>
    /// Job que nos permite mandar notificaciones a los 15 minutos, al dia y 3 dias antes que ocurra la audiencia
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                using (var connection = new SqlConnection(_cadenaSQL))
                {
                    await connection.OpenAsync(stoppingToken);

                    using var cmd = new SqlCommand("sp_GetAudienciasParaCorreo", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using var reader = await cmd.ExecuteReaderAsync(stoppingToken);
                    while (await reader.ReadAsync(stoppingToken))
                    {
                        var idAudiencia = (int)reader["Id_audiencia"];
                        var fechaAudiencia = (DateTime)reader["FechaAudiencia"];
                        var ahora = DateTime.Now;
                        var diferencia = fechaAudiencia - ahora;

                        string tipoNotificacion = null;
                        string subject = null;

                        if (diferencia.TotalMinutes <= 15 && diferencia.TotalMinutes > 0)
                        {
                            tipoNotificacion = "15 minutos antes";
                            subject = $"⏰ Audiencia en menos de 15 minutos";
                        }
                        else if (fechaAudiencia.Date == ahora.Date)
                        {
                            tipoNotificacion = "Diario";
                            subject = $"📅 Hoy tiene una audiencia";
                        }
                        else if (fechaAudiencia.Date == ahora.Date.AddDays(3))
                        {
                            tipoNotificacion = "3 dias Antes";
                            subject = $"📌 Próxima audiencia dentro de 3 días";
                        }
                        else
                        {
                            continue;
                        }

                     
                        if (string.IsNullOrWhiteSpace(tipoNotificacion))
                            continue;

                        if (await CorreoHelper.YaFueNotificada(connection, idAudiencia, tipoNotificacion))
                            continue;

                    
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

        <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;' />

        <div style='font-size: 15px; line-height: 1.6; color: #333;'>
            <p><strong style='color: #083d7a;'>Título:</strong> {reader["Numero"]}</p>
            <p><strong style='color: #083d7a;'>Modalidad:</strong> {reader["Tipo"]}</p>
            <p><strong style='color: #083d7a;'>Tipo de Demanda:</strong> {reader["TipoDemanda"]}</p>
            <p><strong style='color: #083d7a;'>Demandante:</strong> {reader["NombreDemandante"]}</p>
            <p><strong style='color: #083d7a;'>Tipo de Demandante:</strong> {reader["TipoDemandante"]}</p>
            <p><strong style='color: #083d7a;'>Representante:</strong> {reader["NombreRepresentante"]}</p>
            <p><strong style='color: #083d7a;'>Fecha de Audiencia:</strong> {((DateTime)reader["FechaAudiencia"]):f}</p>
        </div>

        <div style='margin-top: 30px; font-size: 0.9em; color: #666; border-top: 1px dashed #ccc; padding-top: 15px;'>
            <em>Este mensaje fue generado automáticamente por el Sistema Legal. Si tiene preguntas, contacte a su supervisor o la Dirección Legal.</em>
        </div>
    </div>";

                        var correos = reader["UsuariosEmails"].ToString()
                            .Split(';', StringSplitOptions.RemoveEmptyEntries);

                        var correo = new CorreoVM
                        {
                            recipients = correos,
                            subject = subject,
                            servicio = "Sistema de Audiencias",
                            messageHtml = body
                        };

                        Console.WriteLine($"📧 Enviando correo ({tipoNotificacion}) para audiencia: {idAudiencia} - {fechaAudiencia}");
                        await Mailing.SendMailAsync(correo);

                      CorreoHelper.RegistrarNotificacion(connection, idAudiencia, tipoNotificacion);
                    }

                }
            }
            
            catch (Exception ex)
            {
                Console.WriteLine("Error en el job de recordatorio: " + ex.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
