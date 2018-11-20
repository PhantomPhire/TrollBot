using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using static System.String;

namespace TrollBot.Services
{
    /// <summary>
    /// A repository for storing and retrieving roasts.
    /// </summary>
    class RoastService
    {
        /// <summary>
        /// Represents the path to the roasts text file.
        /// </summary>
        private readonly string _roastsPath = "./roasts.txt";

        /// <summary>
        /// Defines the delimiter to use for all roasts when reading/saving
        /// </summary>
        private const string RoastDelimiter = "~\n";

        /// <summary>
        /// The list of roasts,
        /// </summary>
        private List<string> _roasts;

        /// <summary>
        /// Initializes a new instance of the RoastService class.
        /// </summary>
        public RoastService()
        {
            readIn().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Determines the probability of the function RollRoast() succeeding.
        /// </summary>
        private const double RoastProbability = 0.2;

        /// <summary>
        /// The random number generator for the roasts class
        /// </summary>
        private readonly Random _rng = new Random();

        /// <summary>
        /// Determines, per message, if the TrollBot should roast a member.
        /// </summary>
        /// <returns>True if the bot should roast the member.</returns>
        public bool RollRoast()
        {
            if (_roasts.Count == 0)
            {
                return false;
            }

            var roll = _rng.Next(0, 100) / 100.0;
            return (roll < RoastProbability);
        }

        /// <summary>
        /// Represents the placeholder for the username to roast.
        /// </summary>
        private const string UsernamePlaceholder = "%U%";

        /// <summary>
        /// Gets a random roast from the roasts list.
        /// </summary>
        /// <param name="username">The name of the user to roast.</param>
        /// <returns>A random roast from the roasts list.</returns>
        public string GetRoast(string username)
        {
            if (_roasts.Count == 0)
            {
                return Empty;
            }

            int roll = _rng.Next(0, _roasts.Count);
            return _roasts[roll].Replace(UsernamePlaceholder, username);
        }

        /// <summary>
        /// Adds a roast to the roast collection and writes to disk.
        /// </summary>
        /// <param name="roast">The roast to add.</param>
        /// <returns>True if the roast add succeeds, false otherwise</returns>
        public async Task<bool> AddRoast(string roast)
        {
            _roasts.Add(roast);
            return await WriteOut();
        }

        /// <summary>
        /// Reads in roasts from the roast configuration file.
        /// </summary>
        /// <returns>True if the read succeeds, false otherwise</returns>
        private async Task<bool> readIn()
        {
            try
            {
                _roasts = new List<string>((await System.IO.File.ReadAllTextAsync(_roastsPath)).Split(RoastDelimiter));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when trying to parse roasts file:\n\n{0}\n\nUnable to roast members!", ex.ToString());
                _roasts = new List<string>();
                return false;
            }
        }

        /// <summary>
        /// Writes the current roasts to the roasts configuration file.
        /// </summary>
        /// <returns>Returns true if the write succeeds, false otherwise</returns>
        private async Task<bool> WriteOut()
        {
            try
            {
                var fileString = string.Empty;
                foreach (var roast in _roasts)
                {
                    if (fileString != Empty)
                    {
                        fileString += RoastDelimiter;
                    }
                    fileString += roast;
                }
                await System.IO.File.WriteAllTextAsync(_roastsPath, fileString);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when trying to write to roasts file:\n\n{0}", ex.ToString());
                return false;
            }
        }
    }
}
