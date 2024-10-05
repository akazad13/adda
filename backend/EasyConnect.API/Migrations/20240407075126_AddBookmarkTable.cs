using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyConnect.API.Migrations;

/// <inheritdoc />
public partial class AddBookmarkTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Likes");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "Photos",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "Url",
            table: "Photos",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "PublicId",
            table: "Photos",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "IsMain",
            table: "Photos",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.AlterColumn<bool>(
            name: "IsApproved",
            table: "Photos",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Photos",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DateAdded",
            table: "Photos",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime(6)");

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "Photos",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<int>(
            name: "SenderId",
            table: "Messages",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<bool>(
            name: "SenderDeleted",
            table: "Messages",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.AlterColumn<int>(
            name: "RecipientId",
            table: "Messages",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<bool>(
            name: "RecipientDeleted",
            table: "Messages",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.AlterColumn<DateTime>(
            name: "MessageSent",
            table: "Messages",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime(6)");

        migrationBuilder.AlterColumn<bool>(
            name: "IsRead",
            table: "Messages",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.AlterColumn<DateTime>(
            name: "DateRead",
            table: "Messages",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "datetime(6)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "Messages",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "Messages",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<string>(
            name: "Value",
            table: "AspNetUserTokens",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AspNetUserTokens",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(255) CHARACTER SET utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "LoginProvider",
            table: "AspNetUserTokens",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(255) CHARACTER SET utf8mb4");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "AspNetUserTokens",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "city",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserName",
            table: "AspNetUsers",
            type: "TEXT",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(256) CHARACTER SET utf8mb4",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "TwoFactorEnabled",
            table: "AspNetUsers",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.AlterColumn<string>(
            name: "SecurityStamp",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "PhoneNumberConfirmed",
            table: "AspNetUsers",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.AlterColumn<string>(
            name: "PhoneNumber",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "PasswordHash",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedUserName",
            table: "AspNetUsers",
            type: "TEXT",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(256) CHARACTER SET utf8mb4",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedEmail",
            table: "AspNetUsers",
            type: "TEXT",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(256) CHARACTER SET utf8mb4",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "LookingFor",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "LockoutEnd",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "datetime(6)",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "LockoutEnabled",
            table: "AspNetUsers",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastActive",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime(6)");

        migrationBuilder.AlterColumn<string>(
            name: "KnownAs",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Introduction",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Interests",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Gender",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "EmailConfirmed",
            table: "AspNetUsers",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "tinyint(1)");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "AspNetUsers",
            type: "TEXT",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(256) CHARACTER SET utf8mb4",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DateOfBirth",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime(6)");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Created",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime(6)");

        migrationBuilder.AlterColumn<string>(
            name: "Country",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ConcurrencyStamp",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "AccessFailedCount",
            table: "AspNetUsers",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "AspNetUsers",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<int>(
            name: "RoleId",
            table: "AspNetUserRoles",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "AspNetUserRoles",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "AspNetUserLogins",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "ProviderDisplayName",
            table: "AspNetUserLogins",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ProviderKey",
            table: "AspNetUserLogins",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(255) CHARACTER SET utf8mb4");

        migrationBuilder.AlterColumn<string>(
            name: "LoginProvider",
            table: "AspNetUserLogins",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(255) CHARACTER SET utf8mb4");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "AspNetUserClaims",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "ClaimValue",
            table: "AspNetUserClaims",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ClaimType",
            table: "AspNetUserClaims",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "AspNetUserClaims",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedName",
            table: "AspNetRoles",
            type: "TEXT",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(256) CHARACTER SET utf8mb4",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AspNetRoles",
            type: "TEXT",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(256) CHARACTER SET utf8mb4",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ConcurrencyStamp",
            table: "AspNetRoles",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "AspNetRoles",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<int>(
            name: "RoleId",
            table: "AspNetRoleClaims",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "ClaimValue",
            table: "AspNetRoleClaims",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ClaimType",
            table: "AspNetRoleClaims",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext CHARACTER SET utf8mb4",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "AspNetRoleClaims",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.CreateTable(
            name: "Bookmarks",
            columns: table => new
            {
                BookmarkerId = table.Column<int>(type: "INTEGER", nullable: false),
                BookmarkedId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Bookmarks", x => new { x.BookmarkerId, x.BookmarkedId });
                table.ForeignKey(
                    name: "FK_Bookmarks_AspNetUsers_BookmarkedId",
                    column: x => x.BookmarkedId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Bookmarks_AspNetUsers_BookmarkerId",
                    column: x => x.BookmarkerId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Bookmarks_BookmarkedId",
            table: "Bookmarks",
            column: "BookmarkedId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Bookmarks");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "Photos",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "Url",
            table: "Photos",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "PublicId",
            table: "Photos",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "IsMain",
            table: "Photos",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<bool>(
            name: "IsApproved",
            table: "Photos",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Photos",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DateAdded",
            table: "Photos",
            type: "datetime(6)",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "Photos",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<int>(
            name: "SenderId",
            table: "Messages",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<bool>(
            name: "SenderDeleted",
            table: "Messages",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<int>(
            name: "RecipientId",
            table: "Messages",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<bool>(
            name: "RecipientDeleted",
            table: "Messages",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<DateTime>(
            name: "MessageSent",
            table: "Messages",
            type: "datetime(6)",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<bool>(
            name: "IsRead",
            table: "Messages",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<DateTime>(
            name: "DateRead",
            table: "Messages",
            type: "datetime(6)",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "Messages",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "Messages",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<string>(
            name: "Value",
            table: "AspNetUserTokens",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AspNetUserTokens",
            type: "varchar(255) CHARACTER SET utf8mb4",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "LoginProvider",
            table: "AspNetUserTokens",
            type: "varchar(255) CHARACTER SET utf8mb4",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "AspNetUserTokens",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "city",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserName",
            table: "AspNetUsers",
            type: "varchar(256) CHARACTER SET utf8mb4",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "TwoFactorEnabled",
            table: "AspNetUsers",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "SecurityStamp",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "PhoneNumberConfirmed",
            table: "AspNetUsers",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "PhoneNumber",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "PasswordHash",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedUserName",
            table: "AspNetUsers",
            type: "varchar(256) CHARACTER SET utf8mb4",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedEmail",
            table: "AspNetUsers",
            type: "varchar(256) CHARACTER SET utf8mb4",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "LookingFor",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "LockoutEnd",
            table: "AspNetUsers",
            type: "datetime(6)",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "LockoutEnabled",
            table: "AspNetUsers",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastActive",
            table: "AspNetUsers",
            type: "datetime(6)",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "KnownAs",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Introduction",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Interests",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Gender",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "EmailConfirmed",
            table: "AspNetUsers",
            type: "tinyint(1)",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "AspNetUsers",
            type: "varchar(256) CHARACTER SET utf8mb4",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DateOfBirth",
            table: "AspNetUsers",
            type: "datetime(6)",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Created",
            table: "AspNetUsers",
            type: "datetime(6)",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "Country",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ConcurrencyStamp",
            table: "AspNetUsers",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "AccessFailedCount",
            table: "AspNetUsers",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "AspNetUsers",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<int>(
            name: "RoleId",
            table: "AspNetUserRoles",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "AspNetUserRoles",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "AspNetUserLogins",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "ProviderDisplayName",
            table: "AspNetUserLogins",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ProviderKey",
            table: "AspNetUserLogins",
            type: "varchar(255) CHARACTER SET utf8mb4",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<string>(
            name: "LoginProvider",
            table: "AspNetUserLogins",
            type: "varchar(255) CHARACTER SET utf8mb4",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "AspNetUserClaims",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "ClaimValue",
            table: "AspNetUserClaims",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ClaimType",
            table: "AspNetUserClaims",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "AspNetUserClaims",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedName",
            table: "AspNetRoles",
            type: "varchar(256) CHARACTER SET utf8mb4",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AspNetRoles",
            type: "varchar(256) CHARACTER SET utf8mb4",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ConcurrencyStamp",
            table: "AspNetRoles",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "AspNetRoles",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<int>(
            name: "RoleId",
            table: "AspNetRoleClaims",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "ClaimValue",
            table: "AspNetRoleClaims",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ClaimType",
            table: "AspNetRoleClaims",
            type: "longtext CHARACTER SET utf8mb4",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "AspNetRoleClaims",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER")
            .Annotation("Sqlite:Autoincrement", true)
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.CreateTable(
            name: "Likes",
            columns: table => new
            {
                LikerId = table.Column<int>(type: "int", nullable: false),
                LikeeId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Likes", x => new { x.LikerId, x.LikeeId });
                table.ForeignKey(
                    name: "FK_Likes_AspNetUsers_LikeeId",
                    column: x => x.LikeeId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Likes_AspNetUsers_LikerId",
                    column: x => x.LikerId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Likes_LikeeId",
            table: "Likes",
            column: "LikeeId");
    }
}
