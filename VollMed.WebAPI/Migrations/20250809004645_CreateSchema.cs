using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VollMed.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "medicos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Crm = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: false),
                    Especialidade = table.Column<int>(type: "int", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "consultas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Paciente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MedicoId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_consultas_medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_consultas_MedicoId",
                table: "consultas",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_medicos_Crm",
                table: "medicos",
                column: "Crm",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_medicos_Email",
                table: "medicos",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consultas");

            migrationBuilder.DropTable(
                name: "medicos");
        }
    }
}
