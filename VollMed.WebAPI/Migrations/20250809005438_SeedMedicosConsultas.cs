using Microsoft.EntityFrameworkCore.Migrations;

namespace VollMed.WebAPI.Migrations
{
    public partial class SeedMedicosConsultas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Populando a tabela Medicos
            migrationBuilder.InsertData(
                table: "Medicos",
                columns: ["Nome", "Email", "Telefone", "Crm", "Especialidade"],
                columnTypes: ["string", "string", "string", "string", "int"],
                values: new object[,]
                {
                { "Gregory House", "house@hospital.com", "(12)12345-6781", "123456", 6 }, // Diagnóstico
                { "Meredith Grey", "meredith@hospital.com", "(12)12345-6782", "654321", 3 }, // Cirurgia Geral
                { "John Carter", "carter@hospital.com", "(12)12345-6783", "234567", 4 }, // Pediatria
                { "Derek Shepherd", "derek@hospital.com", "(12)12345-6784", "345678", 2 }, // Neurocirurgia
                { "Cristina Yang", "cristina@hospital.com", "(12)12345-6785", "456789", 1 }, // Cardiologia
                { "Alex Karev", "alex@hospital.com", "(12)12345-6786", "567890", 4 }, // Pediatria
                { "Izzie Stevens", "izzie@hospital.com", "(12)12345-6787", "678901", 5 }, // Oncologia
                { "Miranda Bailey", "miranda@hospital.com", "(12)12345-6788", "789012", 3 }, // Cirurgia Geral
                { "George O'Malley", "george@hospital.com", "(12)12345-6789", "890123", 3 }, // Cirurgia Geral
                { "Jackson Avery", "jackson@hospital.com", "(12)12345-6790", "901234", 1 }, // Cardiologia
                { "April Kepner", "april@hospital.com", "(12)12345-6791", "012345", 4 }, // Pediatria
                { "Mark Sloan", "mark@hospital.com", "(12)12345-6792", "123789", 3 }, // Cirurgia Geral
                { "Addison Montgomery", "addison@hospital.com", "(12)12345-6793", "234891", 4 }, // Pediatria
                { "Arizona Robbins", "arizona@hospital.com", "(12)12345-6794", "345912", 4 }, // Pediatria
                { "Owen Hunt", "owen@hospital.com", "(12)12345-6795", "456023", 3 }, // Cirurgia Geral
                { "James Wilson", "wilson@hospital.com", "(12)12345-6796", "567134", 5 }, // Oncologia
                { "Allison Cameron", "cameron@hospital.com", "(12)12345-6797", "678245", 6 }, // Diagnóstico
                { "Robert Chase", "chase@hospital.com", "(12)12345-6798", "789356", 6 }, // Diagnóstico
                { "Eric Foreman", "foreman@hospital.com", "(12)12345-6799", "890467", 6 }, // Diagnóstico
                { "Richard Webber", "webber@hospital.com", "(12)12345-6800", "901578", 3 }  // Cirurgia Geral

                });

            // Populando a tabela Consultas com associações fictícias com a tabela Medicos
            migrationBuilder.InsertData(
            table: "Consultas",
            columns: ["MedicoId", "Paciente", "Data"],
            columnTypes: ["int", "string", "DateTime"],
            values: new object[,]
            {
                { 1, "38492017562", new DateTime(2025, 5, 3, 9, 0, 0) },
                { 2, "92837461502", new DateTime(2025, 5, 7, 13, 0, 0) },
                { 3, "10293847561", new DateTime(2025, 5, 12, 15, 0, 0) },
                { 4, "56473829104", new DateTime(2025, 5, 15, 9, 0, 0) },
                { 5, "84736291028", new DateTime(2025, 5, 20, 13, 0, 0) },
                { 6, "29384756109", new DateTime(2025, 5, 25, 15, 0, 0) },
                { 7, "47583920176", new DateTime(2025, 5, 30, 9, 0, 0) },
                { 8, "91827364501", new DateTime(2025, 6, 2, 13, 0, 0) },
                { 9, "38475619203", new DateTime(2025, 6, 7, 15, 0, 0) },
                { 10, "56473829105", new DateTime(2025, 6, 12, 9, 0, 0) },
                { 1, "84736291029", new DateTime(2025, 6, 17, 13, 0, 0) },
                { 2, "29384756110", new DateTime(2025, 6, 22, 15, 0, 0) },
                { 3, "47583920177", new DateTime(2025, 6, 27, 9, 0, 0) },
                { 4, "91827364502", new DateTime(2025, 7, 2, 13, 0, 0) },
                { 5, "38475619204", new DateTime(2025, 7, 7, 15, 0, 0) },
                { 6, "56473829106", new DateTime(2025, 7, 12, 9, 0, 0) },
                { 7, "84736291030", new DateTime(2025, 7, 17, 13, 0, 0) },
                { 8, "29384756111", new DateTime(2025, 7, 22, 15, 0, 0) },
                { 1, "38475619205", new DateTime(2025, 8, 7, 15, 0, 0) },
                { 2, "56473829107", new DateTime(2025, 8, 12, 9, 0, 0) },
                { 3, "84736291031", new DateTime(2025, 8, 17, 13, 0, 0) },
                { 4, "29384756112", new DateTime(2025, 8, 22, 15, 0, 0) },
                { 5, "47583920179", new DateTime(2025, 8, 27, 9, 0, 0) },
                { 6, "91827364504", new DateTime(2025, 9, 2, 13, 0, 0) },
                { 1, "47583920180", new DateTime(2025, 9, 27, 9, 0, 0) },
                { 2, "91827364505", new DateTime(2025, 10, 2, 13, 0, 0) },
                { 3, "38475619207", new DateTime(2025, 10, 7, 15, 0, 0) },
                { 4, "56473829109", new DateTime(2025, 10, 12, 9, 0, 0) },
                { 5, "84736291033", new DateTime(2025, 10, 17, 13, 0, 0) },
                { 10, "56473829110", new DateTime(2025, 5, 5, 9, 0, 0) },
                { 1, "84736291034", new DateTime(2025, 5, 10, 13, 0, 0) },
                { 2, "29384756115", new DateTime(2025, 5, 15, 15, 0, 0) },
                { 3, "47583920182", new DateTime(2025, 5, 20, 9, 0, 0) },
                { 4, "91827364507", new DateTime(2025, 5, 25, 13, 0, 0) }
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Consultas");
            migrationBuilder.Sql("DELETE FROM Medicos");
        }
    }
}
