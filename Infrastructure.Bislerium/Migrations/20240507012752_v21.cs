using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Bislerium.Migrations
{
    /// <inheritdoc />
    public partial class v21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Downvotes",
                table: "CommentReactions");

            migrationBuilder.DropColumn(
                name: "Upvotes",
                table: "CommentReactions");

            migrationBuilder.DropColumn(
                name: "Downvotes",
                table: "BlogReactions");

            migrationBuilder.DropColumn(
                name: "Upvotes",
                table: "BlogReactions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CommentReactions",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BlogReactions",
                newName: "UserName");

            migrationBuilder.AddColumn<int>(
                name: "TotalReaction",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserProfile",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reaction",
                table: "CommentReactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "CommentHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reaction",
                table: "BlogReactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalReaction",
                table: "BlogPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserProfile",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "BlogPostHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalReaction",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserProfile",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Reaction",
                table: "CommentReactions");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "CommentHistories");

            migrationBuilder.DropColumn(
                name: "Reaction",
                table: "BlogReactions");

            migrationBuilder.DropColumn(
                name: "TotalReaction",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UserProfile",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "BlogPostHistories");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "CommentReactions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "BlogReactions",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "Downvotes",
                table: "CommentReactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Upvotes",
                table: "CommentReactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Downvotes",
                table: "BlogReactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Upvotes",
                table: "BlogReactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
