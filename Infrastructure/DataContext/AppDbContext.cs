using Microsoft.EntityFrameworkCore;
using VocabBuilder.Infrastructure.Entities.Vocab;

namespace VocabBuilder.Infrastructure.DataContext;

public partial class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public virtual DbSet<VocabEntity> Vocabs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VocabEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vocab_pk");

            entity.ToTable("vocab");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Word).HasColumnName("word");
            entity.Property(e => e.WordType).HasColumnName("word_type");
            entity.Property(e => e.Phonetic).HasColumnName("phonetic");
            entity.Property(e => e.MaskedWord).HasColumnName("masked_word");
            entity.Property(e => e.Meaning).HasColumnName("meaning");
            entity.Property(e => e.Definition).HasColumnName("definition");
            entity.Property(e => e.Example).HasColumnName("example");
            entity.Property(e => e.Audio).HasColumnName("audio");
            entity.Property(e => e.Image).HasColumnName("image");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()").HasColumnType("timestamp without time zone").HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone").HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone").HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");

            entity.Property(e => e.IsDeleted).HasDefaultValue(false).HasColumnName("is_deleted");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
