﻿// <auto-generated />
using System;
using Common.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Common.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241224095945_CreateCommentVoteTable")]
    partial class CreateCommentVoteTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.Domain.Comments.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(10000)
                        .HasColumnType("character varying(10000)")
                        .HasColumnName("content");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid")
                        .HasColumnName("parent_id");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid")
                        .HasColumnName("post_id");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("publish_date");

                    b.HasKey("Id")
                        .HasName("pk_comment");

                    b.HasIndex("OwnerId")
                        .HasDatabaseName("ix_comment_owner_id");

                    b.HasIndex("ParentId")
                        .HasDatabaseName("ix_comment_parent_id");

                    b.HasIndex("PostId")
                        .HasDatabaseName("ix_comment_post_id");

                    b.ToTable("comment", (string)null);
                });

            modelBuilder.Entity("Core.Domain.Comments.Entities.CommentVote", b =>
                {
                    b.Property<Guid>("CommentId")
                        .HasColumnType("uuid")
                        .HasColumnName("comment_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("CommentId", "UserId")
                        .HasName("pk_comment_vote");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_comment_vote_user_id");

                    b.ToTable("comment_vote", (string)null);
                });

            modelBuilder.Entity("Core.Domain.Communities.Entities.Community", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.HasKey("Id")
                        .HasName("pk_community");

                    b.HasIndex("OwnerId")
                        .HasDatabaseName("ix_community_owner_id");

                    b.ToTable("community", (string)null);
                });

            modelBuilder.Entity("Core.Domain.Communities.Entities.Subscription", b =>
                {
                    b.Property<Guid>("CommunityId")
                        .HasColumnType("uuid")
                        .HasColumnName("community_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("CommunityId", "UserId")
                        .HasName("pk_subscription");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_subscription_user_id");

                    b.ToTable("subscription", (string)null);
                });

            modelBuilder.Entity("Core.Domain.Posts.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("CommunityId")
                        .HasColumnType("uuid")
                        .HasColumnName("community_id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(10000)
                        .HasColumnType("character varying(10000)")
                        .HasColumnName("content");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("publish_date");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_post");

                    b.HasIndex("CommunityId")
                        .HasDatabaseName("ix_post_community_id");

                    b.HasIndex("OwnerId")
                        .HasDatabaseName("ix_post_owner_id");

                    b.ToTable("post", (string)null);
                });

            modelBuilder.Entity("Core.Domain.Posts.Entities.PostVote", b =>
                {
                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid")
                        .HasColumnName("post_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("PostId", "UserId")
                        .HasName("pk_post_vote");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_post_vote_user_id");

                    b.ToTable("post_vote", (string)null);
                });

            modelBuilder.Entity("Core.Domain.Users.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("char(64)")
                        .HasColumnName("external_id");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.HasIndex("ExternalId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_external_id");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("Core.Domain.Comments.Entities.Comment", b =>
                {
                    b.HasOne("Core.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_comment_user_owner_id");

                    b.HasOne("Core.Domain.Comments.Entities.Comment", null)
                        .WithMany()
                        .HasForeignKey("ParentId")
                        .HasConstraintName("fk_comment_comment_parent_id");

                    b.HasOne("Core.Domain.Posts.Entities.Post", null)
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_comment_post_post_id");
                });

            modelBuilder.Entity("Core.Domain.Comments.Entities.CommentVote", b =>
                {
                    b.HasOne("Core.Domain.Comments.Entities.Comment", null)
                        .WithMany("Votes")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_comment_vote_comment_comment_id");

                    b.HasOne("Core.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_comment_vote_user_user_id");
                });

            modelBuilder.Entity("Core.Domain.Communities.Entities.Community", b =>
                {
                    b.HasOne("Core.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_community_user_owner_id");
                });

            modelBuilder.Entity("Core.Domain.Communities.Entities.Subscription", b =>
                {
                    b.HasOne("Core.Domain.Communities.Entities.Community", null)
                        .WithMany()
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_subscription_community_community_id");

                    b.HasOne("Core.Domain.Users.Entities.User", null)
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_subscription_user_user_id");
                });

            modelBuilder.Entity("Core.Domain.Posts.Entities.Post", b =>
                {
                    b.HasOne("Core.Domain.Communities.Entities.Community", null)
                        .WithMany()
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_post_community_community_id");

                    b.HasOne("Core.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_post_user_owner_id");
                });

            modelBuilder.Entity("Core.Domain.Posts.Entities.PostVote", b =>
                {
                    b.HasOne("Core.Domain.Posts.Entities.Post", null)
                        .WithMany("Votes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_post_vote_post_post_id");

                    b.HasOne("Core.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_post_vote_user_user_id");
                });

            modelBuilder.Entity("Core.Domain.Comments.Entities.Comment", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("Core.Domain.Posts.Entities.Post", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("Core.Domain.Users.Entities.User", b =>
                {
                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
