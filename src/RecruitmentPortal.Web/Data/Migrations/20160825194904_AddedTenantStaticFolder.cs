using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentPortal.Web.Data.Migrations
{
    public partial class AddedTenantStaticFolder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Tenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
