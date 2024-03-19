using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sigma.Core.Migrations
{
    /// <inheritdoc />
    public partial class changekms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatModelID",
                table: "Kms");

            migrationBuilder.AlterColumn<string>(
                name: "ChatModelID",
                table: "Apps",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "EmbeddingModelID",
                table: "Apps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EndPoint",
                table: "AIModels",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmbeddingModelID",
                table: "Apps");

            migrationBuilder.AddColumn<string>(
                name: "ChatModelID",
                table: "Kms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ChatModelID",
                table: "Apps",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EndPoint",
                table: "AIModels",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
