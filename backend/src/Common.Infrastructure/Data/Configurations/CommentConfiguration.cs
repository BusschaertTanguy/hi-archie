using Core.Domain.Comments.Entities;
using Core.Domain.Posts.Entities;
using Core.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Data.Configurations;

internal sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(10_000);

        builder.Property(p => p.PublishDate)
            .IsRequired();

        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(p => p.PostId)
            .IsRequired();

        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(p => p.ParentId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .IsRequired();
    }
}