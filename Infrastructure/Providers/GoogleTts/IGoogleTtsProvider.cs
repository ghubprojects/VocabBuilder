namespace VocabBuilder.Infrastructure.Providers.GoogleTts;

public interface IGoogleTtsProvider
{
    Task<string> GenerateAudioAsync(string text);
}
