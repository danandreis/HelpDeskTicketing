using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskTicketing.Migrations
{
    /// <inheritdoc />
    public partial class updateTicketDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketFiles_Tickets_TicketId",
                table: "TicketFiles");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "TicketFiles",
                newName: "TicketMessageId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketFiles_TicketId",
                table: "TicketFiles",
                newName: "IX_TicketFiles_TicketMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketFiles_TicketMessages_TicketMessageId",
                table: "TicketFiles",
                column: "TicketMessageId",
                principalTable: "TicketMessages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketFiles_TicketMessages_TicketMessageId",
                table: "TicketFiles");

            migrationBuilder.RenameColumn(
                name: "TicketMessageId",
                table: "TicketFiles",
                newName: "TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketFiles_TicketMessageId",
                table: "TicketFiles",
                newName: "IX_TicketFiles_TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketFiles_Tickets_TicketId",
                table: "TicketFiles",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }
    }
}
