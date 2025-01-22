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

            modelBuilder.Entity("AinesosaResepti", b =>
                {
                    b.Property<int>("AinesosatId")
                        .HasColumnType("int");

                    b.Property<int>("ReseptitId")
                        .HasColumnType("int");

                    b.HasKey("AinesosatId", "ReseptitId");

                    b.HasIndex("ReseptitId");

                    b.ToTable("AinesosaResepti");
                });

            modelBuilder.Entity("AvainsanaResepti", b =>
                {
                    b.Property<int>("AvainsanatId")
                        .HasColumnType("int");

                    b.Property<int>("ReseptitId")
                        .HasColumnType("int");

                    b.HasKey("AvainsanatId", "ReseptitId");

                    b.HasIndex("ReseptitId");

                    b.ToTable("AvainsanaResepti");
                });

            modelBuilder.Entity("RuokaAPI.Properties.Model.Ainesosa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nimi")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Ainesosat");
                });

            modelBuilder.Entity("RuokaAPI.Properties.Model.Avainsana", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Sana")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

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
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.HasKey("Id");

                    b.ToTable("Reseptit");
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

            modelBuilder.Entity("AinesosaResepti", b =>
                {
                    b.HasOne("RuokaAPI.Properties.Model.Ainesosa", null)
                        .WithMany()
                        .HasForeignKey("AinesosatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RuokaAPI.Properties.Model.Resepti", null)
                        .WithMany()
                        .HasForeignKey("ReseptitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AvainsanaResepti", b =>
                {
                    b.HasOne("RuokaAPI.Properties.Model.Avainsana", null)
                        .WithMany()
                        .HasForeignKey("AvainsanatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RuokaAPI.Properties.Model.Resepti", null)
                        .WithMany()
                        .HasForeignKey("ReseptitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
