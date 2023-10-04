using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OSGbot
{
    public class SetupModule : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly SQLiteService _SQLiteService;
        public SetupModule(SQLiteService SQLiteService)
        {
            _SQLiteService = SQLiteService;
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("канал-уведомления", "Устанавливает канал для уведомлений.")]
        public async Task ReminderChannelSet()
        {
            _SQLiteService.CreateTable("CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY, Name TEXT)");

            EmbedBuilder EB = new()
            {
                Title = "Принято!",
                Description = "Канал для уведомлений установлен.",
                Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = Context.Guild.IconUrl }
            };
            await RespondAsync(embed: EB.Build());
        }
    }
}
