using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatMicroservice.Data.Migrations
{
    public partial class EmojisEmojiCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmojiCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmojiCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emojis",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Decimal = table.Column<string>(nullable: true),
                    Unicode = table.Column<string>(nullable: true),
                    Hex = table.Column<string>(nullable: true),
                    EmojiCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emojis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emojis_EmojiCategories_EmojiCategoryId",
                        column: x => x.EmojiCategoryId,
                        principalTable: "EmojiCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emojis_EmojiCategoryId",
                table: "Emojis",
                column: "EmojiCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emojis");

            migrationBuilder.DropTable(
                name: "EmojiCategories");
        }
    }
}
