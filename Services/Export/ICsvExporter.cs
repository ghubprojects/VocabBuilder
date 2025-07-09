namespace VocabBuilder.Services.Export;

public interface ICsvExporter
{
    byte[] Export<T>(IEnumerable<T> data);
}