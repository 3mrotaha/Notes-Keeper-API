using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesKeeper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSomeConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reminders_NoteId",
                schema: "Notifications",
                table: "Reminders");

            migrationBuilder.AlterColumn<int>(
                name: "NoteId",
                schema: "Notifications",
                table: "Reminders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ReminderId",
                schema: "Contents",
                table: "Notes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_NoteId",
                schema: "Notifications",
                table: "Reminders",
                column: "NoteId",
                unique: true,
                filter: "[NoteId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reminders_NoteId",
                schema: "Notifications",
                table: "Reminders");

            migrationBuilder.AlterColumn<int>(
                name: "NoteId",
                schema: "Notifications",
                table: "Reminders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReminderId",
                schema: "Contents",
                table: "Notes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_NoteId",
                schema: "Notifications",
                table: "Reminders",
                column: "NoteId",
                unique: true);
        }
    }
}
