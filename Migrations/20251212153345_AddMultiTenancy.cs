using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AppBoleteriaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "company_id",
                schema: "public",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "public",
                table: "Transaction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "public",
                table: "TicketType",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "public",
                table: "Ticket",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "public",
                table: "Event",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Company",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Logo = table.Column<string>(type: "text", nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_company_id_email",
                schema: "public",
                table: "users",
                columns: new[] { "company_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CompanyId",
                schema: "public",
                table: "Transaction",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketType_CompanyId",
                schema: "public",
                table: "TicketType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketType_EventId",
                schema: "public",
                table: "TicketType",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CompanyId",
                schema: "public",
                table: "Ticket",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketTypeId",
                schema: "public",
                table: "Ticket",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CompanyId",
                schema: "public",
                table: "Event",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Slug",
                schema: "public",
                table: "Company",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Company_CompanyId",
                schema: "public",
                table: "Event",
                column: "CompanyId",
                principalSchema: "public",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Company_CompanyId",
                schema: "public",
                table: "Ticket",
                column: "CompanyId",
                principalSchema: "public",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_TicketType_TicketTypeId",
                schema: "public",
                table: "Ticket",
                column: "TicketTypeId",
                principalSchema: "public",
                principalTable: "TicketType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketType_Company_CompanyId",
                schema: "public",
                table: "TicketType",
                column: "CompanyId",
                principalSchema: "public",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketType_Event_EventId",
                schema: "public",
                table: "TicketType",
                column: "EventId",
                principalSchema: "public",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Company_CompanyId",
                schema: "public",
                table: "Transaction",
                column: "CompanyId",
                principalSchema: "public",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_Company_company_id",
                schema: "public",
                table: "users",
                column: "company_id",
                principalSchema: "public",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Company_CompanyId",
                schema: "public",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Company_CompanyId",
                schema: "public",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_TicketType_TicketTypeId",
                schema: "public",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketType_Company_CompanyId",
                schema: "public",
                table: "TicketType");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketType_Event_EventId",
                schema: "public",
                table: "TicketType");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Company_CompanyId",
                schema: "public",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_users_Company_company_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropTable(
                name: "Company",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_users_company_id_email",
                schema: "public",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_CompanyId",
                schema: "public",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_TicketType_CompanyId",
                schema: "public",
                table: "TicketType");

            migrationBuilder.DropIndex(
                name: "IX_TicketType_EventId",
                schema: "public",
                table: "TicketType");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_CompanyId",
                schema: "public",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_TicketTypeId",
                schema: "public",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Event_CompanyId",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "public",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "public",
                table: "TicketType");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "public",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "public",
                table: "Event");
        }
    }
}
