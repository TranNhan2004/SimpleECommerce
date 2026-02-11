using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleECommerceBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoleAndModifySomeEntites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Credentials_Roles_RoleId",
                schema: "auth",
                table: "Credentials");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderShippingAddresses_OrderShippingAddressId",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_UserShippingAddresses_UserProfiles_CustomerId",
                schema: "dbo",
                table: "UserShippingAddresses");

            migrationBuilder.DropTable(
                name: "OrderShippingAddresses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "auth");

            migrationBuilder.DropIndex(
                name: "IX_Products_Status",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderShippingAddressId",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId_IsRead",
                schema: "dbo",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_ProductId",
                schema: "dbo",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Credentials_RoleId",
                schema: "auth",
                table: "Credentials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserShippingAddresses",
                schema: "dbo",
                table: "UserShippingAddresses");

            migrationBuilder.DropIndex(
                name: "IX_UserShippingAddresses_CustomerId_IsDefault",
                schema: "dbo",
                table: "UserShippingAddresses");

            migrationBuilder.DropColumn(
                name: "QuantityOnHand",
                schema: "dbo",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "auth",
                table: "Credentials");

            migrationBuilder.RenameTable(
                name: "UserShippingAddresses",
                schema: "dbo",
                newName: "CustomerShippingAddresses",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "Currency",
                schema: "dbo",
                table: "Orders",
                newName: "TotalCurrency");

            migrationBuilder.RenameColumn(
                name: "OrderShippingAddressId",
                schema: "dbo",
                table: "Orders",
                newName: "SellerId");

            migrationBuilder.RenameColumn(
                name: "Ward",
                schema: "dbo",
                table: "CustomerShippingAddresses",
                newName: "RecipientWard");

            migrationBuilder.RenameColumn(
                name: "Province",
                schema: "dbo",
                table: "CustomerShippingAddresses",
                newName: "RecipientProvince");

            migrationBuilder.RenameColumn(
                name: "AddressLine",
                schema: "dbo",
                table: "CustomerShippingAddresses",
                newName: "RecipientAddressLine");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                schema: "dbo",
                table: "CustomerShippingAddresses",
                newName: "RecipientPhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_UserShippingAddresses_CustomerId",
                schema: "dbo",
                table: "CustomerShippingAddresses",
                newName: "IX_CustomerShippingAddresses_CustomerId");

            migrationBuilder.AddColumn<int>(
                name: "TotalInStock",
                schema: "dbo",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                schema: "dbo",
                table: "ProductPrices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiredAt",
                schema: "dbo",
                table: "Orders",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientAddressLine",
                schema: "dbo",
                table: "Orders",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                schema: "dbo",
                table: "Orders",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhoneNumber",
                schema: "dbo",
                table: "Orders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientProvince",
                schema: "dbo",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientWard",
                schema: "dbo",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingAmount",
                schema: "dbo",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShopName",
                schema: "dbo",
                table: "Orders",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseAddressLine",
                schema: "dbo",
                table: "Orders",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseProvince",
                schema: "dbo",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseWard",
                schema: "dbo",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "QuantityReserved",
                schema: "dbo",
                table: "Inventories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantityInStock",
                schema: "dbo",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SellerWarehouseId",
                schema: "dbo",
                table: "Inventories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "auth",
                table: "Credentials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                schema: "dbo",
                table: "CustomerShippingAddresses",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientAddressLine",
                schema: "dbo",
                table: "CustomerShippingAddresses",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerShippingAddresses",
                schema: "dbo",
                table: "CustomerShippingAddresses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SellerShops",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    SellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerShops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerShops_UserProfiles_SellerId",
                        column: x => x.SellerId,
                        principalSchema: "dbo",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SellerWarehouses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    SellerShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AddressLine = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerWarehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerWarehouses_SellerShops_SellerShopId",
                        column: x => x.SellerShopId,
                        principalSchema: "dbo",
                        principalTable: "SellerShops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductId1",
                schema: "dbo",
                table: "ProductPrices",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SellerId",
                schema: "dbo",
                table: "Orders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ProductId_SellerWarehouseId",
                schema: "dbo",
                table: "Inventories",
                columns: new[] { "ProductId", "SellerWarehouseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_SellerWarehouseId",
                schema: "dbo",
                table: "Inventories",
                column: "SellerWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerShops_SellerId",
                schema: "dbo",
                table: "SellerShops",
                column: "SellerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SellerWarehouses_SellerShopId",
                schema: "dbo",
                table: "SellerWarehouses",
                column: "SellerShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerShippingAddresses_UserProfiles_CustomerId",
                schema: "dbo",
                table: "CustomerShippingAddresses",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_SellerWarehouses_SellerWarehouseId",
                schema: "dbo",
                table: "Inventories",
                column: "SellerWarehouseId",
                principalSchema: "dbo",
                principalTable: "SellerWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UserProfiles_SellerId",
                schema: "dbo",
                table: "Orders",
                column: "SellerId",
                principalSchema: "dbo",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_Products_ProductId1",
                schema: "dbo",
                table: "ProductPrices",
                column: "ProductId1",
                principalSchema: "dbo",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerShippingAddresses_UserProfiles_CustomerId",
                schema: "dbo",
                table: "CustomerShippingAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_SellerWarehouses_SellerWarehouseId",
                schema: "dbo",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UserProfiles_SellerId",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_Products_ProductId1",
                schema: "dbo",
                table: "ProductPrices");

            migrationBuilder.DropTable(
                name: "SellerWarehouses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SellerShops",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_ProductId1",
                schema: "dbo",
                table: "ProductPrices");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SellerId",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_ProductId_SellerWarehouseId",
                schema: "dbo",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_SellerWarehouseId",
                schema: "dbo",
                table: "Inventories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerShippingAddresses",
                schema: "dbo",
                table: "CustomerShippingAddresses");

            migrationBuilder.DropColumn(
                name: "TotalInStock",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                schema: "dbo",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RecipientAddressLine",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RecipientPhoneNumber",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RecipientProvince",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RecipientWard",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAmount",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShopName",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WarehouseAddressLine",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WarehouseProvince",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WarehouseWard",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "QuantityInStock",
                schema: "dbo",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "SellerWarehouseId",
                schema: "dbo",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "auth",
                table: "Credentials");

            migrationBuilder.RenameTable(
                name: "CustomerShippingAddresses",
                schema: "dbo",
                newName: "UserShippingAddresses",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "TotalCurrency",
                schema: "dbo",
                table: "Orders",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "SellerId",
                schema: "dbo",
                table: "Orders",
                newName: "OrderShippingAddressId");

            migrationBuilder.RenameColumn(
                name: "RecipientWard",
                schema: "dbo",
                table: "UserShippingAddresses",
                newName: "Ward");

            migrationBuilder.RenameColumn(
                name: "RecipientProvince",
                schema: "dbo",
                table: "UserShippingAddresses",
                newName: "Province");

            migrationBuilder.RenameColumn(
                name: "RecipientAddressLine",
                schema: "dbo",
                table: "UserShippingAddresses",
                newName: "AddressLine");

            migrationBuilder.RenameColumn(
                name: "RecipientPhoneNumber",
                schema: "dbo",
                table: "UserShippingAddresses",
                newName: "PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerShippingAddresses_CustomerId",
                schema: "dbo",
                table: "UserShippingAddresses",
                newName: "IX_UserShippingAddresses_CustomerId");

            migrationBuilder.AlterColumn<int>(
                name: "QuantityReserved",
                schema: "dbo",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "QuantityOnHand",
                schema: "dbo",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                schema: "auth",
                table: "Credentials",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                schema: "dbo",
                table: "UserShippingAddresses",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine",
                schema: "dbo",
                table: "UserShippingAddresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserShippingAddresses",
                schema: "dbo",
                table: "UserShippingAddresses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OrderShippingAddresses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    AddressLine = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderShippingAddresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "auth",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Status",
                schema: "dbo",
                table: "Products",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderShippingAddressId",
                schema: "dbo",
                table: "Orders",
                column: "OrderShippingAddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                schema: "dbo",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ProductId",
                schema: "dbo",
                table: "Inventories",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_RoleId",
                schema: "auth",
                table: "Credentials",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserShippingAddresses_CustomerId_IsDefault",
                schema: "dbo",
                table: "UserShippingAddresses",
                columns: new[] { "CustomerId", "IsDefault" },
                filter: "[IsDefault] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                schema: "auth",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "auth",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "auth",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "auth",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Credentials_Roles_RoleId",
                schema: "auth",
                table: "Credentials",
                column: "RoleId",
                principalSchema: "auth",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderShippingAddresses_OrderShippingAddressId",
                schema: "dbo",
                table: "Orders",
                column: "OrderShippingAddressId",
                principalSchema: "dbo",
                principalTable: "OrderShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserShippingAddresses_UserProfiles_CustomerId",
                schema: "dbo",
                table: "UserShippingAddresses",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
