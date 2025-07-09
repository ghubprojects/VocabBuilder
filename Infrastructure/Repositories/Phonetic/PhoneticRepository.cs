namespace VocabBuilder.Infrastructure.Repositories.Phonetic;

public class PhoneticRepository : IPhoneticRepository
{
    private static readonly Dictionary<string, string> CommonPhonetics = new()
    {
        // Articles & Determiners
        { "a", "ə" },
        { "an", "ən" },
        { "the", "ðə" },

        // Prepositions (rất phổ biến trong idioms/phrasal verbs)
        { "in", "ɪn" },
        { "on", "ɑn" },
        { "at", "ət" },
        { "to", "tə" },
        { "for", "fər" },
        { "of", "əv" },
        { "by", "baɪ" },
        { "with", "wɪð" },
        { "about", "əˈbaʊt" },
        { "over", "ˈoʊvɚ" },
        { "under", "ˈʌndɚ" },
        { "through", "θɹuː" },
        { "off", "ɔf" },
        { "out", "aʊt" },
        { "up", "ʌp" },
        { "down", "daʊn" },
        { "around", "əˈɹaʊnd" },
        { "into", "ˈɪntu" },
        { "onto", "ˈɑntu" },
        { "across", "əˈkɹɑs" },
        { "behind", "bɪˈhaɪnd" },

        // Pronouns
        { "i", "aɪ" },
        { "you", "ju" },
        { "he", "hi" },
        { "she", "ʃi" },
        { "we", "wi" },
        { "they", "ðeɪ" },
        { "me", "mi" },
        { "him", "hɪm" },
        { "her", "hɚ" },
        { "us", "əs" },
        { "them", "ðəm" },
        { "my", "maɪ" },
        { "your", "jɚ" },
        { "their", "ðɛɹ" },
        { "our", "ɑɹ" },

        // Conjunctions
        { "and", "ənd" },
        { "or", "ɚ" },
        { "but", "bət" },
        { "if", "ɪf" },
        { "as", "əz" },
        { "than", "ðən" },
        { "though", "ðoʊ" },
        { "because", "bɪˈkəz" },

        // Auxiliaries / Modals (rất hay dùng trong cấu trúc)
        { "be", "bi" },
        { "am", "əm" },
        { "is", "ɪz" },
        { "are", "ɚ" },
        { "was", "wəz" },
        { "were", "wɚ" },
        { "have", "həv" },
        { "has", "həz" },
        { "had", "həd" },
        { "do", "du" },
        { "does", "dəz" },
        { "did", "dɪd" },
        { "can", "kən" },
        { "could", "kəd" },
        { "should", "ʃəd" },
        { "would", "wəd" },
        { "will", "wɪl" },
        { "might", "maɪt" },
        { "must", "məst" },

        // Verbs thường gặp trong idioms/collocations
        { "make", "meɪk" },
        { "take", "teɪk" },
        { "get", "ɡɛt" },
        { "give", "ɡɪv" },
        { "go", "ɡoʊ" },
        { "come", "kʌm" },
        { "put", "pʊt" },
        { "let", "lɛt" },
        { "bring", "bɹɪŋ" },
        { "keep", "kiːp" },
        { "hold", "hoʊld" },
        { "turn", "tɝn" },
        { "fall", "fɔl" },
        { "run", "ɹʌn" },
        { "call", "kɔl" },
        { "set", "sɛt" },
        { "come", "kʌm" },

        // Nouns thường thấy trong idioms
        { "mind", "maɪnd" },
        { "heart", "hɑɹt" },
        { "face", "feɪs" },
        { "hand", "hænd" },
        { "head", "hɛd" },
        { "back", "bæk" },
        { "bit", "bɪt" },
        { "piece", "pis" },
        { "way", "weɪ" },
        { "place", "pleɪs" },
        { "hell", "hɛl" },
        { "time", "taɪm" },
        { "day", "deɪ" },
        { "end", "ɛnd" },
        { "chance", "tʃæns" },
        { "point", "pɔɪnt" },
        { "line", "laɪn" },
        { "sight", "saɪt" },

        // Adverbs thường đi trong collocation
        { "just", "dʒəst" },
        { "really", "ˈɹɪəli" },
        { "still", "stɪl" },
        { "only", "ˈoʊnli" },
        { "too", "tu" },
        { "even", "ˈivən" },
        { "already", "ɔlˈɹɛdi" },
        { "always", "ˈɔlweɪz" },
        { "never", "ˈnɛvɚ" }
    };

    public string? GetPhonetic(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return null;

        return CommonPhonetics.TryGetValue(word.ToLowerInvariant(), out var phonetic)
            ? phonetic
            : null;
    }

    public bool IsCommonWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        return CommonPhonetics.ContainsKey(word.ToLowerInvariant());
    }

    public IReadOnlyDictionary<string, string> GetAllCommonWords()
    {
        return CommonPhonetics.AsReadOnly();
    }
}