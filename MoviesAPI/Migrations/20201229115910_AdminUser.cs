using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class AdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'98f80ebf-15cb-403d-9f9c-5acf13400480', N'samplemail@gmail.com', N'SAMPLEMAIL@GMAIL.COM', N'samplemail@gmail.com', N'SAMPLEMAIL@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEJx4SrNEQ8ieY1AzcDf1Kq/hKbUZv5Sk1JPqm/fEIjUMuWk0pRjETZfk8b58FL5kZg==', N'JUUOD6NGVD5SFGJ2YFPKQYMXN2IPV73B', N'3ff3df58-314a-45ca-86c2-a59ecbf45412', NULL, 0, 0, NULL, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[AspNetUserClaims] ON 

GO
INSERT [dbo].[AspNetUserClaims] ([Id], [UserId], [ClaimType], [ClaimValue]) VALUES (1, N'98f80ebf-15cb-403d-9f9c-5acf13400480', N'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', N'Admin')
GO
SET IDENTITY_INSERT [dbo].[AspNetUserClaims] OFF
GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete [dbo].[AspNetUsers] where [Id] = '98f80ebf-15cb-403d-9f9c-5acf13400480'
            delete [dbo].[AspNetUserClaims] where [Id] = 1
            ");
        }
    }
}
