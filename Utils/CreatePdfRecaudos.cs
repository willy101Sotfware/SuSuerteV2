using SuSuerteV2.Domain.UIServices;
using SuSuerteV2.Domain;
using SuSuerteV2.Domain.Enumerables;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;




namespace SuSuerteV2.Utils
{
    public class PdfReaders
    {
   

        public static byte[] CreatePdfRecaudos(Transaction Transaction, Dictionary<string, string> body)
        {
            try
            {
                // Licencia
                QuestPDF.Settings.License = LicenseType.Community;

                EventLogger.SaveLog(EventType.Info,JsonConvert.SerializeObject(Transaction));

                var pdf = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Margin(30);
                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 800);

                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                //HEADER
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("Ico"));
                                col.Item().AlignCenter().PaddingTop(20).Width(200).Image(imagen);
                                col.Item().AlignCenter().ShowIf(Transaction.Type == ETypeTramites.Recaudos).Text("NIT. 810.000.317-8").FontSize(13).FontColor("808080");
                                col.Item().AlignCenter().PaddingBottom(40).ShowIf(Transaction.Type == ETypeTramites.Recaudos).Text(Transaction.ResponseNotifyPayment.Empresa.Direccion + " Tel.(6) " + Transaction.ResponseNotifyPayment.Empresa.Telefono).FontSize(13).FontColor("808080");

                                //BODY
                                col.Item().AlignCenter().AlignMiddle().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.ExtraBlack().FontColor("009fe3").FontFamily("Tahoma").FontSize(28));
                                    txt.Line("Comprobante de");
                                    txt.Line("Operación");
                                    txt.Line($"{DateTime.Now.ToString(DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? "Sáb d MMM hh:mm tt" : "ddd d MMM hh:mm tt", new CultureInfo("es-ES"))}").NormalWeight().FontSize(16).FontColor("000000");
                                    txt.Line("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").Light().FontSize(15).FontColor("808080");
                                });

                                // BOTÓN SIMPLIFICADO (sin SkiaSharp personalizado)
                                col.Item().AlignCenter().PaddingBottom(20).PaddingVertical(10).PaddingHorizontal(50)
                                    .Background("0136ce").Text(txt =>
                                    {
                                        txt.DefaultTextStyle(style => style.FontSize(22).FontColor("ffffff").Bold());
                                        txt.Span("Total:    ");
                                        txt.Span($"{Transaction.Total:C0}");
                                    });


                                col.Item().AlignCenter().AlignMiddle().ShowIf(Transaction.Type == ETypeTramites.Recaudos).Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line($"RECAUDO PARA TERCEROS {Transaction.Company.Nombre}");
                                    txt.Line($"{Transaction.Total:C0}");
                                });


                                col.Item().PaddingTop(-25).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");

                                foreach (var item in body)
                                {

                                    col.Item().PaddingHorizontal(25).Row(row =>
                                    {
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().AlignMiddle().Text(item.Key).ExtraBlack().FontColor("444444").FontFamily("Tahoma").FontSize(23);
                                        });
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().Text(item.Value).FontColor("222222").FontFamily("Tahoma").FontSize(15);
                                        });
                                    });
                                    col.Item().PaddingTop(-5).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");
                                }

                                //FOOTER
                                col.Item().AlignCenter().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line("Si tiene alguna inquietud contáctenos al teléfono");
                                    txt.Line("*57 (6) 898 48 48 ext. 100 o vía email a");
                                    txt.Line("servicioalcliente@susuerte.com");
                                });
                            });


                        });

                        //    AdminPayPlus.SaveLog(document.ToString());

                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 800);
                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("SecondImage"));
                                col.Item().AlignCenter().Width(310).Image(imagen);

                            });
                        });
                    });
                }).GeneratePdf();



                return pdf;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "AppConfig", "CreatePdfRecaudos catch", ex.Message + " " + ex.StackTrace, null);
              

                return null;
            }

        }

        public static byte[] CreatePdfChance(Transaction Transaction, Dictionary<string, string> body)
        {
            try
            {
                // Licencia
                QuestPDF.Settings.License = LicenseType.Community;

                EventLogger.SaveLog(EventType.Info, "CreatePdfChance", JsonConvert.SerializeObject(Transaction));
               

                var pdf = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Margin(30);
                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 800);

                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                //HEADER
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("Ico"));
                                col.Item().AlignCenter().PaddingTop(20).Width(200).Image(imagen);
                                col.Item().AlignCenter().ShowIf(Transaction.Type == ETypeTramites.Chance).Text("NIT. 810.000.317-8").FontSize(13).FontColor("808080");
                                col.Item().AlignCenter().PaddingBottom(40).ShowIf(Transaction.Type == ETypeTramites.Chance).Text(Transaction.ResponseNotifyChance.Empresa.Direccion + " Tel.(6) " + "898 48 48").FontSize(13).FontColor("808080");

                                //BODY
                                col.Item().AlignCenter().AlignMiddle().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.ExtraBlack().FontColor("009fe3").FontFamily("Tahoma").FontSize(28));
                                    txt.Line("Comprobante de");
                                    txt.Line("Operación");
                                    txt.Line($"{DateTime.Now.ToString(DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? "Sáb d MMM hh:mm tt" : "ddd d MMM hh:mm tt", new CultureInfo("es-ES"))}").NormalWeight().FontSize(16).FontColor("000000");
                                    txt.Line("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").Light().FontSize(15).FontColor("808080");
                                });

                                // BOTÓN SIMPLIFICADO (sin SkiaSharp personalizado)
                                col.Item().AlignCenter().PaddingBottom(20).PaddingVertical(10).PaddingHorizontal(50)
                                    .Background("0136ce").Text(txt =>
                                    {
                                        txt.DefaultTextStyle(style => style.FontSize(22).FontColor("ffffff").Bold());
                                        txt.Span("Total:    ");
                                        txt.Span($"{Transaction.Total:C0}");
                                    });



                                col.Item().AlignCenter().AlignMiddle().ShowIf(Transaction.Type == ETypeTramites.Chance).Text(txt =>
                                {
                                    string loterias = string.Join(", ", Transaction.ListaChances.SelectMany(listado => listado.Loterias.Select(nombre => nombre.Nombre)));

                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line($"{loterias} Fecha Sorteo: {Transaction.ResponseNotifyChance.Chance.Fechasorteo}");
                                    string tipoChance = string.Empty;
                                    int valor = 0;
                                    foreach (var Loterias in Transaction.ListaChances)
                                    {
                                        if (Loterias.Directo != 0) { tipoChance = "Directo"; valor = Loterias.Directo; }
                                        else if (Loterias.Combinado != 0) { tipoChance = "Combinado"; valor = Loterias.Combinado; }
                                        else if (Loterias.Pata != 0) { tipoChance = "Pata"; valor = Loterias.Pata; }
                                        else if (Loterias.Una != 0) { tipoChance = "Uña"; valor = Loterias.Una; }


                                        txt.Line($"{tipoChance}     {Loterias.Numero}       {valor.ToString("C0")}");

                                    }
                                });

                                col.Item().PaddingTop(-25).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");

                                foreach (var item in body)
                                {

                                    col.Item().PaddingHorizontal(25).Row(row =>
                                    {
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().AlignMiddle().Text(item.Key).ExtraBlack().FontColor("444444").FontFamily("Tahoma").FontSize(23);
                                        });
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().Text(item.Value).FontColor("222222").FontFamily("Tahoma").FontSize(15);
                                        });
                                    });
                                    col.Item().PaddingTop(-5).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");
                                }

                                //FOOTER
                                col.Item().AlignCenter().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line("Si tiene alguna inquietud contáctenos al teléfono");
                                    txt.Line("*300 912 5787 o vía email a");
                                    txt.Line("servicioalcliente@susuerte.com");
                                });
                            });


                        });

                        //    AdminPayPlus.SaveLog(document.ToString());

                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 800);
                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("SecondImage"));
                                col.Item().AlignCenter().Width(310).Image(imagen);
                                col.Item().AlignCenter().PaddingHorizontal(25).Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("222222"));
                                    txt.Line("El usuario se hace responsable de la custodia del");
                                    txt.Line("formulario apostado, puede verificarlo en su correo");
                                    txt.Line("electrónico. En caso de ser ganador debe imprimir el");
                                    txt.Line("formulario para reclamar el premio en punto de venta.");
                                    txt.Line("Si el premio es superior a $1.000.000 debe cobrarlo en");
                                    txt.Line("la oficina principal.");


                                });
                            });
                        });
                    });
                }).GeneratePdf();



                return pdf;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "CreatePdfChance", "CreatePdfChance catch", ex.Message + " " + ex.StackTrace, null);

                return null;
            }
        }


        public static byte[] CreatePdfBetplay(Transaction Transaction, Dictionary<string, string> body)
        {
            try
            {
                // Licencia
                QuestPDF.Settings.License = LicenseType.Community;
                EventLogger.SaveLog(EventType.Info, "CreatePdfBetplay", JsonConvert.SerializeObject(Transaction));
              

                var pdf = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Margin(30);
                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 820);

                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                //HEADER
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("Ico"));
                                col.Item().AlignCenter().PaddingTop(20).Width(200).Image(imagen);
                                col.Item().AlignCenter().ShowIf(Transaction.Type == ETypeTramites.BetPlay).Text(Transaction.ResponseNotifyBetplay.Empresa.Nit).FontSize(13).FontColor("808080");
                                col.Item().AlignCenter().PaddingBottom(40).ShowIf(Transaction.Type == ETypeTramites.BetPlay).Text(Transaction.ResponseNotifyBetplay.Empresa.Direccion + "  Tel.(6)  " + Transaction.ResponseNotifyBetplay.Empresa.Telefono).FontSize(13).FontColor("808080");

                                //BODY
                                col.Item().AlignCenter().AlignMiddle().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.ExtraBlack().FontColor("009fe3").FontFamily("Tahoma").FontSize(28));
                                    txt.Line("Comprobante de");
                                    txt.Line("Operación");
                                    txt.Line($"{DateTime.Now.ToString(DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? "Sáb d MMM hh:mm tt" : "ddd d MMM hh:mm tt", new CultureInfo("es-ES"))}").NormalWeight().FontSize(16).FontColor("000000");
                                    txt.Line("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").Light().FontSize(15).FontColor("808080");
                                });

                                // BOTÓN SIMPLIFICADO (sin SkiaSharp personalizado)
                                col.Item().AlignCenter().PaddingBottom(20).PaddingVertical(10).PaddingHorizontal(50)
                                    .Background("0136ce").Text(txt =>
                                    {
                                        txt.DefaultTextStyle(style => style.FontSize(22).FontColor("ffffff").Bold());
                                        txt.Span("Total:    ");
                                        txt.Span($"{Transaction.Total:C0}");
                                    });


                                col.Item().AlignCenter().AlignMiddle().ShowIf(Transaction.Type == ETypeTramites.BetPlay).Text(txt =>
                                {

                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line($" RECAUDO PARA TERCEROS recarga BetPlay por");
                                    decimal valor = (decimal)Transaction.ResponseNotifyBetplay.ValorRecaudo;
                                    string formattedValue = valor.ToString("C0", CultureInfo.CurrentCulture);
                                    txt.Line($" el valor de {formattedValue} a la cédula {Transaction.ResponseNotifyBetplay.ClienteId}");


                                });





                                col.Item().PaddingTop(-25).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");

                                foreach (var item in body)
                                {

                                    col.Item().PaddingHorizontal(25).Row(row =>
                                    {
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().AlignMiddle().Text(item.Key).ExtraBlack().FontColor("444444").FontFamily("Tahoma").FontSize(23);
                                        });
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().Text(item.Value).FontColor("222222").FontFamily("Tahoma").FontSize(15);
                                        });
                                    });
                                    col.Item().PaddingTop(-5).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");
                                }

                                //FOOTER
                                col.Item().AlignCenter().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line("Si tiene alguna inquietud contáctenos al teléfono");
                                    txt.Line(Transaction.ResponseNotifyBetplay.Empresa.Telefono + " o vía email a");
                                    txt.Line("servicioalcliente@susuerte.com");
                                });
                            });


                        });

                        //    AdminPayPlus.SaveLog(document.ToString());

                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 830);
                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("SecondImage"));
                                col.Item().AlignCenter().Width(310).Image(imagen);

                            });
                        });
                    });
                }).GeneratePdf();

                //   AdminPayPlus.SaveLog(pdf);

                return pdf;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "CreatePdfBetplay", "CreatePdfBetplay catch", ex.Message + " " + ex.StackTrace, null);

                return null;
            }
        }


        public static byte[] CreatePdfRecargasCel(Transaction Transaction, Dictionary<string, string> body)
        {
            try
            {
                // Licencia
                QuestPDF.Settings.License = LicenseType.Community;

                EventLogger.SaveLog(EventType.Info, "CreatePdfRecargasCel", JsonConvert.SerializeObject(Transaction));
       

                var pdf = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Margin(30);
                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 820);

                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                //HEADER
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("Ico"));
                                col.Item().AlignCenter().PaddingTop(20).Width(200).Image(imagen);
                                col.Item().AlignCenter().ShowIf(Transaction.Type == ETypeTramites.RecargasCel).Text(Transaction.ResponseNotificacionRecargaCel.Empresa.Nit).FontSize(13).FontColor("808080");
                                col.Item().AlignCenter().PaddingBottom(40).ShowIf(Transaction.Type == ETypeTramites.RecargasCel).Text(Transaction.ResponseNotificacionRecargaCel.Empresa.Direccion + "  Tel.(6)  " + Transaction.ResponseNotificacionRecargaCel.Empresa.Telefono).FontSize(13).FontColor("808080");

                                //BODY
                                col.Item().AlignCenter().AlignMiddle().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.ExtraBlack().FontColor("009fe3").FontFamily("Tahoma").FontSize(28));
                                    txt.Line("Comprobante de");
                                    txt.Line("Operación");
                                    txt.Line($"{DateTime.Now.ToString(DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? "Sáb d MMM hh:mm tt" : "ddd d MMM hh:mm tt", new CultureInfo("es-ES"))}").NormalWeight().FontSize(16).FontColor("000000");
                                    txt.Line("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").Light().FontSize(15).FontColor("808080");
                                });

                                // BOTÓN SIMPLIFICADO (sin SkiaSharp personalizado)
                                col.Item().AlignCenter().PaddingBottom(20).PaddingVertical(10).PaddingHorizontal(50)
                                    .Background("0136ce").Text(txt =>
                                    {
                                        txt.DefaultTextStyle(style => style.FontSize(22).FontColor("ffffff").Bold());
                                        txt.Span("Total:    ");
                                        txt.Span($"{Transaction.Total:C0}");
                                    });



                                col.Item().AlignCenter().AlignMiddle().ShowIf(Transaction.Type == ETypeTramites.RecargasCel).Text(txt =>
                                {

                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line($" RECAUDO PARA TERCEROS recarga Celular por");
                                    decimal valor = Convert.ToDecimal(Transaction.Total);
                                    string formattedValue = valor.ToString("C0", CultureInfo.CurrentCulture);
                                    txt.Line($" el valor de {formattedValue} al número de celular {Transaction.NumOperator}");


                                });





                                col.Item().PaddingTop(-25).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");

                                foreach (var item in body)
                                {

                                    col.Item().PaddingHorizontal(25).Row(row =>
                                    {
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().AlignMiddle().Text(item.Key).ExtraBlack().FontColor("444444").FontFamily("Tahoma").FontSize(23);
                                        });
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().Text(item.Value).FontColor("222222").FontFamily("Tahoma").FontSize(15);
                                        });
                                    });
                                    col.Item().PaddingTop(-5).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");
                                }

                                //FOOTER
                                col.Item().AlignCenter().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line("Si tiene alguna inquietud contáctenos al teléfono");
                                    txt.Line(Transaction.ResponseNotificacionRecargaCel.Empresa.Telefono + " o vía email a");
                                    txt.Line("servicioalcliente@susuerte.com");
                                });
                            });


                        });

                        //    AdminPayPlus.SaveLog(document.ToString());

                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 830);
                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("SecondImage"));
                                col.Item().AlignCenter().Width(310).Image(imagen);

                            });
                        });
                    });
                }).GeneratePdf();

                //   AdminPayPlus.SaveLog(pdf);

                return pdf;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "CreatePdfRecargasCel", "CreatePdfRecargasCel catch", ex.Message + " " + ex.StackTrace, null);
            

                return null;
            }
        }


        public static byte[] CreatePdfPaquetesCel(Transaction Transaction, Dictionary<string, string> body)
        {
            try
            {
                // Licencia
                QuestPDF.Settings.License = LicenseType.Community;

                EventLogger.SaveLog(EventType.Info, "CreatePdfPaquetesCel", JsonConvert.SerializeObject(Transaction));


                var pdf = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Margin(30);
                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 820);

                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                //HEADER
                                byte[] imagen = File.ReadAllBytes( AppConfig.Get("Ico"));
                                col.Item().AlignCenter().PaddingTop(20).Width(200).Image(imagen);
                                col.Item().AlignCenter().ShowIf(Transaction.Type == ETypeTramites.PaquetesCel).Text("8100003178").FontSize(13).FontColor("808080");
                                col.Item().AlignCenter().PaddingBottom(40).ShowIf(Transaction.Type == ETypeTramites.PaquetesCel).Text("CRA 23C NO 64-32 MANIZALES" + "  Tel.(6)  " + "8971499").FontSize(13).FontColor("808080");

                                //BODY
                                col.Item().AlignCenter().AlignMiddle().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.ExtraBlack().FontColor("009fe3").FontFamily("Tahoma").FontSize(28));
                                    txt.Line("Comprobante de");
                                    txt.Line("Operación");
                                    txt.Line($"{DateTime.Now.ToString(DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? "Sáb d MMM hh:mm tt" : "ddd d MMM hh:mm tt", new CultureInfo("es-ES"))}").NormalWeight().FontSize(16).FontColor("000000");
                                    txt.Line("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").Light().FontSize(15).FontColor("808080");
                                });
                                // BOTÓN SIMPLIFICADO (sin SkiaSharp personalizado)
                                col.Item().AlignCenter().PaddingBottom(20).PaddingVertical(10).PaddingHorizontal(50)
                                    .Background("0136ce").Text(txt =>
                                    {
                                        txt.DefaultTextStyle(style => style.FontSize(22).FontColor("ffffff").Bold());
                                        txt.Span("Total:    ");
                                        txt.Span($"{Transaction.Total:C0}");
                                    });

                                col.Item().AlignCenter().AlignMiddle().ShowIf(Transaction.Type == ETypeTramites.PaquetesCel).Text(txt =>
                                {

                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line($" RECAUDO PARA TERCEROS Paquetes Celular por");
                                    decimal valor = Convert.ToDecimal(Transaction.Total);
                                    string formattedValue = valor.ToString("C0", CultureInfo.CurrentCulture);
                                    txt.Line($" el valor de {formattedValue} al número de celular {Transaction.NumOperator}");


                                });





                                col.Item().PaddingTop(-25).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");

                                foreach (var item in body)
                                {

                                    col.Item().PaddingHorizontal(25).Row(row =>
                                    {
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().AlignMiddle().Text(item.Key).ExtraBlack().FontColor("444444").FontFamily("Tahoma").FontSize(23);
                                        });
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().Text(item.Value).FontColor("222222").FontFamily("Tahoma").FontSize(15);
                                        });
                                    });
                                    col.Item().PaddingTop(-5).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");
                                }

                                //FOOTER
                                col.Item().AlignCenter().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line("Si tiene alguna inquietud contáctenos al teléfono");
                                    txt.Line("8971499" + " o vía email a");
                                    txt.Line("servicioalcliente@susuerte.com");
                                });
                            });


                        });

                        //    AdminPayPlus.SaveLog(document.ToString());

                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 830);
                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("SecondImage"));
                                col.Item().AlignCenter().Width(310).Image(imagen);

                            });
                        });
                    });
                }).GeneratePdf();

                //   AdminPayPlus.SaveLog(pdf);

                return pdf;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "CreatePdfPaquetesCel", "CreatePdfPaquetesCel catch", ex.Message + " " + ex.StackTrace, null);

                return null;
            }
        }



        public static byte[] CreatePdfAstro(Transaction Transaction, Dictionary<string, string> body)
        {
            try
            {
                // Licencia
                QuestPDF.Settings.License = LicenseType.Community;
                EventLogger.SaveLog(EventType.Info, "CreatePdfAstro", JsonConvert.SerializeObject(Transaction));


                var pdf = Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Margin(30);
                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 820);

                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                //HEADER
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("Ico"));
                                col.Item().AlignCenter().PaddingTop(20).Width(200).Image(imagen);
                                col.Item().AlignCenter().ShowIf(Transaction.Type == ETypeTramites.Astro).Text("8100003178").FontSize(13).FontColor("808080");
                                col.Item().AlignCenter().PaddingBottom(40).ShowIf(Transaction.Type == ETypeTramites.Astro).Text("CRA 23C NO 64-32 MANIZALES" + "  Tel.(6)  " + "8971499").FontSize(13).FontColor("808080");

                                //BODY
                                col.Item().AlignCenter().AlignMiddle().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.ExtraBlack().FontColor("009fe3").FontFamily("Tahoma").FontSize(28));
                                    txt.Line("Comprobante de");
                                    txt.Line("Operación");
                                    txt.Line($"{DateTime.Now.ToString(DateTime.Now.DayOfWeek == DayOfWeek.Saturday ? "Sáb d MMM hh:mm tt" : "ddd d MMM hh:mm tt", new CultureInfo("es-ES"))}").NormalWeight().FontSize(16).FontColor("000000");
                                    txt.Line("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").Light().FontSize(15).FontColor("808080");
                                });

                                // BOTÓN SIMPLIFICADO (sin SkiaSharp personalizado)
                                col.Item().AlignCenter().PaddingBottom(20).PaddingVertical(10).PaddingHorizontal(50)
                                    .Background("0136ce").Text(txt =>
                                    {
                                        txt.DefaultTextStyle(style => style.FontSize(22).FontColor("ffffff").Bold());
                                        txt.Span("Total:    ");
                                        txt.Span($"{Transaction.Total:C0}");
                                    });

                                col.Item().AlignCenter().AlignMiddle().ShowIf(Transaction.Type == ETypeTramites.Recaudos).Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line($"RECAUDO PARA TERCEROS {Transaction.Company.Nombre}");
                                    txt.Line($"{Transaction.Total:C0}");
                                });


                                col.Item().AlignCenter().AlignMiddle().ShowIf(Transaction.Type == ETypeTramites.Astro).Text(txt =>
                                {

                                    txt.DefaultTextStyle(style => style.FontSize(13).FontColor("808080"));
                                    txt.Line($" RECAUDO PARA TERCEROS SUPER ASTRO por");
                                    decimal valor = Convert.ToDecimal(Transaction.Total);
                                    string formattedValue = valor.ToString("C0", CultureInfo.CurrentCulture);
                                    txt.Line($" el valor de {formattedValue} al número apostado {Transaction.ResponseVentaAstro.Listadodetalles.Detalle.Numeroapostado}");


                                });





                                col.Item().PaddingTop(-25).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");

                                foreach (var item in body)
                                {

                                    col.Item().PaddingHorizontal(25).Row(row =>
                                    {
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().AlignMiddle().Text(item.Key).ExtraBlack().FontColor("444444").FontFamily("Tahoma").FontSize(13);
                                        });
                                        row.RelativeItem().AlignLeft().AlignMiddle().Column(cols =>
                                        {
                                            cols.Item().Text(item.Value).FontColor("222222").FontFamily("Tahoma").FontSize(10);
                                        });
                                    });
                                    col.Item().PaddingTop(-5).PaddingBottom(10).AlignCenter().Text("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _").FontFamily("Tahoma").Light().FontSize(15).FontColor("808080");
                                }

                                //FOOTER
                                col.Item().AlignCenter().Text(txt =>
                                {
                                    txt.DefaultTextStyle(style => style.FontSize(10).FontColor("808080"));
                                    txt.Line("Si tiene alguna inquietud contáctenos al teléfono");
                                    txt.Line("8971499" + " o vía email a");
                                    txt.Line("servicioalcliente@susuerte.com");
                                });
                            });


                        });

                        //    AdminPayPlus.SaveLog(document.ToString());

                        document.Page(pages =>
                        {
                            pages.Margin(10);
                            pages.Size(400, 700);
                            pages.Content().Background("f6f6f6").Column(col =>
                            {
                                byte[] imagen = File.ReadAllBytes(AppConfig.Get("SecondImage"));
                                col.Item().AlignCenter().Width(310).Image(imagen);

                            });
                        });
                    });
                }).GeneratePdf();

                //   AdminPayPlus.SaveLog(pdf);

                return pdf;
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "CreatePdfAstro", "CreatePdfAstro catch", ex.Message + " " + ex.StackTrace, null);

                return null;
            }
        }



    }
}
