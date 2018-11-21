using Discord;
using Discord.Audio;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using static System.String;

namespace TrollBot.Services
{
    public class StalkingService
    {
        public StalkingService()
        {

        }

        /// <summary>
        /// The user that is currently being targeted
        /// </summary>
        private ulong _target = 0;

        /// <summary>
        /// The guild where the targeting was initiated
        /// </summary>
        private IGuild _targetGuild;

        /// <summary>
        /// Bool that prevents from multiple subscriptions to SpeakingUpdating event.
        /// </summary>
        private bool _trackingVoice = false;



        public async Task SetTarget(SocketGuildUser user, IGuild guild)
        {
            if (_target != 0)
            {
                ClearTarget(guild);
            }

            if (guild == null || user.Id == 0)
            {
                return;
            }

            if (_trackingVoice == false)
            {
                guild.AudioClient.SpeakingUpdated += OnSpeakingUpdated;
                _trackingVoice = true;
            }
            _target = user.Id;
            _targetGuild = guild;
        }

        public async Task ClearTarget(IGuild guild)
        {
            guild.AudioClient.SpeakingUpdated -= OnSpeakingUpdated;
            _trackingVoice = false;
            _target = 0;
            _targetGuild = null;
        }

        private async Task OnSpeakingUpdated(ulong arg1, bool arg2)
        {
            await Service.Current.GetService<AudioService>().SendAudioAsync(_targetGuild);
        }

        public ulong GetTarget()
        {
            return _target;
        }

        public IGuild GetTargetGuild()
        {
            return _targetGuild;
        }
    }

}