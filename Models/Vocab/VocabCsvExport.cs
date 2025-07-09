using VocabBuilder.Infrastructure.Entities.Vocab;
using VocabBuilder.Shared;

namespace VocabBuilder.Models.Vocab;

public class VocabCsvExport
{
    public int Id { get; set; }

    public string Word { get; set; } = string.Empty;

    public string WordType { get; set; } = string.Empty;

    public string Meaning { get; set; } = string.Empty;

    public string Phonetic { get; set; } = string.Empty;

    public string Definition { get; set; } = string.Empty;

    public string Example { get; set; } = string.Empty;

    public string MaskedWord { get; set; } = string.Empty;

    public string Audio { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public static VocabCsvExport FromEntity(VocabEntity entity) => new()
    {
        Id = entity.Id,
        Word = entity.Word,
        WordType = entity.WordType,
        Meaning = entity.Meaning,
        Phonetic = StringHelper.WrapWithSlash(entity.Phonetic),
        Definition = entity.Definition,
        Example = entity.Example,
        MaskedWord = entity.MaskedWord,
        Audio = StringHelper.WrapAudio(entity.Audio),
        Image = StringHelper.WrapImage(entity.Image)
    };
}
