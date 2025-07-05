using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaboresDoCerrado.ApiAutenticacao.Net.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaIndicesUnicosParaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "tbl_usuario",
                newName: "NomeCompleto");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "tbl_usuario",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NomeUsuario",
                table: "tbl_usuario",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_usuario_Email",
                table: "tbl_usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_usuario_NomeUsuario",
                table: "tbl_usuario",
                column: "NomeUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_usuario_Email",
                table: "tbl_usuario");

            migrationBuilder.DropIndex(
                name: "IX_tbl_usuario_NomeUsuario",
                table: "tbl_usuario");

            migrationBuilder.DropColumn(
                name: "NomeUsuario",
                table: "tbl_usuario");

            migrationBuilder.RenameColumn(
                name: "NomeCompleto",
                table: "tbl_usuario",
                newName: "Nome");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "tbl_usuario",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
