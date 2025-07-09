#nullable disable

namespace VocabBuilder.Infrastructure.Entities.Vocab;

public partial class VocabEntity : IAuditable, ISoftDeletable
{
    public int Id { get; set; }

    public string Word { get; set; }

    public string WordType { get; set; }

    public string Meaning { get; set; }

    public string Phonetic { get; set; }

    public string Definition { get; set; }

    public string Example { get; set; }

    public string MaskedWord { get; set; }

    public string Audio { get; set; }

    public string Image { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime DeletedAt { get; set; }

    public string DeletedBy { get; set; }

    public bool IsDeleted { get; set; }
}