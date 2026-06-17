using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Role = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false, defaultValue: "User"),
                    RefreshToken = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
