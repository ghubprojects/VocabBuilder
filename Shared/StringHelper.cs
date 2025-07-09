namespace VocabBuilder.Shared;

public static class StringHelper
{
    public static string WrapWithSlash(string text)
    {
        return $"/{text.Trim('/')}/";
    }

    public static string UnwrapSlashes(string text)
    {
        return text.Trim('/').Trim();
    }

    public static string WrapImage(string filename)
    {
        return $"<img src=\"{filename}\" />";
    }

    public static string WrapAudio(string filename)
    {
        return $"[sound:{filename}]";
    }
}
