using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabBuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteFieldsToVocab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "vocab",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "deleted_by",
                table: "vocab",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "vocab");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "vocab");
        }
    }
}
