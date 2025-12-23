using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBoleteriaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessLog_Ticket_TicketId",
                table: "AccessLog");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessLog_users_StaffId",
                table: "AccessLog");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_users_OrganizerId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_TicketType_TicketTypeId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_users_UserId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketType_Event_EventId",
                table: "TicketType");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Ticket_TicketId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_users_UserId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_users_Role_role_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketType",
                table: "TicketType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessLog",
                table: "AccessLog");

            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Role",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "Transactions",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "TicketType",
                newName: "TicketTypes",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Ticket",
                newName: "Tickets",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Events",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AccessLog",
                newName: "AccessLogs",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_UserId",
                schema: "public",
                table: "Transactions",
                newName: "IX_Transactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_TicketId",
                schema: "public",
                table: "Transactions",
                newName: "IX_Transactions_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketType_EventId",
                schema: "public",
                table: "TicketTypes",
                newName: "IX_TicketTypes_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_UserId",
                schema: "public",
                table: "Tickets",
                newName: "IX_Tickets_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_TicketTypeId",
                schema: "public",
                table: "Tickets",
                newName: "IX_Tickets_TicketTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_OrganizerId",
                schema: "public",
                table: "Events",
                newName: "IX_Events_OrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLog_TicketId",
                schema: "public",
                table: "AccessLogs",
                newName: "IX_AccessLogs_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLog_StaffId",
                schema: "public",
                table: "AccessLogs",
                newName: "IX_AccessLogs_StaffId");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                schema: "public",
                table: "users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                schema: "public",
                table: "users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "public",
                table: "users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "public",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "public",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_users_Role_role_id",
                schema: "public",
                table: "users",
                column: "role_id",
                principalSchema: "public",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_users_Role_role_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                schema: "public",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketTypes",
                schema: "public",
                table: "TicketTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
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

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "public",
                table: "users");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "public",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Role",
                schema: "public",
                newName: "Role");

            migrationBuilder.RenameTable(
                name: "Transactions",
                schema: "public",
                newName: "Transaction");

            migrationBuilder.RenameTable(
                name: "TicketTypes",
                schema: "public",
                newName: "TicketType");

            migrationBuilder.RenameTable(
                name: "Tickets",
                schema: "public",
                newName: "Ticket");

            migrationBuilder.RenameTable(
                name: "Events",
                schema: "public",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "AccessLogs",
                schema: "public",
                newName: "AccessLog");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_UserId",
                table: "Transaction",
                newName: "IX_Transaction_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_TicketId",
                table: "Transaction",
                newName: "IX_Transaction_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketTypes_EventId",
                table: "TicketType",
                newName: "IX_TicketType_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_UserId",
                table: "Ticket",
                newName: "IX_Ticket_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_TicketTypeId",
                table: "Ticket",
                newName: "IX_Ticket_TicketTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_OrganizerId",
                table: "Event",
                newName: "IX_Event_OrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLogs_TicketId",
                table: "AccessLog",
                newName: "IX_AccessLog_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLogs_StaffId",
                table: "AccessLog",
                newName: "IX_AccessLog_StaffId");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketType",
                table: "TicketType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessLog",
                table: "AccessLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLog_Ticket_TicketId",
                table: "AccessLog",
                column: "TicketId",
                principalTable: "Ticket",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLog_users_StaffId",
                table: "AccessLog",
                column: "StaffId",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_users_OrganizerId",
                table: "Event",
                column: "OrganizerId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_TicketType_TicketTypeId",
                table: "Ticket",
                column: "TicketTypeId",
                principalTable: "TicketType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_users_UserId",
                table: "Ticket",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketType_Event_EventId",
                table: "TicketType",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Ticket_TicketId",
                table: "Transaction",
                column: "TicketId",
                principalTable: "Ticket",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_users_UserId",
                table: "Transaction",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_Role_role_id",
                table: "users",
                column: "role_id",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
