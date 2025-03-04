﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SolarPlantAPI.Data;

#nullable disable

namespace SolarPlantAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20250208163617_UpdateSolarPowerPlantSchema")]
    partial class UpdateSolarPowerPlantSchema
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SolarPlantAPI.Models.ProductionRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("ForecastedProduction")
                        .HasColumnType("float");

                    b.Property<double>("RealProduction")
                        .HasColumnType("float");

                    b.Property<int>("SolarPowerPlantId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("SolarPowerPlantId");

                    b.ToTable("ProductionRecords");
                });

            modelBuilder.Entity("SolarPlantAPI.Models.SolarPowerPlant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("InstallationDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("InstalledPower")
                        .HasColumnType("float");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SolarPowerPlants");
                });

            modelBuilder.Entity("SolarPlantAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SolarPlantAPI.Models.ProductionRecord", b =>
                {
                    b.HasOne("SolarPlantAPI.Models.SolarPowerPlant", "SolarPowerPlant")
                        .WithMany("ProductionRecords")
                        .HasForeignKey("SolarPowerPlantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SolarPowerPlant");
                });

            modelBuilder.Entity("SolarPlantAPI.Models.SolarPowerPlant", b =>
                {
                    b.Navigation("ProductionRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
