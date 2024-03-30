using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sigma.Core.Migrations
{
    /// <inheritdoc />
    public partial class Plugin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apis");

            migrationBuilder.RenameColumn(
                name: "ApiFunctionList",
                table: "Apps",
                newName: "PluginList");

            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Describe = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Method = table.Column<int>(type: "INTEGER", nullable: false),
                    Header = table.Column<string>(type: "varchar(1000)", nullable: true),
                    Query = table.Column<string>(type: "varchar(1000)", nullable: true),
                    JsonBody = table.Column<string>(type: "varchar(7000)", nullable: true),
                    InputPrompt = table.Column<string>(type: "varchar(1500)", nullable: true),
                    OutputPrompt = table.Column<string>(type: "varchar(1500)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeletedBy = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.RenameColumn(
                name: "PluginList",
                table: "Apps",
                newName: "ApiFunctionList");

            migrationBuilder.CreateTable(
                name: "Apis",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeletedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Describe = table.Column<string>(type: "TEXT", nullable: false),
                    Header = table.Column<string>(type: "varchar(1000)", nullable: true),
                    InputPrompt = table.Column<string>(type: "varchar(1500)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    JsonBody = table.Column<string>(type: "varchar(7000)", nullable: true),
                    Method = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OutputPrompt = table.Column<string>(type: "varchar(1500)", nullable: true),
                    Query = table.Column<string>(type: "varchar(1000)", nullable: true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apis", x => x.Id);
                });
        }
    }
}
