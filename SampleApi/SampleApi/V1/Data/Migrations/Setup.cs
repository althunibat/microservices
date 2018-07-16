using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SampleApi.V1.Data.Migrations {
    public partial class Setup : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                "People",
                table => new {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_People", x => x.Id); });

            migrationBuilder.CreateTable(
                "Addresses",
                table => new {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Country = table.Column<string>(maxLength: 50, nullable: false),
                    City = table.Column<string>(maxLength: 50, nullable: false),
                    Street = table.Column<string>(maxLength: 50, nullable: false),
                    Building = table.Column<string>(maxLength: 50, nullable: false),
                    PersonId = table.Column<long>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        "FK_Addresses_People_PersonId",
                        x => x.PersonId,
                        "People",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                "People",
                new[] {"Id", "Name"},
                new object[] {1L, "Hamza Althunibat"});

            migrationBuilder.InsertData(
                "Addresses",
                new[] {"Id", "Building", "City", "Country", "PersonId", "Street", "Type"},
                new object[] {1L, "Golden Sands Tower", "Sharja", "United Arab Emirates", 1L, "Wehda Street", 0});

            migrationBuilder.CreateIndex(
                "IX_Addresses_Country",
                "Addresses",
                "Country");

            migrationBuilder.CreateIndex(
                "IX_Addresses_PersonId",
                "Addresses",
                "PersonId");

            migrationBuilder.CreateIndex(
                "IX_People_Name",
                "People",
                "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                "Addresses");

            migrationBuilder.DropTable(
                "People");
        }
    }
}