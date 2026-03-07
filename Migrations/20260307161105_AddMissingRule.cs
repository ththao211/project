using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWP_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5971));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5986));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5987));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5988));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5989));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5991));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5992));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 8,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5993));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 9,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5994));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5995));

            migrationBuilder.InsertData(
                table: "ReputationRules",
                columns: new[] { "RuleID", "Category", "Description", "IsActive", "RuleName", "UpdatedAt", "Value" },
                values: new object[] { 11, "Limit", "Số task Fail liên tiếp để bị khóa tài khoản", true, "Max_Consecutive_Fails", new DateTime(2026, 3, 7, 23, 11, 3, 784, DateTimeKind.Local).AddTicks(5997), 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 11);

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5525));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5538));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5540));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5541));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5542));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5543));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5544));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 8,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5545));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 9,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5546));

            migrationBuilder.UpdateData(
                table: "ReputationRules",
                keyColumn: "RuleID",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 6, 21, 59, 2, 939, DateTimeKind.Local).AddTicks(5547));
        }
    }
}
