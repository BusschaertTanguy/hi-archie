using Core.Domain.Communities.Entities;
using Core.Domain.Posts.Entities;
using Core.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.EntityFramework.Configurations;

internal sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(10_000);

        builder.Property(p => p.PublishDate)
            .IsRequired();
        
        builder.HasOne<Community>()
            .WithMany()
            .HasForeignKey(p => p.CommunityId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .IsRequired();
    }
}