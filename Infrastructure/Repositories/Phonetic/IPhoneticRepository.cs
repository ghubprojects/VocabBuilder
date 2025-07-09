namespace VocabBuilder.Infrastructure.Repositories.Phonetic;

public interface IPhoneticRepository
{
    /// <summary>
    /// Gets phonetic transcription for a word from common words dictionary
    /// </summary>
    /// <param name="word">Word to get phonetic for</param>
    /// <returns>Phonetic transcription if found, null otherwise</returns>
    string? GetPhonetic(string word);

    /// <summary>
    /// Checks if a word exists in the common words dictionary
    /// </summary>
    /// <param name="word">Word to check</param>
    /// <returns>True if word exists in common words dictionary</returns>
    bool IsCommonWord(string word);

    /// <summary>
    /// Gets all common words
    /// </summary>
    /// <returns>Dictionary of common words and their phonetics</returns>
    IReadOnlyDictionary<string, string> GetAllCommonWords();
}
