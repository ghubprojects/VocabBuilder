using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;

namespace VocabBuilder.Shared;

public static class FileHelper
{
    private const string UploadFolder = "uploads";
    private const string TempFolder = "tmp";

    public static string GetFullPath(string path, string webRootPath) =>
        Path.Combine(webRootPath, path);

    public static string GetRelativePath(string path, string webRootPath) =>
        Path.GetRelativePath(webRootPath, path);

    public static string GetFullTempUploadPath(string fileName, string webRootPath) =>
        Path.Combine(webRootPath, GetTempUploadPath(fileName));

    public static string GetTempUploadPath(string filename) =>
        $"{UploadFolder}\\{TempFolder}\\{filename}";

    public static string GetFullUploadPath(string fileName, string webRootPath) =>
        Path.Combine(webRootPath, GetUploadPath(fileName));

    public static string GetUploadPath(string filename) =>
        $"{UploadFolder}\\{filename}";

    /// <summary>
    /// Normalizes a file name by removing invalid characters, converting to lowercase, 
    /// appending an optional prefix, a UTC timestamp, and ensuring the correct file extension.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="extension"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static string NormalizeFileName(string fileName, string extension = "", string prefix = "")
    {
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = "file";
        var name = Regex.Replace(
            Path.GetFileNameWithoutExtension(fileName).Trim().ToLowerInvariant(),
            @"[^\w\-]+", "_"
        );

        var ext = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(ext))
            ext = extension;
        if (!string.IsNullOrWhiteSpace(ext) && !ext.StartsWith("."))
            ext = "." + ext;

        var ts = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff");
        var pre = string.IsNullOrWhiteSpace(prefix) ? "" : $"{prefix}_";

        return $"{pre}{name}_{ts}{ext}";
    }

    /// <summary>
    /// Checks if the file type is accepted based on the provided accept string.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="accept"></param>
    /// <returns></returns>
    public static bool IsAcceptedFileType(string fileName, string accept)
    {
        var acceptedTypes = accept.Split(',')
            .Select(a => a.Trim())
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .ToList();

        var contentTypeProvider = new FileExtensionContentTypeProvider();
        if (!contentTypeProvider.TryGetContentType(fileName, out var contentType))
            contentType = "application/octet-stream"; // fallback

        return acceptedTypes.Any(type =>
            type.StartsWith('.') && type == Path.GetExtension(fileName)
            || type.EndsWith("/*") && contentType.StartsWith(type[..^1])
            || type == contentType
        );
    }
}
