using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BlindChat.Infrastructure.Migrations
{
    public partial class enablecolumnsintableauthmessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSigned",
                table: "AuthenticationMessages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "AuthenticationMessages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSigned",
                table: "AuthenticationMessages");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "AuthenticationMessages");
        }
    }
}
