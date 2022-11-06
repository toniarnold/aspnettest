﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace asplib.Model.Db
{
    public partial class ASP_DBEntities : DbContext
    {
        public ASP_DBEntities()
        {
        }

        public ASP_DBEntities(DbContextOptions<ASP_DBEntities> options)
            : base(options)
        {
        }

        public virtual DbSet<Main> Main { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Scaffolding:ConnectionString", "Data Source=(local);Initial Catalog=asp.db;Integrated Security=true");

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