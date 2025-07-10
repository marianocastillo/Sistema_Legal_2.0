using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using webapi.Comun;
using Microsoft.EntityFrameworkCore;
using webapi.ViewModels;
using Sistema_Legal_2._0.Server.Entities;

public class RecordatorioJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly db_silegContext _db_silegContext;
    public RecordatorioJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<db_silegContext>();

                    var hoy = DateTime.Today;
                    var ahora = DateTime.Now;
                    var en3dias = hoy.AddDays(3);

                    // ✅ Audiencias que ocurren en 3 días
                    var anticipadas = await db.Audiencias
                        .Where(x => x.Fecha.HasValue && x.Fecha.Value.Date == en3dias)
                        .ToListAsync();

                    foreach (var audiencia in anticipadas)
                    {
                        var correo = new CorreoVM
                        {
                            recipients = new[] { "ronvargas@contraloria.gob.do" },
                            subject = "Recordatorio: Audiencia próxima",
                            servicio = "Sistema de Audiencias",
                            messageHtml = $"<p>Te recordamos que tienes una audiencia programada para el <b>{audiencia.Fecha:dd/MM/yyyy hh:mm tt}</b>.</p>"
                        };

                        Console.WriteLine($"Enviando correo para audiencia: {audiencia.Id_audiencia} - {audiencia.Fecha}");
                        await Mailing.SendMailAsync(correo);

                        await Mailing.SendMailAsync(correo);
                    }

                    // ✅ Audiencias del mismo día (hoy), pero solo si es 8 AM o 3 PM
                    if (true)
                    {
                        var hoyMismo = await db.Audiencias
                            .Where(x => x.Fecha.HasValue && x.Fecha.Value.Date == hoy)
                            .ToListAsync();

                        foreach (var audiencia in hoyMismo)
                        {
                            var correo = new CorreoVM
                            {
                                recipients = new[] { "ronvargas@contraloria.gob.do" },
                                subject = "Recordatorio: Audiencia de hoy",
                                servicio = "Sistema de Audiencias",
                                messageHtml = $"<p>Te recordamos que hoy tienes una audiencia programada a las <b>{audiencia.Fecha:hh:mm tt}</b>.</p>"
                            };
                            Console.WriteLine($"Enviando correo para audiencia: {audiencia.Id_audiencia} - {audiencia.Fecha}");

                            await Mailing.SendMailAsync(correo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en el job de recordatorio: " + ex.Message);
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
