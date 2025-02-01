using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace chat.Api.Migrations
{
    /// <inheritdoc />
    public partial class vf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacion_User_UserId",
                table: "Notificacion");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Notificacion",
                newName: "MensajeId");

            migrationBuilder.RenameColumn(
                name: "Mensaje",
                table: "Notificacion",
                newName: "FechaEliminacion");

            migrationBuilder.RenameIndex(
                name: "IX_Notificacion_UserId",
                table: "Notificacion",
                newName: "IX_Notificacion_MensajeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacion_Mensaje_MensajeId",
                table: "Notificacion",
                column: "MensajeId",
                principalTable: "Mensaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacion_Mensaje_MensajeId",
                table: "Notificacion");

            migrationBuilder.RenameColumn(
                name: "MensajeId",
                table: "Notificacion",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "FechaEliminacion",
                table: "Notificacion",
                newName: "Mensaje");

            migrationBuilder.RenameIndex(
                name: "IX_Notificacion_MensajeId",
                table: "Notificacion",
                newName: "IX_Notificacion_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacion_User_UserId",
                table: "Notificacion",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
