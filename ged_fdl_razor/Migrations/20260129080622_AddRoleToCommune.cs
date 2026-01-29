using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ged_fdl_razor.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleToCommune : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Communes",
                columns: table => new
                {
                    CommuneID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    MustChangePassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    District = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communes", x => x.CommuneID);
                });

            migrationBuilder.CreateTable(
                name: "DossiersFinancement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateSoumission = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    CommuneId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateValidation = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DossiersFinancement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DossiersFinancement_Communes_CommuneId",
                        column: x => x.CommuneId,
                        principalTable: "Communes",
                        principalColumn: "CommuneID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    Taille = table.Column<long>(type: "INTEGER", nullable: false),
                    Contenu = table.Column<byte[]>(type: "BLOB", nullable: false),
                    DossierFinancementId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateUpload = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_Documents_DossiersFinancement_DossierFinancementId",
                        column: x => x.DossierFinancementId,
                        principalTable: "DossiersFinancement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Remarques",
                columns: table => new
                {
                    RemarqueID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Texte = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DossierFinancementId = table.Column<int>(type: "INTEGER", nullable: false),
                    CommuneId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remarques", x => x.RemarqueID);
                    table.ForeignKey(
                        name: "FK_Remarques_Communes_CommuneId",
                        column: x => x.CommuneId,
                        principalTable: "Communes",
                        principalColumn: "CommuneID");
                    table.ForeignKey(
                        name: "FK_Remarques_DossiersFinancement_DossierFinancementId",
                        column: x => x.DossierFinancementId,
                        principalTable: "DossiersFinancement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DossierFinancementId",
                table: "Documents",
                column: "DossierFinancementId");

            migrationBuilder.CreateIndex(
                name: "IX_DossiersFinancement_CommuneId",
                table: "DossiersFinancement",
                column: "CommuneId");

            migrationBuilder.CreateIndex(
                name: "IX_Remarques_CommuneId",
                table: "Remarques",
                column: "CommuneId");

            migrationBuilder.CreateIndex(
                name: "IX_Remarques_DossierFinancementId",
                table: "Remarques",
                column: "DossierFinancementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Remarques");

            migrationBuilder.DropTable(
                name: "DossiersFinancement");

            migrationBuilder.DropTable(
                name: "Communes");
        }
    }
}
