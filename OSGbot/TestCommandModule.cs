using System;
using Discord;
using Discord.Interactions;

namespace App
{
    public class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("медкарта", "Напоминает, когда нужно обновить медкарту")]
        public async Task SetReminderCommand([Summary("Дата", "в формате дд.мм.гг")] string date,
            IUser? user = null)
        {
            // Validate the input format
            if (IsValidDateFormat(date))
            {
                DateTime dateTime = DateTime.ParseExact(date, "dd.MM.yyyy", null);
                long unixTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
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
            }


            EmbedBuilder confirmedBuilder = new()
            {
                Title = "Принято!",
                Description = "Вы получите уведомление за 24 часа до окончания срока!",
                Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = Context.Guild.IconUrl }
            };

            if (user != null && user != Context.User)
            {
                confirmedBuilder.Description = $"{user.Mention} получит уведомление за 24 часа до окончания срока!";
            }

            await RespondAsync(embed: confirmedBuilder.Build(), ephemeral: true);
        }

        static bool IsValidDateFormat(string input)
        {
            // Check if the input matches the expected format
            return DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out _);
        }
    }
}