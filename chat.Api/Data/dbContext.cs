
    ﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using chat.Modelos;

    public class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> options)
            : base(options)
        {
        }

        public DbSet<chat.Modelos.Grupo> Grupo { get; set; } = default!;

        public DbSet<chat.Modelos.Mensaje> Mensaje { get; set; } = default!;

        public DbSet<chat.Modelos.Notificacion> Notificacion { get; set; } = default!;

        public DbSet<chat.Modelos.User> User { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relación Mensaje -> User (Remitente)
            modelBuilder.Entity<Mensaje>()
                .HasOne(m => m.UserRemitente)
                .WithMany(u => u.MensajesEnviados)
                .HasForeignKey(m => m.UserRemitenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relación Mensaje -> User (Destinatario)
            modelBuilder.Entity<Mensaje>()
                .HasOne(m => m.UserDestinatario)
                .WithMany(u => u.MensajesRecibidos)
                .HasForeignKey(m => m.UserDestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relación Mensaje -> Grupo
            modelBuilder.Entity<Mensaje>()
                .HasOne(m => m.Grupo)
                .WithMany(g => g.Mensajes)
                .HasForeignKey(m => m.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

