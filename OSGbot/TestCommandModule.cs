using System.Threading.Tasks;
using Discord.Commands;
using Discord.Interactions;

namespace App
{
    public class TestCommandModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("test-slash", "Says hello")]
        public async Task HelloCommand()
        {
            await RespondAsync($"Hello, {Context.User.Mention}!");
        }
    }
}