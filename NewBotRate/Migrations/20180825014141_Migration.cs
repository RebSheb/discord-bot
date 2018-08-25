using Microsoft.EntityFrameworkCore.Migrations;

namespace NewBotRate.Migrations
{
    public partial class Migration : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<ulong>(nullable: false),
                    GuildID = table.Column<ulong>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Read = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedbackID);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildID = table.Column<ulong>(nullable: false),
                    UserID = table.Column<ulong>(nullable: false),
                    TagName = table.Column<string>(nullable: true),
                    TagMsg = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
