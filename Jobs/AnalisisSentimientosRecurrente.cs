using System;

namespace BlogMVC.Jobs;

public class AnalisisSentimientosRecurrente : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            Console.WriteLine("Iniciando analisis de sntimientos de comentarios");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
