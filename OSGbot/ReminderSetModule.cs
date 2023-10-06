using System;
using Discord;
using Discord.Interactions;

namespace OSGbot
{
    public class ReminderSetModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly SQLiteService _SQLiteService;
        public ReminderSetModule(SQLiteService SQLiteService) {
            _SQLiteService = SQLiteService;
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("а-медкарта", "Напоминает, когда нужно обновить медкарту")]
        public async Task SetReminderCommandAdmin([Summary("Дата", "в формате дд.мм.гг")] string date,
            IUser? user)
        {
            long unixTimestamp;
            // Validate the input format
            if (IsValidDateFormat(date))
            {
                DateTime dateTime = DateTime.ParseExact(date, "dd.MM.yyyy", null);
                unixTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
            }
            else
            {
                EmbedBuilder failedBuilder = new()
                {
                    Title = "Ошибка!",
                    Description = $"Формат ввода даты: {DateTime.Now:dd.MM.yyyy}",
                    Color = Color.Red,
                    Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = Context.Guild.IconUrl }
                };
                await RespondAsync(embed: failedBuilder.Build(), ephemeral: true);
                return;
            }

            if (String.IsNullOrEmpty(_SQLiteService.ExecuteGlobalValueQuery(_SQLiteService.channelIDValueName)))
            {
                EmbedBuilder builder = new()
                {
                    Title = "Ошибка!",
                    Description = $"Канал для уведомлений не установлен,\nИспользуйте </канал-уведомления:1159215165653405778> в нужном канале.",
                    Color = Color.Red,
                    Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = Context.Guild.IconUrl }
                };
                await RespondAsync(embed: builder.Build(), ephemeral: true);
                return;
            }

            AddUserToReminderDB(user.Id, unixTimestamp);

            EmbedBuilder confirmedBuilder = new()
            {
                Title = "Принято!",
                Description = $"{user.Mention} получит уведомление за 24 часа до окончания срока!",
                Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = Context.Guild.IconUrl }
            };

            await RespondAsync(embed: confirmedBuilder.Build(), ephemeral: true);
        }


        [SlashCommand("медкарта", "Напоминает, когда нужно обновить медкарту")]
        public async Task SetReminderCommand([Summary("Дата", "в формате дд.мм.гг")] string date)
        {
            long unixTimestamp;
            // Validate the input format
            if (IsValidDateFormat(date))
            {
                DateTime dateTime = DateTime.ParseExact(date, "dd.MM.yyyy", null);
                unixTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
            }
            else
            {
                EmbedBuilder failedBuilder = new()
                {
                    Title = "Ошибка!",
                    Description = $"Формат ввода даты: {DateTime.Now:dd.MM.yyyy}",
                    Color = Color.Red,
                    Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = Context.Guild.IconUrl }
                };
                await RespondAsync(embed: failedBuilder.Build(), ephemeral: true);
                return;
            }

            if (String.IsNullOrEmpty(_SQLiteService.ExecuteGlobalValueQuery(_SQLiteService.channelIDValueName)))
            {
                EmbedBuilder builder = new()
                {
                    Title = "Ошибка!",
                    Description = $"Канал для уведомлений не установлен,\nИспользуйте `/канал-уведомления`> в нужном канале.",
                    Color = Color.Red,
                    Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = Context.Guild.IconUrl }
                };
                await RespondAsync(embed: builder.Build(), ephemeral: true);
                return;
            }

            AddUserToReminderDB(Context.User.Id, unixTimestamp);

            EmbedBuilder confirmedBuilder = new()
            {
                Title = "Принято!",
                Description = "Вы получите уведомление за 24 часа до окончания срока!",
                Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = Context.Guild.IconUrl }
            };

            await RespondAsync(embed: confirmedBuilder.Build(), ephemeral: true);
        }

        private void AddUserToReminderDB(ulong userID, long timestamp)
        {
            Console.WriteLine($"{userID} => {timestamp}");
            _SQLiteService.ExecuteInsertOrReplace(_SQLiteService.DBname, new Dictionary<string, object>
            {
                { "discordID", userID },
                { "notificationDate", timestamp }
            });
        }

        static bool IsValidDateFormat(string input)
        {
            return DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out _);
        }
    }
}