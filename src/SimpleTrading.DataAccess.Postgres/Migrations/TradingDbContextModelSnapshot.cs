﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SimpleTrading.DataAccess;

#nullable disable

namespace SimpleTrading.DataAccess.Postgres.Migrations
{
    [DbContext(typeof(TradingDbContext))]
    partial class TradingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Asset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.HasKey("Id");

                    b.HasIndex("Symbol")
                        .IsUnique();

                    b.ToTable("Asset");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IsoCode")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("IsoCode")
                        .IsUnique();

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Profile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<bool>("IsSelected")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Profile");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Reference", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<string>("Notes")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<Guid>("TradeId")
                        .HasColumnType("uuid");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("TradeId");

                    b.ToTable("Reference");
                });

            modelBuilder.Entity("SimpleTrading.Domain.Trading.Trade", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AssetId")
                        .HasColumnType("uuid");

                    b.Property<decimal?>("Balance")
                        .HasPrecision(24, 8)
                        .HasColumnType("numeric(24,8)");

                    b.Property<DateTime?>("Closed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uuid");

                    b.Property<string>("Notes")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<DateTime>("Opened")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ProfileId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Size")
                        .HasPrecision(24, 8)
                        .HasColumnType("numeric(24,8)");

                    b.ComplexProperty<Dictionary<string, object>>("PositionPrices", "SimpleTrading.Domain.Trading.Trade.PositionPrices#PositionPrices", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<decimal>("Entry")
                                .HasPrecision(24, 8)
                                .HasColumnType("numeric(24,8)");

                            b1.Property<decimal?>("Exit")
                                .HasPrecision(24, 8)
                                .HasColumnType("numeric(24,8)");

                            b1.Property<decimal?>("StopLoss")
                                .HasPrecision(24, 8)
                                .HasColumnType("numeric(24,8)");

                            b1.Property<decimal?>("TakeProfit")
                                .HasPrecision(24, 8)
                                .HasColumnType("numeric(24,8)");
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
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Language")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

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
                                .HasColumnType("uuid");

                            b1.Property<int>("Index")
                                .HasColumnType("integer");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<short?>("Performance")
                                .HasColumnType("smallint");

                            b1.Property<string>("Source")
                                .IsRequired()
                                .HasColumnType("text");

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
