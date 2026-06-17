namespace CleanApiStarter.Api.Infrastructure.Persistence.Configuration;

public sealed class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("project_members");

        builder.HasKey(member => new
        {
            member.ProjectId,
            member.UserId
        });

        builder.Property(member => member.ProjectId)
            .HasColumnName("project_id");

        builder.Property(member => member.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(450);

        builder.Property(member => member.CreatedAt)
            .HasColumnName("created_at");

        builder.HasOne(member => member.Project)
            .WithMany(project => project.Members)
            .HasForeignKey(member => member.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
