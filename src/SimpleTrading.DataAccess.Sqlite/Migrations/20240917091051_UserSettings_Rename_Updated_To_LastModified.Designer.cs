﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleTrading.DataAccess;

#nullable disable

namespace SimpleTrading.DataAccess.Sqlite.Migrations
{
    [DbContext(typeof(TradingDbContext))]
    [Migration("20240917091051_UserSettings_Rename_Updated_To_LastModified")]
    partial class UserSettings_Rename_Updated_To_LastModified
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Asset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Symbol")
                        .IsUnique();

                    b.ToTable("Asset");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("IsoCode")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("IsoCode")
                        .IsUnique();

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Profile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSelected")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Profile");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Reference", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TradeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TradeId");

                    b.ToTable("Reference");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Trade", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AssetId")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Balance")
                        .HasPrecision(24, 8)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Closed")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Opened")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProfileId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Size")
                        .HasPrecision(24, 8)
                        .HasColumnType("TEXT");

                    b.ComplexProperty<Dictionary<string, object>>("PositionPrices", "SimpleTrading.Domain.Trading.Trade.PositionPrices#PositionPrices", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<decimal>("Entry")
                                .HasPrecision(24, 8)
                                .HasColumnType("TEXT");

                            b1.Property<decimal?>("Exit")
                                .HasPrecision(24, 8)
                                .HasColumnType("TEXT");

                            b1.Property<decimal?>("StopLoss")
                                .HasPrecision(24, 8)
                                .HasColumnType("TEXT");

                            b1.Property<decimal?>("TakeProfit")
                                .HasPrecision(24, 8)
                                .HasColumnType("TEXT");
                        });

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Trade");
                });

            modelBuilder.Entity("SimpleTrading.Domain.User.UserSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UserSettings");
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

                    b.OwnsOne("SimpleTrading.Domain.Trading.Result", "Result", b1 =>
                        {
                            b1.Property<Guid>("TradeId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Index")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<short?>("Performance")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Source")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("TradeId");

                            b1.ToTable("Trade");

                            b1.ToJson("Result");

                            b1.WithOwner()
                                .HasForeignKey("TradeId");
                        });

                    b.Navigation("Asset");

                    b.Navigation("Currency");

                    b.Navigation("Profile");

                    b.Navigation("Result");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Trade", b =>
                {
                    b.Navigation("References");
                });
#pragma warning restore 612, 618
        }
    }
}
