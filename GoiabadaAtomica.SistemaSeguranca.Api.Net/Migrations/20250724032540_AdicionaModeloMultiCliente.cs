using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoiabadaAtomica.ApiAutenticacao.Net.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaModeloMultiCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "tbl_roles",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "tbl_features",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "tbl_client_systems",
                newName: "IsActive");

            migrationBuilder.AlterColumn<int>(
                name: "ClientSystemId",
                table: "tbl_features",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tbl_authentication_providers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConfigurationJson = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ClientSystemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_authentication_providers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_authentication_providers_tbl_client_systems_ClientSystem~",
                        column: x => x.ClientSystemId,
                        principalTable: "tbl_client_systems",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tbl_password_policies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinLength = table.Column<int>(type: "int", nullable: false),
                    RequiresUppercase = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequiresLowercase = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequiresDigit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequiresSpecialChar = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHistoryCount = table.Column<int>(type: "int", nullable: false),
                    ExpirationDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_password_policies", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tbl_role_x_feature",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    ClientSystemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_role_x_feature", x => new { x.RoleId, x.FeatureId, x.ClientSystemId });
                    table.ForeignKey(
                        name: "FK_tbl_role_x_feature_tbl_client_systems_ClientSystemId",
                        column: x => x.ClientSystemId,
                        principalTable: "tbl_client_systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_role_x_feature_tbl_features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "tbl_features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_role_x_feature_tbl_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tbl_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tbl_user_x_provider",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AuthenticationProviderId = table.Column<int>(type: "int", nullable: false),
                    ExternalIdentifier = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user_x_provider", x => new { x.UserId, x.AuthenticationProviderId });
                    table.ForeignKey(
                        name: "FK_tbl_user_x_provider_tbl_authentication_providers_Authenticat~",
                        column: x => x.AuthenticationProviderId,
                        principalTable: "tbl_authentication_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_user_x_provider_tbl_users_UserId",
                        column: x => x.UserId,
                        principalTable: "tbl_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_features_ClientSystemId",
                table: "tbl_features",
                column: "ClientSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_authentication_providers_ClientSystemId",
                table: "tbl_authentication_providers",
                column: "ClientSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_authentication_providers_Name",
                table: "tbl_authentication_providers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_role_x_feature_ClientSystemId",
                table: "tbl_role_x_feature",
                column: "ClientSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_role_x_feature_FeatureId",
                table: "tbl_role_x_feature",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_x_provider_AuthenticationProviderId",
                table: "tbl_user_x_provider",
                column: "AuthenticationProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_features_tbl_client_systems_ClientSystemId",
                table: "tbl_features",
                column: "ClientSystemId",
                principalTable: "tbl_client_systems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_features_tbl_client_systems_ClientSystemId",
                table: "tbl_features");

            migrationBuilder.DropTable(
                name: "tbl_password_policies");

            migrationBuilder.DropTable(
                name: "tbl_role_x_feature");

            migrationBuilder.DropTable(
                name: "tbl_user_x_provider");

            migrationBuilder.DropTable(
                name: "tbl_authentication_providers");

            migrationBuilder.DropIndex(
                name: "IX_tbl_features_ClientSystemId",
                table: "tbl_features");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "tbl_roles",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "tbl_features",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "tbl_client_systems",
                newName: "Status");

            migrationBuilder.UpdateData(
                table: "tbl_features",
                keyColumn: "ClientSystemId",
                keyValue: null,
                column: "ClientSystemId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ClientSystemId",
                table: "tbl_features",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
