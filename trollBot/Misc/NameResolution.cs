using System;
using Discord.WebSocket;

namespace TrollBot
{
    /// <summary>
    /// Provides functions for resolving string input from users to objects
    /// </summary>
    static class NameResolution
    {
        /// <summary>
        /// Resolves user input to a SocketGuildUser object. Applicable only internally to guilds
        /// </summary>
        /// <param name="input">The string user input</param>
        /// <param name="guild">The guild from which the input originated</param>
        /// <returns></returns>
        public static SocketGuildUser StringToGuildUser(string input, SocketGuild guild)
        {
            if (input == null || guild == null)
            {
                return null;
            }

            // Make input lowercase for ease of user input
            string inputLower = input.ToLower();

            // First, try parsing it as an user ID
            try
            {
                var id = Convert.ToUInt64(inputLower);
                var result = guild.GetUser(id);
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception)
            {
            }

            // If not, let's iterate over the guild's users to try and find a match
            foreach (SocketGuildUser user in guild.Users)
            {
                if (user.Username != null && CompareNames(inputLower, user.Username.ToLower()))
                {
                    return user;
                }
            }

            // This is done in two different iterations so that users cannot abuse nicknames to obfuscate the results
            // of these commands
            foreach (SocketGuildUser user in guild.Users)
            {
                if (user.Nickname != null && CompareNames(inputLower, user.Nickname.ToLower()))
                {
                    return user;
                }
            }

            return null;
        }

        /// <summary>
        /// Compares two names and returns the result
        /// </summary>
        /// <param name="first">The first name in the comparison</param>
        /// <param name="second">The second name in the comparison</param>
        /// <returns>True if the strings are equal, close enough to each other, or the first is a substring starting the second.
        /// False otherwise.</returns>
        private static bool CompareNames(string first, string second)
        {
            return (first == second ||
                    TestLevenshteinDistance(first, second) ||
                    second.StartsWith(first));
        }

        /// <summary>
        /// Indicates the percentage threshold that two strings must be within one another to pass the Levenshtein test.
        /// </summary>
        private const double LevenshteinThreshold = 0.3;

        /// <summary>
        /// Compares two strings to each other on the basis of Levenshtein distance and returns the result
        /// </summary>
        /// <param name="first">The first name in the comparison</param>
        /// <param name="second">The second name in the comparison</param>
        /// <returns>True if the two strings are within a percentage Levenshtein distance of one another. False otherwise</returns>
        private static bool TestLevenshteinDistance(string first, string second)
        {
            int distance = ComputeLevenshteinDistance(first, second);

            return (double) distance / (double) second.Length < LevenshteinThreshold;
        }

        /// <summary>
        /// Computes the Levenshtein distance between two strings, or the number of edits one must make to either string to make it the other
        /// </summary>
        /// <param name="first">The first name in the comparison</param>
        /// <param name="second">The second name in the comparison</param>
        /// <returns>The Levenshtein distance between the strings</returns>
        private static int ComputeLevenshteinDistance(string first, string second)
        {
            int n = first.Length;
            int m = second.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (second[j - 1] == first[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}
