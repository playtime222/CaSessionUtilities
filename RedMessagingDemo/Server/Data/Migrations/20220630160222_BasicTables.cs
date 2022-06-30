using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedMessagingDemo.Server.Data.Migrations
{
    public partial class BasicTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentEnrollmentId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentEnrollmentId",
                table: "AspNetUsers");
        }
    }
}
