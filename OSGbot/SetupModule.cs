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
            _SQLiteService.ExecuteInsertOrReplace("GlobalValues", new Dictionary<string, object> { 
                { "Name", _SQLiteService.channelIDValueName }, 
                { "Value", Context.Channel.Id } });
            EmbedBuilder EB = new()
            {
                Title = "Принято!",
                Description =  $"Канал для уведомлений установлен на <#{Context.Channel.Id}>",
                Footer = new() { Text = "O.S.G. Dyachenko", IconUrl = Context.Guild.IconUrl }
            };
            await RespondAsync(embed: EB.Build());
        }
    }
}
