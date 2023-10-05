using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;

namespace OSGbot {
    class Program
    {
        private DiscordSocketClient? _client;
        private CommandService? _commands;
        private IServiceProvider? _services;
        private CommandServiceConfig? _commandConfig;
        private ICommandContext? _context;
        private InteractionService? _interactionService;
        private ulong _guildId = 868119144288096277;

        public static async Task Main(string[] args)
        {
            string? secret = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

            if (string.IsNullOrEmpty(secret))
            {
                Console.WriteLine("Secret not found. Make sure the environment variable is set.");
                return;
            }

            var program = new Program();
            await program.RunBotAsync(secret);
        }

        static IServiceProvider CreateServices()
        {
            String dbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SQLiteDB.db");
            var connectionString = new SqliteConnectionStringBuilder(null)
            {
                DataSource = dbFile,
                Mode = SqliteOpenMode.ReadWriteCreate,
            }.ToString();
            var collection = new ServiceCollection()
                .AddSingleton(new SQLiteService(connectionString));

            return collection.BuildServiceProvider();
        }

        public async Task RunBotAsync(string Token)
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _commandConfig = new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = Discord.Commands.RunMode.Async
            };
            
            _services = CreateServices();

            _client.Log += LogAsync;
            _client.Ready += OnReadyAsync;

            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log);
        }

        private async Task OnReadyAsync()
        {
            _interactionService = new InteractionService(_client.Rest);
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _interactionService.RegisterCommandsToGuildAsync(_guildId);


            _client.InteractionCreated += async (x) =>
            {
                var ctx = new SocketInteractionContext(_client, x);
                await _interactionService.ExecuteCommandAsync(ctx, _services);
            };
        }
    }
}