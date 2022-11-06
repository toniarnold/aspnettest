﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace apiservice.Model.Db
{
    public partial class AspserviceDbContext : DbContext
    {
        public AspserviceDbContext()
        {
        }

        public AspserviceDbContext(DbContextOptions<AspserviceDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accesscode> Accesscode { get; set; } = null!;
        public virtual DbSet<Main> Main { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Scaffolding:ConnectionString", "Data Source=(local);Initial Catalog=apiservice.db;Integrated Security=true");

            modelBuilder.Entity<Accesscode>(entity =>
            {
                entity.HasKey(e => e.Accesscodeid)
                    .IsClustered(false);

                entity.HasIndex(e => e.Phonenumber, "IX_Accesscode_phonenumber");

                entity.Property(e => e.Accesscodeid).HasColumnName("accesscodeid");

                entity.Property(e => e.Accesscode1)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("accesscode");

                entity.Property(e => e.Changed)
                    .HasColumnType("datetime")
                    .HasColumnName("changed")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("phonenumber");

                entity.Property(e => e.Session)
                    .HasColumnName("session")
                    .HasDefaultValueSql("newid()");

                entity.HasOne(d => d.SessionNavigation)
                    .WithMany(p => p.Accesscode)
                    .HasForeignKey(d => d.Session)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Accesscode_Main");
            });

            modelBuilder.Entity<Main>(entity =>
            {
                entity.HasKey(e => e.Session)
                    .IsClustered(false);

                entity.HasIndex(e => e.Mainid, "CIX_Main_mainid")
                    .IsUnique()
                    .IsClustered();

                entity.HasIndex(e => e.Clsid, "IX_Main_clsid");

                entity.Property(e => e.Session)
                    .HasColumnName("session")
                    .HasDefaultValueSql("newid()");

                entity.Property(e => e.Changed)
                    .HasColumnType("datetime")
                    .HasColumnName("changed")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Clsid).HasColumnName("clsid");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Main1).HasColumnName("main");

                entity.Property(e => e.Mainid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("mainid");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}