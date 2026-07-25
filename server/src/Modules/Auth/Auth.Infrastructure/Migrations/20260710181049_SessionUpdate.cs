using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SessionUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refresh_token",
                schema: "auth",
                table: "sessions");

            migrationBuilder.AddColumn<string>(
                name: "ip_address",
                schema: "auth",
                table: "sessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_agent",
                schema: "auth",
                table: "sessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ip_address",
                schema: "auth",
                table: "sessions");

            migrationBuilder.DropColumn(
                name: "user_agent",
                schema: "auth",
                table: "sessions");

            migrationBuilder.AddColumn<Guid>(
                name: "refresh_token",
                schema: "auth",
                table: "sessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
