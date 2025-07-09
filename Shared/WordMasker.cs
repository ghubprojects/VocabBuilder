namespace VocabBuilder.Shared;

public static class WordMasker
{
    /// <summary>
    /// Masks a word by replacing all vowels with underscores.
    /// Eg: "anticipate" => "_nt_c_p_t_"
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static string MaskVowels(string word)
    {
        var vowels = "aeiouAEIOU";
        return new string(word.Select(c => vowels.Contains(c) ? '_' : c).ToArray());
    }

    /// <summary>
    /// Masks a word by replacing a percentage of its characters with underscores.
    /// Eg: "anticipate" => "a_ti_ip_te" (random)
    /// </summary>
    /// <param name="word"></param>
    /// <param name="percentToMask"></param>
    /// <returns></returns>
    public static string MaskRandom(string word, double percentToMask = 0.4)
    {
        if (word.Length <= 2)
            return word; // avoid masking too much

        var rng = new Random();
        var chars = word.ToCharArray();
        var indexes = Enumerable.Range(1, word.Length - 2).OrderBy(_ => rng.Next()).Take((int)(word.Length * percentToMask));

        foreach (var i in indexes)
            chars[i] = '_';

        return new string(chars);
    }

    /// <summary>
    /// Masks a word by replacing vowels with underscores and applying light random masking.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static string MaskHybrid(string word)
    {
        var baseMasked = MaskVowels(word);
        return MaskRandom(baseMasked, 0.2);
    }
}