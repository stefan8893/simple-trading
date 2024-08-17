﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleTrading.Domain.DataAccess;

#nullable disable

namespace SimpleTrading.DataAccess.SqlServer.Migrations
{
    [DbContext(typeof(TradingDbContext))]
    partial class TradingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Asset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.HasIndex("Symbol")
                        .IsUnique();

                    b.ToTable("Asset", (string)null);
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("IsoCode")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("IsoCode")
                        .IsUnique();

                    b.ToTable("Currency", (string)null);
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Profile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<bool>("IsSelected")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Profile", (string)null);
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Reference", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("Notes")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<Guid>("TradeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("TradeId");

                    b.ToTable("Reference", (string)null);
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Trade", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AssetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("Balance")
                        .HasPrecision(24, 8)
                        .HasColumnType("decimal(24,8)");

                    b.Property<DateTime?>("Closed")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Notes")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<DateTime>("Opened")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ProfileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Result")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Size")
                        .HasPrecision(24, 8)
                        .HasColumnType("decimal(24,8)");

                    b.ComplexProperty<Dictionary<string, object>>("PositionPrices", "SimpleTrading.Domain.Trading.Trade.PositionPrices#PositionPrices", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<decimal>("Entry")
                                .HasPrecision(24, 8)
                                .HasColumnType("decimal(24,8)");

                            b1.Property<decimal?>("Exit")
                                .HasPrecision(24, 8)
                                .HasColumnType("decimal(24,8)");

                            b1.Property<decimal?>("StopLoss")
                                .HasPrecision(24, 8)
                                .HasColumnType("decimal(24,8)");

                            b1.Property<decimal?>("TakeProfit")
                                .HasPrecision(24, 8)
                                .HasColumnType("decimal(24,8)");
                        });

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Trade", (string)null);
                });

            modelBuilder.Entity("SimpleTrading.Domain.User.UserSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Language")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("UserSettings", (string)null);
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Reference", b =>
                {
                    b.HasOne("SimpleTrading.Domain.Trading.Trade", "Trade")
                        .WithMany("References")
                        .HasForeignKey("TradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trade");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Trade", b =>
                {
                    b.HasOne("SimpleTrading.Domain.Trading.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SimpleTrading.Domain.Trading.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SimpleTrading.Domain.Trading.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Asset");

                    b.Navigation("Currency");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Trade", b =>
                {
                    b.Navigation("References");
                });
#pragma warning restore 612, 618
        }
    }
}
