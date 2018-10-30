using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCoreWindowsAuthExample.Migrations
{
    public partial class CreateDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SysConfigs",
                columns: table => new
                {
                    SysConfigId = table.Column<int>(nullable: false),
                    AppName = table.Column<string>(nullable: true),
                    AppFolder = table.Column<string>(nullable: true),
                    DeveloperName = table.Column<string>(nullable: true),
                    DeveloperEmail = table.Column<string>(nullable: true),
                    BusinessOwnerName = table.Column<string>(nullable: true),
                    BusinessOwnerEmail = table.Column<string>(nullable: true),
                    AppFromName = table.Column<string>(nullable: true),
                    AppFromEmail = table.Column<string>(nullable: true),
                    SmtpServer = table.Column<string>(nullable: true),
                    SmtpPort = table.Column<int>(nullable: true),
                    UserAdministratorName = table.Column<string>(nullable: true),
                    UserAdministratorEmail = table.Column<string>(nullable: true),
                    Rebuild = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysConfigs", x => x.SysConfigId);
                });

            migrationBuilder.CreateTable(
                name: "UserInformations",
                columns: table => new
                {
                    UserInformationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LanId = table.Column<string>(maxLength: 25, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInformations", x => x.UserInformationId);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 25, nullable: true),
                    Description = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleId);
                });

            migrationBuilder.CreateTable(
                name: "UserInformationUserRoles",
                columns: table => new
                {
                    UserInformationId = table.Column<int>(nullable: false),
                    UserRoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInformationUserRoles", x => new { x.UserInformationId, x.UserRoleId });
                    table.ForeignKey(
                        name: "FK_UserInformationUserRoles_UserInformations_UserInformationId",
                        column: x => x.UserInformationId,
                        principalTable: "UserInformations",
                        principalColumn: "UserInformationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInformationUserRoles_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "UserRoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SysConfigs",
                columns: new[] { "SysConfigId", "AppFolder", "AppFromEmail", "AppFromName", "AppName", "BusinessOwnerEmail", "BusinessOwnerName", "DeveloperEmail", "DeveloperName", "Rebuild", "SmtpPort", "SmtpServer", "UserAdministratorEmail", "UserAdministratorName" },
                values: new object[] { 1, "AspNetCoreWindowsAuthExample", "example@noReply.kartech.com", "Example Application", "AspNetCoreWindowsAuthExample", "jim@kartech.com", "Jim Karnopp", "jim@kartech.com", "Jim Karnopp", "False", 25, "smtp.MailServer.com", null, null });

            migrationBuilder.InsertData(
                table: "UserInformations",
                columns: new[] { "UserInformationId", "Email", "Enabled", "FirstName", "LanId", "LastName" },
                values: new object[] { 1, "jim@kartech.com", true, "Jim", "na\\karnopp", "Karnopp" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserRoleId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Users with admin access", "Administrator" },
                    { 2, "Users with base level Access", "User" }
                });

            migrationBuilder.InsertData(
                table: "UserInformationUserRoles",
                columns: new[] { "UserInformationId", "UserRoleId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_UserInformationUserRoles_UserRoleId",
                table: "UserInformationUserRoles",
                column: "UserRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysConfigs");

            migrationBuilder.DropTable(
                name: "UserInformationUserRoles");

            migrationBuilder.DropTable(
                name: "UserInformations");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
