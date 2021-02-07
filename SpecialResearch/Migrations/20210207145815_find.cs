using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpecialResearch.Migrations
{
    public partial class find : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interface",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    NormalState = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interface", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StageName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestName = table.Column<string>(nullable: false),
                    TestDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    StageID = table.Column<int>(nullable: false),
                    CreatorId = table.Column<int>(nullable: false),
                    ControlerId = table.Column<int>(nullable: false),
                    UseOrder = table.Column<string>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    PhotoCopy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Request_User_ControlerId",
                        column: x => x.ControlerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Request_User_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Request_Stage_StageID",
                        column: x => x.StageID,
                        principalTable: "Stage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    RequestId = table.Column<int>(nullable: false),
                    Manufacturer = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    OperatingMode = table.Column<string>(nullable: true),
                    PhotoCopy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipment_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResult",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(nullable: false),
                    Result = table.Column<string>(nullable: true),
                    InterfaceId = table.Column<int>(nullable: false),
                    SignalFound = table.Column<bool>(nullable: false),
                    TestIsOk = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    frequency = table.Column<int>(nullable: false),
                    TestTypeId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResult_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResult_Interface_InterfaceId",
                        column: x => x.InterfaceId,
                        principalTable: "Interface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResult_TestType_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResult_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Может всё", "admin" },
                    { 2, "Приемщик СВТ", "receiver" },
                    { 3, "Испытатель.", "tester" },
                    { 4, "Контролер. Может выдавать предписания", "controller" },
                    { 5, "Управленец. Может все смотреть. Отчеты - его главная страница", "manager" }
                });

            migrationBuilder.InsertData(
                table: "Stage",
                columns: new[] { "Id", "StageName" },
                values: new object[,]
                {
                    { 1, "Заявка принята" },
                    { 2, "Испытания проведены" },
                    { 3, "Предписание выдано" },
                    { 4, "Заявка закрыта" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { 1, "Nobody", "Никто", "1443E1C37D06715738B11A27A4B0A5818466ECDA4EBD2E857A0DDEEA24D4BA84", 1 });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { 2, "admin", "Иванов И.И.", "8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_RequestId",
                table: "Equipment",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ControlerId",
                table: "Request",
                column: "ControlerId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_CreatorId",
                table: "Request",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_StageID",
                table: "Request",
                column: "StageID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_EquipmentId",
                table: "TestResult",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_InterfaceId",
                table: "TestResult",
                column: "InterfaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_TestTypeId",
                table: "TestResult",
                column: "TestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_UserId",
                table: "TestResult",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResult");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Interface");

            migrationBuilder.DropTable(
                name: "TestType");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Stage");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
