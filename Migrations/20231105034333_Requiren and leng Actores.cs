using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasApi.Migrations
{
    /// <inheritdoc />
    public partial class RequirenandlengActores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Actores",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Actores",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Actores",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Actores",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);
        }
    }
}
