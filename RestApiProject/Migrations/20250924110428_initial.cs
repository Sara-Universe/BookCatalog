using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestApiProject.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedYear = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsBorrowed = table.Column<bool>(type: "bit", nullable: false),
                    BorrowedByUserId = table.Column<int>(type: "int", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Users_BorrowedByUserId",
                        column: x => x.BorrowedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BorrowRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsOverdue = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowRecords_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteBooks",
                columns: table => new
                {
                    FavoriteBooksId = table.Column<int>(type: "int", nullable: false),
                    FavoritedByUsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteBooks", x => new { x.FavoriteBooksId, x.FavoritedByUsersId });
                    table.ForeignKey(
                        name: "FK_UserFavoriteBooks_Books_FavoriteBooksId",
                        column: x => x.FavoriteBooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteBooks_Users_FavoritedByUsersId",
                        column: x => x.FavoritedByUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWishlistBooks",
                columns: table => new
                {
                    WishlistBooksId = table.Column<int>(type: "int", nullable: false),
                    WishlistedByUsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWishlistBooks", x => new { x.WishlistBooksId, x.WishlistedByUsersId });
                    table.ForeignKey(
                        name: "FK_UserWishlistBooks_Books_WishlistBooksId",
                        column: x => x.WishlistBooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWishlistBooks_Users_WishlistedByUsersId",
                        column: x => x.WishlistedByUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BorrowedByUserId",
                table: "Books",
                column: "BorrowedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRecords_BookId",
                table: "BorrowRecords",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRecords_UserId",
                table: "BorrowRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteBooks_FavoritedByUsersId",
                table: "UserFavoriteBooks",
                column: "FavoritedByUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWishlistBooks_WishlistedByUsersId",
                table: "UserWishlistBooks",
                column: "WishlistedByUsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowRecords");

            migrationBuilder.DropTable(
                name: "UserFavoriteBooks");

            migrationBuilder.DropTable(
                name: "UserWishlistBooks");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
