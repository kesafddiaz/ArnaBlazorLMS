﻿// <auto-generated />
using System;
using ArnaBlazorTest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ArnaBlazorTest.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250705045943_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ArnaBlazorTest.Models.Assignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("MaterialUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.AssignmentProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AnswersJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AssignmentId")
                        .HasColumnType("int");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SubmittedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

                    b.HasIndex("UserId");

                    b.ToTable("AssignmentProgresses");
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssignmentId")
                        .HasColumnType("int");

                    b.Property<string>("CorrectAnswer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OptionsJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ManagerId")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ManagerId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Learner"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Manager"
                        });
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.AssignmentProgress", b =>
                {
                    b.HasOne("ArnaBlazorTest.Models.Assignment", "Assignment")
                        .WithMany("AssignmentProgresses")
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ArnaBlazorTest.Models.User", "User")
                        .WithMany("AssignmentProgresses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Assignment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.Question", b =>
                {
                    b.HasOne("ArnaBlazorTest.Models.Assignment", "Assignment")
                        .WithMany("Questions")
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assignment");
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.User", b =>
                {
                    b.HasOne("ArnaBlazorTest.Models.User", "Manager")
                        .WithMany("Subordinates")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ArnaBlazorTest.Models.UserRole", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Manager");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.Assignment", b =>
                {
                    b.Navigation("AssignmentProgresses");

                    b.Navigation("Questions");
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.User", b =>
                {
                    b.Navigation("AssignmentProgresses");

                    b.Navigation("Subordinates");
                });

            modelBuilder.Entity("ArnaBlazorTest.Models.UserRole", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
