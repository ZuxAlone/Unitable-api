using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unitable.DataAccess.Migrations
{
    public partial class UpdateClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recompensas_Usuarios_UsuarioId",
                table: "Recompensas");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_UsuarioId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "GrupoUsuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_UsuarioId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Recompensas_UsuarioId",
                table: "Recompensas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Recompensas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Actividades",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "DuracionMin",
                table: "Actividades",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Detalle",
                table: "Actividades",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Recompensas",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Actividades",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "DuracionMin",
                table: "Actividades",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Detalle",
                table: "Actividades",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "GrupoUsuario",
                columns: table => new
                {
                    GruposId = table.Column<int>(type: "int", nullable: false),
                    UsuariosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoUsuario", x => new { x.GruposId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_GrupoUsuario_Grupos_GruposId",
                        column: x => x.GruposId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrupoUsuario_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_UsuarioId",
                table: "Usuarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Recompensas_UsuarioId",
                table: "Recompensas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoUsuario_UsuariosId",
                table: "GrupoUsuario",
                column: "UsuariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recompensas_Usuarios_UsuarioId",
                table: "Recompensas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_UsuarioId",
                table: "Usuarios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
