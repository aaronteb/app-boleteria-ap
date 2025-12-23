using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBoleteriaApi.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserCompanyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_company_id_email",
                schema: "public",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "company_id",
                schema: "public",
                table: "users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_users_company_id",
                schema: "public",
                table: "users",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "public",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_company_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_email",
                schema: "public",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "company_id",
                schema: "public",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_company_id_email",
                schema: "public",
                table: "users",
                columns: new[] { "company_id", "email" },
                unique: true);
        }
    }
}
