﻿// <auto-generated />
using BlindChat.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace BlindChat.Infrastructure.Migrations
{
    [DbContext(typeof(BlindChatDbContext))]
    [Migration("20180504122254_add nickname to blind participant")]
    partial class addnicknametoblindparticipant
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BlindChatCore.Model.AuthenticationMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("GroupId");

                    b.Property<bool?>("IsSigned");

                    b.Property<string>("Message");

                    b.Property<int?>("ParticipantId");

                    b.Property<string>("Signature");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("AuthenticationMessages");
                });

            modelBuilder.Entity("BlindChatCore.Model.BlindParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("GroupId");

                    b.Property<string>("NickName");

                    b.Property<string>("PublicKey");

                    b.Property<string>("Signature");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("BlindParticipants");
                });

            modelBuilder.Entity("BlindChatCore.Model.ConfirmationCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Code");

                    b.Property<Guid?>("GroupId");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("ConfirmationCodes");
                });

            modelBuilder.Entity("BlindChatCore.Model.ConversationMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("GroupId");

                    b.Property<string>("Message");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("ConversationMessages");
                });

            modelBuilder.Entity("BlindChatCore.Model.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("OwnerEmail");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("BlindChatCore.Model.Participant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<bool>("EmailIsAlreadyInvited");

                    b.Property<bool>("EmailIsConfirmed");

                    b.Property<Guid?>("GroupId");

                    b.Property<int>("InvitationCode");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("BlindChatCore.Model.AuthenticationMessage", b =>
                {
                    b.HasOne("BlindChatCore.Model.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");

                    b.HasOne("BlindChatCore.Model.Participant", "Participant")
                        .WithMany()
                        .HasForeignKey("ParticipantId");
                });

            modelBuilder.Entity("BlindChatCore.Model.BlindParticipant", b =>
                {
                    b.HasOne("BlindChatCore.Model.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("BlindChatCore.Model.ConfirmationCode", b =>
                {
                    b.HasOne("BlindChatCore.Model.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("BlindChatCore.Model.ConversationMessage", b =>
                {
                    b.HasOne("BlindChatCore.Model.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("BlindChatCore.Model.Participant", b =>
                {
                    b.HasOne("BlindChatCore.Model.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });
#pragma warning restore 612, 618
        }
    }
}
