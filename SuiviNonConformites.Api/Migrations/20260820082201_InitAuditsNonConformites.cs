using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuiviNonConformites.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitAuditsNonConformites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Reference = table.Column<string>(type: "TEXT", nullable: false),
                    Client = table.Column<string>(type: "TEXT", nullable: false),
                    DateAudit = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonConformites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Gravite = table.Column<int>(type: "INTEGER", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    DateEcheance = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuditId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonConformites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonConformites_Audits_AuditId",
                        column: x => x.AuditId, //la colonne de la table NonConformites qui porte la contrainte.
                        principalTable: "Audits",
                        principalColumn: "Id", //elle référence la colonne Id de la table Audits — c'est ça, concrètement, une clé étrangère : une colonne qui doit obligatoirement correspondre à une valeur existante dans une autre table.
                        onDelete: ReferentialAction.Cascade); //le point le plus important à bien comprendre. Ça signifie que si tu supprimes un Audit, toutes ses NonConformite liées seront automatiquement supprimées aussi par la base de données elle-même, sans que ton code C# ait besoin de le faire explicitement. C'est un comportement choisi par défaut par EF Core (convention), pas quelque chose que tu as demandé explicitement dans OnModelCreating — il l'a déduit du fait que la relation est obligatoire (AuditId n'est pas nullable).
                });

            migrationBuilder.CreateIndex(
                name: "IX_NonConformites_AuditId",
                table: "NonConformites",
                column: "AuditId"); //Un index sur la colonne AuditId — généré automatiquement par convention dès qu'une colonne sert de clé étrangère. Son rôle : accélérer les requêtes qui filtrent ou joignent sur cette colonne (typiquement : "donne-moi toutes les non-conformités de l'audit n°3"), en évitant à la base de scanner ligne par ligne toute la table. Tu n'as rien eu à faire pour l't'obtenir, c'est un comportement standard d'EF Core sur les FK.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NonConformites");

            migrationBuilder.DropTable(
                name: "Audits");
        }
    }
}
