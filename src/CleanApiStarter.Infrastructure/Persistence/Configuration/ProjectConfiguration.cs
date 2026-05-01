namespace CleanApiStarter.Infrastructure.Persistence.Configuration;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Id)
            .HasColumnName("id");

        builder.Property(project => project.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(project => project.OwnerUserId)
            .HasColumnName("owner_user_id")
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(project => project.CreatedAt)
            .HasColumnName("created_at");
    }
}