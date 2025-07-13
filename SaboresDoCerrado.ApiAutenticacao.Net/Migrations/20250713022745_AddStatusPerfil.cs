using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaboresDoCerrado.ApiAutenticacao.Net.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "tbl_perfil",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "tbl_perfil");
        }
    }
}
