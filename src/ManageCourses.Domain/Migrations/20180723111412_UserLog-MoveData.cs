using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class UserLogMoveData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update mc_user u set
                    first_login_date_utc = ul.first_login_date_utc,
                    last_login_date_utc = ul.last_login_date_utc,
                    welcome_email_date_utc = ul.welcome_email_date_utc, 
                    sign_in_user_id = ul.sign_in_user_id
                from user_log ul
                where ul.user_id = u.id or ul.user_email = u.email;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
