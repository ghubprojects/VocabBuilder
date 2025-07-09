using System.Diagnostics;
using VocabBuilder.Shared;

namespace VocabBuilder.Infrastructure.Providers.GoogleTts;

public class GoogleTtsProvider(IWebHostEnvironment environment) : IGoogleTtsProvider
{
    public async Task<string> GenerateAudioAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Console.Error.WriteLine("Text cannot be empty.");
            return string.Empty;
        }

        var fileName = FileHelper.NormalizeFileName(text, "mp3");
        var filePath = FileHelper.GetFullTempUploadPath(fileName, environment.WebRootPath);

        string pythonScript = Path.Combine(environment.WebRootPath, "scripts", "google-tts.py");
        string arguments = $"\"{text}\" \"{filePath}\"";

        var psi = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"{pythonScript} {arguments}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            Console.Error.WriteLine($"Python error: {error}");
            return string.Empty;
        }

        Console.WriteLine(output);
        return fileName;
    }
}
