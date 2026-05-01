namespace CleanApiStarter.Infrastructure.Persistence.Configuration;

public sealed class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("project_tasks");

        builder.HasKey(task => task.Id);

        builder.Property(task => task.Id)
            .HasColumnName("id");

        builder.Property(task => task.ProjectId)
            .HasColumnName("project_id");

        builder.Property(task => task.Title)
            .HasColumnName("title")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(task => task.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(task => task.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(task => task.DueDate)
            .HasColumnName("due_date");

        builder.Property(task => task.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(task => task.CompletedAt)
            .HasColumnName("completed_at");

        builder.HasOne(task => task.Project)
            .WithMany(project => project.Tasks)
            .HasForeignKey(task => task.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}