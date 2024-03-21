using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sigma.Core.Migrations
{
    /// <inheritdoc />
    public partial class UseIntentionRecognition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Apps",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "UseIntentionRecognition",
                table: "AIModels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseIntentionRecognition",
                table: "AIModels");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Apps",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
