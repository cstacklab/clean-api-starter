namespace CleanApiStarter.Infrastructure.Persistence.Configuration;

public sealed class WordConfiguration : IEntityTypeConfiguration<Word>
{
    public void Configure(EntityTypeBuilder<Word> builder)
    {
        builder.ToTable("words");

        builder.HasKey(word => word.Id);

        builder.Property(word => word.Id)
            .HasColumnName("id");

        builder.Property(word => word.Text)
            .HasColumnName("text")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(word => word.Meaning)
            .HasColumnName("meaning")
            .IsRequired();

        builder.Property(word => word.Synonyms)
            .HasColumnName("synonyms")
            .HasColumnType("jsonb")
            .HasConversion(
                synonyms => JsonSerializer.Serialize(synonyms, JsonSerializerOptions.Default),
                value => JsonSerializer.Deserialize<List<string>>(value, JsonSerializerOptions.Default) ?? new List<string>());

        builder.Property(word => word.UsageExample)
            .HasColumnName("usage_example")
            .IsRequired();

        builder.Property(word => word.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(word => word.UpdatedAt)
            .HasColumnName("updated_at");
    }
}