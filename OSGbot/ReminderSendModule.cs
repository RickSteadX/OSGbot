using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace OSGbot
{
    public class ReminderSendModule
    {
        private readonly SQLiteService _SQLiteService;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        private readonly DiscordSocketClient _client;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _running;

        public ReminderSendModule(IServiceProvider services, DiscordSocketClient client)
        {
            _SQLiteService = services.GetRequiredService<SQLiteService>();
            _client = client;
            _cancellationTokenSource = new CancellationTokenSource();
            _running = false;
        }

        public async Task StartTimer()
        {
            if (_running) return;

            _running = true;

            await Console.Out.WriteLineAsync("Timer started");

            await Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Console.Out.WriteLineAsync("Tick");

                    await Task.Delay(_interval);
                }
            }, _cancellationTokenSource.Token);
        }

        public void StopTimer()
        {
            _cancellationTokenSource.Cancel();
            _running = false;
        }
    }
}
