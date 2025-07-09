namespace VocabBuilder.Shared;

public static class DownloadHelper
{
    public static async Task<string> DownloadUrlToTempFileAsync(HttpClient httpClient, string url, string webRootPath, string fileName)
    {
        try
        {
            var cleanUrl = url.Split('?')[0];
            var extension = Path.GetExtension(cleanUrl).TrimStart('.');
            var normalizedFileName = FileHelper.NormalizeFileName(fileName, extension);
            var tempPath = FileHelper.GetFullTempUploadPath(normalizedFileName, webRootPath);

            var data = await httpClient.GetByteArrayAsync(cleanUrl);
            await File.WriteAllBytesAsync(tempPath, data);

            return FileHelper.GetRelativePath(tempPath, webRootPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] DownloadUrlToTempFileAsync failed: {ex.Message}");
            return string.Empty;
        }
    }
}
