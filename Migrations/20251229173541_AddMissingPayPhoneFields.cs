using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBoleteriaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingPayPhoneFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. SOLO para Users - CountryCode (ÚNICO campo que falta aquí)
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "593");

            // 2. SOLO para Company - Campos PayPhone que faltan
            migrationBuilder.AddColumn<string>(
                name: "PayPhoneStoreId",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayPhonePhoneNumber",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayPhoneCountryCode",
                table: "Company",
                type: "text",
                nullable: true,
                defaultValue: "593");

            migrationBuilder.AddColumn<string>(
                name: "PayPhoneCurrency",
                table: "Company",
                type: "text",
                nullable: true,
                defaultValue: "USD");

            migrationBuilder.AddColumn<int>(
                name: "PayPhoneTimeZone",
                table: "Company",
                type: "integer",
                nullable: true,
                defaultValue: -5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Eliminar de Users
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "users");

            // 2. Eliminar de Company
            migrationBuilder.DropColumn(
                name: "PayPhoneStoreId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "PayPhonePhoneNumber",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "PayPhoneCountryCode",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "PayPhoneCurrency",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "PayPhoneTimeZone",
                table: "Company");
        }
    }
}