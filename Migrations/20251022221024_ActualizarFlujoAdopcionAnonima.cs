using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarFlujoAdopcionAnonima : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adopciones_Clientes_ClienteId",
                table: "Adopciones");

            migrationBuilder.DropIndex(
                name: "IX_Adopciones_ClienteId",
                table: "Adopciones");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Adopciones");

            migrationBuilder.AddColumn<string>(
                name: "EmailPostulante",
                table: "Adopciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Motivo",
                table: "Adopciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombrePostulante",
                table: "Adopciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelefonoPostulante",
                table: "Adopciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailPostulante",
                table: "Adopciones");

            migrationBuilder.DropColumn(
                name: "Motivo",
                table: "Adopciones");

            migrationBuilder.DropColumn(
                name: "NombrePostulante",
                table: "Adopciones");

            migrationBuilder.DropColumn(
                name: "TelefonoPostulante",
                table: "Adopciones");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Adopciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Adopciones_ClienteId",
                table: "Adopciones",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Adopciones_Clientes_ClienteId",
                table: "Adopciones",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
