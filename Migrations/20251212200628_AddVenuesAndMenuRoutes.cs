using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AppBoleteriaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddVenuesAndMenuRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VenueId",
                schema: "public",
                table: "Event",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MenuRoute",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    RequiredRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuRoute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuRoute_MenuRoute_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "MenuRoute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venue",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Venue_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "public",
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "public",
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_VenueId",
                schema: "public",
                table: "Event",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuRoute_ParentId",
                schema: "public",
                table: "MenuRoute",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Code",
                schema: "public",
                table: "Permission",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                schema: "public",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId_PermissionId",
                schema: "public",
                table: "RolePermission",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Venue_CompanyId",
                schema: "public",
                table: "Venue",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Venue_VenueId",
                schema: "public",
                table: "Event",
                column: "VenueId",
                principalSchema: "public",
                principalTable: "Venue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Venue_VenueId",
                schema: "public",
                table: "Event");

            migrationBuilder.DropTable(
                name: "MenuRoute",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RolePermission",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Venue",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Permission",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Event_VenueId",
                schema: "public",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "VenueId",
                schema: "public",
                table: "Event");
        }
    }
}
