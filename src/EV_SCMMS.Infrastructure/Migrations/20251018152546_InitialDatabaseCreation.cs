using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_SCMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabaseCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "center",
                columns: table => new
                {
                    centerid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'ACTIVE'::character varying"),
                    isactive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("center_pkey", x => x.centerid);
                });

            migrationBuilder.CreateTable(
                name: "spareparttype_tuht",
                columns: table => new
                {
                    typeid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'ACTIVE'::character varying"),
                    isactive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("spareparttype_tuht_pkey", x => x.typeid);
                });

            migrationBuilder.CreateTable(
                name: "inventory_tuht",
                columns: table => new
                {
                    inventoryid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    centerid = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    minimumstocklevel = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'ACTIVE'::character varying"),
                    isactive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("inventory_tuht_pkey", x => x.inventoryid);
                    table.ForeignKey(
                        name: "inventory_tuht_centerid_fkey",
                        column: x => x.centerid,
                        principalTable: "center",
                        principalColumn: "centerid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sparepart_tuht",
                columns: table => new
                {
                    sparepartid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    vehiclemodelid = table.Column<int>(type: "integer", nullable: true),
                    inventoryid = table.Column<Guid>(type: "uuid", nullable: true),
                    typeid = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    unitprice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    manufacture = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'ACTIVE'::character varying"),
                    isactive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("sparepart_tuht_pkey", x => x.sparepartid);
                    table.ForeignKey(
                        name: "sparepart_tuht_inventoryid_fkey",
                        column: x => x.inventoryid,
                        principalTable: "inventory_tuht",
                        principalColumn: "inventoryid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "sparepart_tuht_typeid_fkey",
                        column: x => x.typeid,
                        principalTable: "spareparttype_tuht",
                        principalColumn: "typeid",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "sparepartforecast_tuht",
                columns: table => new
                {
                    forecastid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    sparepartid = table.Column<Guid>(type: "uuid", nullable: false),
                    centerid = table.Column<Guid>(type: "uuid", nullable: false),
                    predictedusage = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    safetystock = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    reorderpoint = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    forecastedby = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, defaultValueSql: "'AI'::character varying"),
                    forecastconfidence = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    forecastdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    approvedby = table.Column<Guid>(type: "uuid", nullable: true),
                    approveddate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'PENDING'::character varying"),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    isactive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sparepartforecast_tuht_pkey", x => x.forecastid);
                    table.ForeignKey(
                        name: "sparepartforecast_tuht_centerid_fkey",
                        column: x => x.centerid,
                        principalTable: "center",
                        principalColumn: "centerid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "sparepartforecast_tuht_sparepartid_fkey",
                        column: x => x.sparepartid,
                        principalTable: "sparepart_tuht",
                        principalColumn: "sparepartid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sparepartusagehistory_tuht",
                columns: table => new
                {
                    usageid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    sparepartid = table.Column<Guid>(type: "uuid", nullable: false),
                    centerid = table.Column<Guid>(type: "uuid", nullable: false),
                    quantityused = table.Column<int>(type: "integer", nullable: false),
                    useddate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'ACTIVE'::character varying"),
                    isactive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("sparepartusagehistory_tuht_pkey", x => x.usageid);
                    table.ForeignKey(
                        name: "sparepartusagehistory_tuht_centerid_fkey",
                        column: x => x.centerid,
                        principalTable: "center",
                        principalColumn: "centerid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "sparepartusagehistory_tuht_sparepartid_fkey",
                        column: x => x.sparepartid,
                        principalTable: "sparepart_tuht",
                        principalColumn: "sparepartid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sparepartreplenishmentrequest",
                columns: table => new
                {
                    requestid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    centerid = table.Column<Guid>(type: "uuid", nullable: false),
                    sparepartid = table.Column<Guid>(type: "uuid", nullable: false),
                    forecastid = table.Column<Guid>(type: "uuid", nullable: true),
                    suggestedquantity = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'PENDING'::character varying"),
                    approvedby = table.Column<Guid>(type: "uuid", nullable: true),
                    approvedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    isactive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sparepartreplenishmentrequest_pkey", x => x.requestid);
                    table.ForeignKey(
                        name: "sparepartreplenishmentrequest_centerid_fkey",
                        column: x => x.centerid,
                        principalTable: "center",
                        principalColumn: "centerid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "sparepartreplenishmentrequest_forecastid_fkey",
                        column: x => x.forecastid,
                        principalTable: "sparepartforecast_tuht",
                        principalColumn: "forecastid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "sparepartreplenishmentrequest_sparepartid_fkey",
                        column: x => x.sparepartid,
                        principalTable: "sparepart_tuht",
                        principalColumn: "sparepartid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inventory_tuht_centerid",
                table: "inventory_tuht",
                column: "centerid");

            migrationBuilder.CreateIndex(
                name: "IX_sparepart_tuht_inventoryid",
                table: "sparepart_tuht",
                column: "inventoryid");

            migrationBuilder.CreateIndex(
                name: "IX_sparepart_tuht_typeid",
                table: "sparepart_tuht",
                column: "typeid");

            migrationBuilder.CreateIndex(
                name: "IX_sparepartforecast_tuht_centerid",
                table: "sparepartforecast_tuht",
                column: "centerid");

            migrationBuilder.CreateIndex(
                name: "IX_sparepartforecast_tuht_sparepartid",
                table: "sparepartforecast_tuht",
                column: "sparepartid");

            migrationBuilder.CreateIndex(
                name: "IX_sparepartreplenishmentrequest_centerid",
                table: "sparepartreplenishmentrequest",
                column: "centerid");

            migrationBuilder.CreateIndex(
                name: "IX_sparepartreplenishmentrequest_forecastid",
                table: "sparepartreplenishmentrequest",
                column: "forecastid");

            migrationBuilder.CreateIndex(
                name: "IX_sparepartreplenishmentrequest_sparepartid",
                table: "sparepartreplenishmentrequest",
                column: "sparepartid");

            migrationBuilder.CreateIndex(
                name: "IX_sparepartusagehistory_tuht_centerid",
                table: "sparepartusagehistory_tuht",
                column: "centerid");

            migrationBuilder.CreateIndex(
                name: "IX_sparepartusagehistory_tuht_sparepartid",
                table: "sparepartusagehistory_tuht",
                column: "sparepartid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sparepartreplenishmentrequest");

            migrationBuilder.DropTable(
                name: "sparepartusagehistory_tuht");

            migrationBuilder.DropTable(
                name: "sparepartforecast_tuht");

            migrationBuilder.DropTable(
                name: "sparepart_tuht");

            migrationBuilder.DropTable(
                name: "inventory_tuht");

            migrationBuilder.DropTable(
                name: "spareparttype_tuht");

            migrationBuilder.DropTable(
                name: "center");
        }
    }
}
