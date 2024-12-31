using Core.Domain.Posts.Entities;
using Core.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.EntityFramework.Configurations;

internal sealed class PostVoteConfiguration : IEntityTypeConfiguration<PostVote>
{
    public void Configure(EntityTypeBuilder<PostVote> builder)
    {
        builder.HasKey(pv => new { pv.PostId, pv.UserId });

        builder.HasOne<Post>()
            .WithMany(pv => pv.Votes)
            .HasForeignKey(pv => pv.PostId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(pv => pv.UserId)
            .IsRequired();
    }
}