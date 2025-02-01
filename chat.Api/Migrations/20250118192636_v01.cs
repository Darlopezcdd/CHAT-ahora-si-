using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace chat.Api.Migrations
{
    /// <inheritdoc />
    public partial class v01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grupo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProfilePicture = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrupoUser",
                columns: table => new
                {
                    GruposId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoUser", x => new { x.GruposId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_GrupoUser_Grupo_GruposId",
                        column: x => x.GruposId,
                        principalTable: "Grupo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrupoUser_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mensaje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserRemitenteId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserDestinatarioId = table.Column<int>(type: "INTEGER", nullable: true),
                    GrupoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Contenido = table.Column<string>(type: "TEXT", nullable: false),
                    UrlArchivo = table.Column<string>(type: "TEXT", nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Leido = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensaje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensaje_Grupo_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mensaje_User_UserDestinatarioId",
                        column: x => x.UserDestinatarioId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mensaje_User_UserRemitenteId",
                        column: x => x.UserRemitenteId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mensaje = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificacion_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrupoUser_UsersId",
                table: "GrupoUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensaje_GrupoId",
                table: "Mensaje",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensaje_UserDestinatarioId",
                table: "Mensaje",
                column: "UserDestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensaje_UserRemitenteId",
                table: "Mensaje",
                column: "UserRemitenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacion_UserId",
                table: "Notificacion",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrupoUser");

            migrationBuilder.DropTable(
                name: "Mensaje");

            migrationBuilder.DropTable(
                name: "Notificacion");

            migrationBuilder.DropTable(
                name: "Grupo");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
