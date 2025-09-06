using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataPreprocessing.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "preprocessing_script_stores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    script_content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_preprocessing_script_stores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "process_datas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data = table.Column<string>(type: "text", nullable: true),
                    status_process = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    preprocessing_script_store_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_process_datas", x => x.id);
                    table.ForeignKey(
                        name: "fk_process_datas_preprocessing_script_stores_preprocessing_scr",
                        column: x => x.preprocessing_script_store_id,
                        principalTable: "preprocessing_script_stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_preprocessing_script_stores_name",
                table: "preprocessing_script_stores",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_process_datas_preprocessing_script_store_id",
                table: "process_datas",
                column: "preprocessing_script_store_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "process_datas");

            migrationBuilder.DropTable(
                name: "preprocessing_script_stores");
        }
    }
}
