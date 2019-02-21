using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TrollBot.Services;

namespace TrollBot.Commands
{
    /// <summary>
    /// Represents the repository for all commands used by TrollBot.
    /// </summary>
    public class TrollBotCommands : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// The ping command. Does stuff. Except not really.
        /// </summary>
        [Command("ping"), Alias("pong", "hello")]
        public Task PingAsync()
        {
            return ReplyAsync("pong!");
        }

        /// <summary>
        /// Adds a roast to the roast list in the roast service, and saves it to the disc.
        /// </summary>
        /// <param name="roast">The roast to add.</param>
        [Command("addroast", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddRoastAsync([Remainder] string roast)
        {
            bool result = await Service.Current.GetRequiredService<RoastService>().AddRoast(roast);
            if (result)
            {
                await ReplyAsync("Roast added!");
            }
            else
            {
                await ReplyAsync("Issue saving roast; roast is added but has not been saved to disc.");
            }
        }

        /// <summary>
        /// Roasts an user
        /// </summary>
        /// <param name="username">The name of the user to roast</param>
        [Command("roast", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task RoastAsync([Remainder] string username)
        {
            await ReplyAsync(Service.Current.GetRequiredService<RoastService>().GetRoast(username));
        }

        /// <summary>
        /// Joins a voice channel
        /// </summary>
        /// <returns></returns>
        [Command("join", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task JoinVoiceChannel()
        {
            IVoiceChannel channel = null;
            if (Context.User is SocketGuildUser user)
            {
                channel = user.VoiceChannel;
            }
            await Service.Current.GetService<AudioService>().JoinAudioChannelTask(Context.Guild, channel);
        }

        /// <summary>
        /// Leaves voice channel
        /// </summary>
        /// <returns></returns>
        [Command("leave", RunMode = RunMode.Async),RequireContext(ContextType.Guild),
        RequireUserPermission(GuildPermission.Administrator)]
        public async Task LeaveVoiceChannel()
        {
            await Service.Current.GetService<AudioService>().LeaveAudioChannelTask(Context.Guild);
        }

        /// <summary>
        /// Selects a user to stalk
        /// </summary>
        /// <param name="identification"></param>
        /// <returns></returns>
        [Command("follow", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task StalkUser([Remainder] string identification)
        {
            var userList = Context.Guild.Users;
            foreach (var user in userList)
            {
                if (user.Id.ToString() == identification || user.Username == identification ||
                    user.Nickname == identification)
                {
                    await Service.Current.GetService<StalkingService>().SetTarget(user, Context.Guild);
                    await ReplyAsync("Huehuehuehue");
                    break;
                }
            }
        }

        [Command("stop", RunMode = RunMode.Async), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task StopStalking()
        {
            await Service.Current.GetService<StalkingService>().ClearTarget(Context.Guild);
        }

        /// <summary>
        /// Adds a suggestion to the list in the suggestion service, and saves it to the disc.
        /// </summary>
        /// <param name="entry"></param>
        [Command("addsuggestion", RunMode = RunMode.Async), Alias("adds"), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddSuggestionAsync([Remainder] string entry)
        {
            var result = await Service.Current.GetRequiredService<SuggestionService>().AddSuggestion(entry);
            if (result == 1)
            {
                await ReplyAsync("Entry added!");
            }
            else if (result == 2)
            {
                await ReplyAsync("Bruh...that was already suggested. Can't you read?");
            }
            else
            {
                await ReplyAsync("Issue saving suggestion; it was added but has not been saved to disc.");
            }
        }

        /// <summary>
        /// Removes a suggestion from the list in the suggestion service, and saves it to the disc.
        /// </summary>
        /// <param name="entry"></param>
        [Command("removesuggestion", RunMode = RunMode.Async), Alias("rems"), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveSuggestionAsync([Remainder] string entry)
        {
            var result = await Service.Current.GetRequiredService<SuggestionService>().RemoveSuggestion(entry);
            if (result == 1)
            {
                await ReplyAsync("Entry removed!");
            }
            else if (result == 2)
            {
                await ReplyAsync("This was never suggested... Can't you read?");
            }
            else
            {
                await ReplyAsync("Issue saving list after removing suggestion; it was removed but has not been saved to disc.");
            }
        }
        /// <summary>
        /// Roasts an user
        /// </summary>
        /// <param name="username">The name of the user to roast</param>
        [Command("getsuggestions", RunMode = RunMode.Async), Alias("gets"), RequireContext(ContextType.Guild),
         RequireUserPermission(GuildPermission.Administrator)]
        public async Task GetSuggestionsAsync()
        {
            await ReplyAsync(Service.Current.GetRequiredService<SuggestionService>().GetSuggestions());
        }
    }
}
