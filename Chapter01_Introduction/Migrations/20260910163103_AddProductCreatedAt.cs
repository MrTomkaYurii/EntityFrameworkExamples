using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCoreExamples.Chapter01_Introduction.Migrations
{
    // Друга міграція. EF порівняв поточну модель зі знімком (ModelSnapshot)
    // і побачив нову властивість Product.CreatedAt — тому Up() лише додає стовпець.
    /// <inheritdoc />
    public partial class AddProductCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Products");
        }
    }
}
