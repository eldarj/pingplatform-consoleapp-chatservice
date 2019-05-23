using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatMicroservice.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Firstname = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: false),
                    DateRegistered = table.Column<DateTime>(nullable: false),
                    AvatarImageUrl = table.Column<string>(nullable: true),
                    CoverImageUrl = table.Column<string>(nullable: true),
                    DataSpaceDirName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false),
                    ContactAccountId = table.Column<int>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    ContactName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => new { x.AccountId, x.ContactAccountId });
                    table.ForeignKey(
                        name: "FK_Contacts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contacts_Accounts_ContactAccountId",
                        column: x => x.ContactAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_PhoneNumber",
                table: "Accounts",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactAccountId",
                table: "Contacts",
                column: "ContactAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
