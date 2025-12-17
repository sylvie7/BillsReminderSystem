using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillReminderSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyToBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Bills",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Bills");
        }
    }
}
