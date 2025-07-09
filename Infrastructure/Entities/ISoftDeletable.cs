namespace VocabBuilder.Infrastructure.Entities;

public interface ISoftDeletable
{
    public DateTime DeletedAt { get; set; }

    public string DeletedBy { get; set; }

    public bool IsDeleted { get; set; }
}
