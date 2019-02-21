using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using static System.String;

namespace TrollBot.Services
{
    /// <summary>
    /// A repository for storing suggestions to a List and recalling suggestions.
    /// </summary>
    class SuggestionService
    {
        /// <summary>
        /// Represents the path to the Lists text file.
        /// </summary>
        private readonly string _suggestionsPath = "./Suggestions.txt";

        /// <summary>
        /// Defines the delimiter to use for all Lists when reading/saving
        /// </summary>
        private const string ListDelimiter = "~\n";

        /// <summary>
        /// List of suggestions
        /// </summary>
        private List<string> _suggestions;

        /// <summary>
        /// Initializes a new instance of the ListService class.
        /// </summary>
        public SuggestionService()
        {
            ReadIn().GetAwaiter().GetResult();
        }

        public string GetSuggestions()
        {
            if (_suggestions.Count == 0)
            {
                return Empty;
            }

            string stringToReturn = string.Empty;

            foreach (var entry in _suggestions)
            {
                stringToReturn += entry+"\n";
            }

            return stringToReturn;
        }
        /// <summary>
        /// Adds a suggestion to the List and writes to disk.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns>1 if the List add succeeds, 2 if it's a duplicate, 0 otherwise</returns>
        public async Task<int> AddSuggestion(string entry)
        {
            bool writeOutResult = false;

            //check if suggestion already exists
            foreach (var suggestion in _suggestions)
            {
                if (suggestion.ToLower().Equals(entry.ToLower()))
                {
                    return 2;
                }
            }

            _suggestions.Add(entry);
            writeOutResult = await WriteOut();
            return writeOutResult ? 1 : 0;
        }

        /// <summary>
        /// Removes a suggestion from the List and writes to disk.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns>True if the suggestion removal succeeds, false otherwise</returns>
        public async Task<int> RemoveSuggestion(string entry)
        {
            //check if suggestion exists
            foreach (var suggestion in _suggestions)
            {
                if (suggestion.ToLower().Equals(entry.ToLower()))
                {
                    _suggestions.Remove(entry);
                    var writeOutResult = await WriteOut();
                    return writeOutResult ? 1 : 0;
                }
            }

            return 2;

        }

        /// <summary>
        /// Reads in suggestions from the suggestion configuration file.
        /// </summary>
        /// <returns>True if the read succeeds, false otherwise</returns>
        private async Task<bool> ReadIn()
        {
            try
            {
                _suggestions = new List<string>((await System.IO.File.ReadAllTextAsync(_suggestionsPath)).Split(ListDelimiter));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when trying to parse suggestion list:\n\n{0}\n\nUnable to list suggestions!", ex.ToString());
                _suggestions = new List<string>();
                return false;
            }
        }

        /// <summary>
        /// Writes the current Lists to the Lists configuration file.
        /// </summary>
        /// <returns>Returns true if the write succeeds, false otherwise</returns>
        private async Task<bool> WriteOut()
        {
            try
            {
                var fileString = string.Empty;
                foreach (var suggestion in _suggestions)
                {
                    if (fileString != Empty)
                    {
                        fileString += ListDelimiter;
                    }
                    fileString += suggestion;
                }
                await System.IO.File.WriteAllTextAsync(_suggestionsPath, fileString);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when trying to write to Suggestion file:\n\n{0}", ex.ToString());
                return false;
            }
        }
    }
}
