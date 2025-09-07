using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataPreprocessing.Migrations
{
    /// <inheritdoc />
    public partial class ProcessId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "process_id",
                table: "process_datas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "process_id",
                table: "process_datas");
        }
    }
}
