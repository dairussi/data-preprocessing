using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataPreprocessing.Migrations
{
    /// <inheritdoc />
    public partial class MessageErrorProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error_message",
                table: "process_datas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error_message",
                table: "process_datas");
        }
    }
}
