using Discord;
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

            await Console.Out.WriteLineAsync("Timer started.");

            await Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Console.Out.WriteLineAsync("Checking for notifications...");
                    await SendReminder();

                    await Task.Delay(_interval);
                }
            }, _cancellationTokenSource.Token);
        }

        public async Task SendReminder()
        {
            DateTime currentTime = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

            string query = $"SELECT discordID FROM Users WHERE notificationDate < {unixTime}";
            List<string> discordIDs = _SQLiteService.ExecuteQuery(query);
            ulong channel = ulong.Parse(_SQLiteService.ExecuteGlobalValueQuery(_SQLiteService.channelIDValueName));
            

            EmbedBuilder embedBuilder = new()
            {
                Title = "Внимание!",
                Description = "Срок вашей медкарты истекает через 24 часа.\nОбновите её как можно скорее.",
                Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = _client.GetGuild(868119144288096277).IconUrl }
            };

            foreach (string discordID in discordIDs)
            {
                await _client.GetGuild(868119144288096277).GetTextChannel(channel)
                    .SendMessageAsync($"<@{discordID}>", embed: embedBuilder.Build());
                _SQLiteService.ExecuteNonQuery($"DELETE FROM Users WHERE discordID='{discordID}'");
                await Task.Delay(10000);
            }
        }

        public void StopTimer()
        {
            _cancellationTokenSource.Cancel();
            _running = false;
        }
    }
}
