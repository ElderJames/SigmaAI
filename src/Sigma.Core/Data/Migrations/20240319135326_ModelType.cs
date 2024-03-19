using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sigma.Core.Migrations
{
    /// <inheritdoc />
    public partial class ModelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AIModelType",
                table: "AIModels",
                newName: "IsEmbedding");

            migrationBuilder.AddColumn<bool>(
                name: "IsChat",
                table: "AIModels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChat",
                table: "AIModels");

            migrationBuilder.RenameColumn(
                name: "IsEmbedding",
                table: "AIModels",
                newName: "AIModelType");
        }
    }
}
