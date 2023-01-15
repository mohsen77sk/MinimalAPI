using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF object_id(N'[app].[fn_generatePeopleCode]', N'FN') IS NULL
                BEGIN
                    DECLARE @sql NVARCHAR(MAX);
                    SET @sql = N'
                    CREATE FUNCTION [app].[fn_generatePeopleCode]()
                    RETURNS NVARCHAR(450)
                    AS
                    BEGIN
                        IF EXISTS(SELECT 1 FROM [app].[People])
                            BEGIN
                                RETURN (CAST((SELECT TOP 1 Code FROM [app].[People] ORDER BY Code DESC) AS INT)) + 1;
                            End
                        RETURN 1000;
                    END';
                    EXEC sp_executesql @sql;
                END
                GO
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE [app].[People]
                ADD CONSTRAINT DEFAULT_People_Code
                DEFAULT app.fn_generatePeopleCode() FOR Code;
                GO
            ");

            migrationBuilder.Sql(@"
                IF object_id(N'[app].[fn_generateAccountsCode]', N'FN') IS NULL
                BEGIN
                    DECLARE @sql NVARCHAR(MAX);
                    SET @sql = N'
                        CREATE FUNCTION [app].[fn_generateAccountsCode]()
                        RETURNS NVARCHAR(450)
                        AS
                        BEGIN            
                            IF EXISTS(SELECT 1 FROM [app].[Accounts])
                            BEGIN
                                RETURN (CAST((SELECT TOP 1 Code FROM [app].[Accounts] ORDER BY Code DESC) AS INT)) + 1;
                            End
                            RETURN 1000;
                        END';
                    EXEC sp_executesql @sql;
                END
                GO
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE [app].[Accounts]
                ADD CONSTRAINT DEFAULT_Accounts_Code
                DEFAULT app.fn_generateAccountsCode() FOR Code;
                GO
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [app].[People] DROP CONSTRAINT DEFAULT_People_Code");
            migrationBuilder.Sql("DROP FUNCTION [app].[fn_generatePeopleCode]");

            migrationBuilder.Sql("ALTER TABLE [app].[Accounts] DROP CONSTRAINT DEFAULT_Accounts_Code");
            migrationBuilder.Sql("DROP FUNCTION [app].[fn_generateAccountsCode]");
        }
    }
}