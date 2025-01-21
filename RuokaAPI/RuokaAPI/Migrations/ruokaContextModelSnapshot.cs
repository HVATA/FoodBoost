﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RuokaAPI.Data;

#nullable disable

namespace RuokaAPI.Migrations
{
    [DbContext(typeof(ruokaContext))]
    partial class ruokaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RuokaAPI.Properties.Model.Avsanat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Avainsanat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReseptiId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Avainsanat");
                });

            modelBuilder.Entity("RuokaAPI.Properties.Model.Kayttaja", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Etunimi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kayttajataso")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nimimerkki")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sahkopostiosoite")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salasana")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sukunimi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Kayttajat");
                });

            modelBuilder.Entity("RuokaAPI.Properties.Model.Resepti", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ainesosat")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Avainsanat")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Katseluoikeus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kuva1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kuva2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kuva3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kuva4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kuva5")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kuva6")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Tekijäid")
                        .HasColumnType("int");

                    b.Property<string>("Valmistuskuvaus")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Reseptit");
                });

            modelBuilder.Entity("RuokaAPI.Properties.Model.Ruokaaineet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ainesosat")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ruokaaineet");
                });

            modelBuilder.Entity("RuokaAPI.Properties.Model.Suosikit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("kayttajaID")
                        .HasColumnType("int");

                    b.Property<int>("reseptiID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Suosikit");
                });
#pragma warning restore 612, 618
        }
    }
}
