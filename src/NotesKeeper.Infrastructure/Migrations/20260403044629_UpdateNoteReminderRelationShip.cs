using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesKeeper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNoteReminderRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reminders_NoteId",
                schema: "Notifications",
                table: "Reminders");

            migrationBuilder.AddColumn<int>(
                name: "ReminderId",
                schema: "Contents",
                table: "Notes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_NoteId",
                schema: "Notifications",
                table: "Reminders",
                column: "NoteId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reminders_NoteId",
                schema: "Notifications",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "ReminderId",
                schema: "Contents",
                table: "Notes");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_NoteId",
                schema: "Notifications",
                table: "Reminders",
                column: "NoteId");
        }
    }
}
