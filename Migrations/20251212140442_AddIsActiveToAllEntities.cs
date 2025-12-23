using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBoleteriaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_Tickets_TicketId",
                schema: "public",
                table: "AccessLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_users_StaffId",
                schema: "public",
                table: "AccessLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_users_OrganizerId",
                schema: "public",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketTypes_TicketTypeId",
                schema: "public",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_users_UserId",
                schema: "public",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTypes_Events_EventId",
                schema: "public",
                table: "TicketTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tickets_TicketId",
                schema: "public",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_users_UserId",
                schema: "public",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                schema: "public",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TicketId",
                schema: "public",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                schema: "public",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketTypes",
                schema: "public",
                table: "TicketTypes");

            migrationBuilder.DropIndex(
                name: "IX_TicketTypes_EventId",
                schema: "public",
                table: "TicketTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
                schema: "public",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketTypeId",
                schema: "public",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                schema: "public",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessLogs",
                schema: "public",
                table: "AccessLogs");

            migrationBuilder.DropIndex(
                name: "IX_AccessLogs_StaffId",
                schema: "public",
                table: "AccessLogs");

            migrationBuilder.DropIndex(
                name: "IX_AccessLogs_TicketId",
                schema: "public",
                table: "AccessLogs");

            migrationBuilder.RenameTable(
                name: "Transactions",
                schema: "public",
                newName: "Transaction",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "TicketTypes",
                schema: "public",
                newName: "TicketType",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Tickets",
                schema: "public",
                newName: "Ticket",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Events",
                schema: "public",
                newName: "Event",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AccessLogs",
                schema: "public",
                newName: "AccessLog",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_UserId",
                schema: "public",
                table: "Ticket",
                newName: "IX_Ticket_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_OrganizerId",
                schema: "public",
                table: "Event",
                newName: "IX_Event_OrganizerId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "Role",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "public",
                table: "Transaction",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                schema: "public",
                table: "Transaction",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Transaction",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "public",
                table: "Transaction",
                type: "numeric(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "Transaction",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                schema: "public",
                table: "TicketType",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "TicketType",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "TicketType",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Used",
                schema: "public",
                table: "Ticket",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                schema: "public",
                table: "Ticket",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "Ticket",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "Event",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                schema: "public",
                table: "Event",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Event",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BannerUrl",
                schema: "public",
                table: "Event",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "Event",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScannedAt",
                schema: "public",
                table: "AccessLog",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "public",
                table: "AccessLog",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                schema: "public",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketType",
                schema: "public",
                table: "TicketType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ticket",
                schema: "public",
                table: "Ticket",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                schema: "public",
                table: "Event",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessLog",
                schema: "public",
                table: "AccessLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_users_OrganizerId",
                schema: "public",
                table: "Event",
                column: "OrganizerId",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_users_UserId",
                schema: "public",
                table: "Ticket",
                column: "UserId",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_users_OrganizerId",
                schema: "public",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_users_UserId",
                schema: "public",
                table: "Ticket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                schema: "public",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketType",
                schema: "public",
                table: "TicketType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ticket",
                schema: "public",
                table: "Ticket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                schema: "public",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessLog",
                schema: "public",
                table: "AccessLog");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "public",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "public",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "public",
                table: "TicketType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "public",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "public",
                table: "AccessLog");

            migrationBuilder.RenameTable(
                name: "Transaction",
                schema: "public",
                newName: "Transactions",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "TicketType",
                schema: "public",
                newName: "TicketTypes",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Ticket",
                schema: "public",
                newName: "Tickets",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Event",
                schema: "public",
                newName: "Events",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AccessLog",
                schema: "public",
                newName: "AccessLogs",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_UserId",
                schema: "public",
                table: "Tickets",
                newName: "IX_Tickets_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_OrganizerId",
                schema: "public",
                table: "Events",
                newName: "IX_Events_OrganizerId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "public",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                schema: "public",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "public",
                table: "Transactions",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                schema: "public",
                table: "TicketTypes",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "TicketTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Used",
                schema: "public",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                schema: "public",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "Events",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "BannerUrl",
                schema: "public",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScannedAt",
                schema: "public",
                table: "AccessLogs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                schema: "public",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketTypes",
                schema: "public",
                table: "TicketTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tickets",
                schema: "public",
                table: "Tickets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                schema: "public",
                table: "Events",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessLogs",
                schema: "public",
                table: "AccessLogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TicketId",
                schema: "public",
                table: "Transactions",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                schema: "public",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_EventId",
                schema: "public",
                table: "TicketTypes",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketTypeId",
                schema: "public",
                table: "Tickets",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_StaffId",
                schema: "public",
                table: "AccessLogs",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_TicketId",
                schema: "public",
                table: "AccessLogs",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_Tickets_TicketId",
                schema: "public",
                table: "AccessLogs",
                column: "TicketId",
                principalSchema: "public",
                principalTable: "Tickets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_users_StaffId",
                schema: "public",
                table: "AccessLogs",
                column: "StaffId",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_users_OrganizerId",
                schema: "public",
                table: "Events",
                column: "OrganizerId",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketTypes_TicketTypeId",
                schema: "public",
                table: "Tickets",
                column: "TicketTypeId",
                principalSchema: "public",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_users_UserId",
                schema: "public",
                table: "Tickets",
                column: "UserId",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTypes_Events_EventId",
                schema: "public",
                table: "TicketTypes",
                column: "EventId",
                principalSchema: "public",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tickets_TicketId",
                schema: "public",
                table: "Transactions",
                column: "TicketId",
                principalSchema: "public",
                principalTable: "Tickets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_users_UserId",
                schema: "public",
                table: "Transactions",
                column: "UserId",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
