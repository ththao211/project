using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWP_BE.Migrations
{
    /// <inheritdoc />
    public partial class Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Tạo bảng AnnotatorStats (Chỉ giữ lại cái này)
            migrationBuilder.CreateTable(
                name: "AnnotatorStats",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalCompletedTasks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FirstTryApprovedTasks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalWorkingHours = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    AvgCompletionHours = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    CurrentPerfectStreak = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnotatorStats", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_AnnotatorStats_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            // 2. Tạo bảng ReviewerStats (Chỉ giữ lại cái này)
            migrationBuilder.CreateTable(
                name: "ReviewerStats",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalReviewedTasks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalReviewHours = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    AvgReviewHours = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    DisputedTasks = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewerStats", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_ReviewerStats_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}