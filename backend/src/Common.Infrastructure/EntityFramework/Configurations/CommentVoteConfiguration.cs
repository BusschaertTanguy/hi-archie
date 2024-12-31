using Core.Domain.Comments.Entities;
using Core.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.EntityFramework.Configurations;

internal sealed class CommentVoteConfiguration : IEntityTypeConfiguration<CommentVote>
{
    public void Configure(EntityTypeBuilder<CommentVote> builder)
    {
        builder.HasKey(cv => new { cv.CommentId, cv.UserId });

        builder.HasOne<Comment>()
            .WithMany(cv => cv.Votes)
            .HasForeignKey(cv => cv.CommentId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(cv => cv.UserId)
            .IsRequired();
    }
}