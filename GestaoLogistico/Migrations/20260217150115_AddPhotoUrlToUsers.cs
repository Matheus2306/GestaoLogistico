using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoLogistico.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoUrlToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlFoto",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlFoto",
                table: "Usuarios");
        }
    }
}
