using Core.Domain.Communities.Entities;
using Core.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Data.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => new { s.CommunityId, s.UserId });

        builder.HasOne<Community>()
            .WithMany()
            .HasForeignKey(s => s.CommunityId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .IsRequired();
    }
}