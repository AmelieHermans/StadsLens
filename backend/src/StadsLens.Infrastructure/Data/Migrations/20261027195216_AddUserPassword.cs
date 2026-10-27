using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StadsLens.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            // De oude testgebruikers (enkel een naam, vaak dubbel) hebben geen wachtwoord en kunnen niet meer inloggen.
            // Weg ermee, anders faalt de unieke index op Name. Hun bezoeken verdwijnen mee (cascade).
            migrationBuilder.Sql("DELETE FROM Users WHERE PasswordHash = ''");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");
        }
    }
}
