using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BlindChat.Infrastructure.Migrations
{
    public partial class CreateBlindParticipanttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessages_Participants_ParticipantId",
                table: "ConversationMessages");

            migrationBuilder.DropIndex(
                name: "IX_ConversationMessages_ParticipantId",
                table: "ConversationMessages");

            migrationBuilder.DropColumn(
                name: "PublicKey",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "ConversationMessages");

            migrationBuilder.CreateTable(
                name: "BlindParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<Guid>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlindParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlindParticipants_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlindParticipants_GroupId",
                table: "BlindParticipants",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlindParticipants");

            migrationBuilder.AddColumn<string>(
                name: "PublicKey",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParticipantId",
                table: "ConversationMessages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_ParticipantId",
                table: "ConversationMessages",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessages_Participants_ParticipantId",
                table: "ConversationMessages",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
