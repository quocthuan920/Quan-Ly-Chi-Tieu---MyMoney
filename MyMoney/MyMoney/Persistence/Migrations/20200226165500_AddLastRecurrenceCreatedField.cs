﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MyMoney.Persistence.Migrations
{
    public partial class AddLastRecurrenceCreatedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS PaymentTag;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS Tags;");

            migrationBuilder.AddColumn<DateTime>(name: "LastRecurrenceCreated",
                                                 table: "RecurringPayments",
                                                 nullable: false,
                                                 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LastRecurrenceCreated",
                                        table: "RecurringPayments");

            migrationBuilder.CreateTable(name: "Tags",
                                         columns: table => new
                                         {
                                             Id = table.Column<int>(nullable: false)
                              .Annotation("Sqlite:Autoincrement", true),
                                             Name = table.Column<string>(nullable: true)
                                         },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_Tags", x => x.Id);
                                                      });

            migrationBuilder.CreateTable(name: "PaymentTag",
                                         columns: table => new
                                         {
                                             PaymentId = table.Column<int>(nullable: false),
                                             TagId = table.Column<int>(nullable: false)
                                         },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_PaymentTag", x => new { x.PaymentId, x.TagId });
                                                          table.ForeignKey(name: "FK_PaymentTag_Payments_PaymentId",
                                                                           column: x => x.PaymentId,
                                                                           principalTable: "Payments",
                                                                           principalColumn: "Id",
                                                                           onDelete: ReferentialAction.Cascade);
                                                          table.ForeignKey(name: "FK_PaymentTag_Tags_TagId",
                                                                           column: x => x.TagId,
                                                                           principalTable: "Tags",
                                                                           principalColumn: "Id",
                                                                           onDelete: ReferentialAction.Cascade);
                                                      });

            migrationBuilder.CreateIndex(name: "IX_PaymentTag_TagId",
                                         table: "PaymentTag",
                                         column: "TagId");
        }
    }
}
