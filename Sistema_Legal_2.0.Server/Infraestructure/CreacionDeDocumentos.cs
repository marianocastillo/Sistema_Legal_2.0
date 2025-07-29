using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using static Sistema_Legal_2._0.Server.Models.SistemadeRegistro;

public class LitigioPdfDocument : IDocument
{
    private readonly LitigioCompletoDto _data;
    private readonly string _filesServerPath; 
    private readonly IConfiguration _configuration;

    public LitigioPdfDocument(IConfiguration config, LitigioCompletoDto data)
    {
        _data = data;
        _filesServerPath = config.GetSection("Configuracion").GetValue<string>("FilesServerPath")?.TrimEnd('/') + "/";
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Size(PageSizes.A4);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header().Text($"Litigio N° {_data.Litigio.id_Ltg}").FontSize(16).Bold();

            page.Content().Column(col =>
            {
                // Datos generales
                col.Item().Text($"Acto: {_data.Litigio.ltg_acto}");
                col.Item().Text($"Fecha: {_data.Litigio.ltg_Fecha_Acto:dd/MM/yyyy}");
                col.Item().Text($"Demandante: {_data.Litigio.ltg_Nombre_Demandante} ({_data.Litigio.ltg_Cedula_Demandante})");
                col.Item().Text($"Representante: {_data.Litigio.ltg_Nombre_Representante} ({_data.Litigio.ltg_Cedula_Representante})");
                col.Item().Text($"Tipo de Demanda: {_data.Litigio.Nombre}");
                col.Item().Text($"Sentencia: {_data.Litigio.desc_Sentencia}");
                col.Item().Text($"Estatus: {_data.Litigio.ltg_description}");

                // Audiencias
                col.Item().PaddingTop(20).Text("Audiencias:").Bold();

                foreach (var aud in _data.Audiencias)
                {
                    col.Item().PaddingBottom(10).Column(audCol =>
                    {
                        audCol.Item().Text($"● {aud.Audiencia.Numero} ({aud.Audiencia.Tipo}) - {aud.Audiencia.Fecha:dd/MM/yyyy HH:mm}");
                        audCol.Item().Text($"   Sala: {aud.NombreSala.Trim()} - Tribunal: {aud.NombreTribunal}");
                        foreach (var ev in aud.Evidencias)
                        {
                            var rutaRelativa = ev.Ruta_Archivo.Replace("\\", "/");
                            var url = $"https://localhost:5173/api/Files/rutaspor/{rutaRelativa}";


                            audCol.Item().Text(text =>
                            {
                                text.Span("   - Evidencia: ").FontColor(Colors.Black);
                                text.Span(ev.Comentario_Evidencia ?? "(sin comentario)").FontColor(Colors.Grey.Darken1);
                                text.Line("");
                                text.Hyperlink("Ver archivo", url).FontColor(Colors.Blue.Medium);
                            });
                        }

                    });
                }

                // Cambios de estatus
                col.Item().PaddingTop(20).Text("Historial de cambios de estatus:").Bold();
                foreach (var cambio in _data.CambiosEstatus)
                {
                    col.Item().Text($"→ {cambio.Fecha_cambio:dd/MM/yyyy HH:mm}: {cambio.Valor_anterior} → {cambio.Valor_nuevo}");
                }
            });

            page.Footer().AlignCenter().Text(txt => txt.Span("Generado por el sistema legal"));
        });
    }
}
