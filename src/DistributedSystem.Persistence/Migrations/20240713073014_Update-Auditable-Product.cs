using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DistributedSystem.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditableProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOnUtc",
                table: "Product",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedOnUtc",
                table: "Product",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUtc",
                table: "Product");
        }
    }
}
