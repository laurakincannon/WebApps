using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WDLMassage.Models
{
    public partial class Team102DBContext : DbContext
    {
        public Team102DBContext()
        {
        }

        public Team102DBContext(DbContextOptions<Team102DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<Intake> Intake { get; set; }
        public virtual DbSet<Massage> Massage { get; set; }
        public virtual DbSet<Outtake> Outtake { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=buscissql1601\\cisweb;Database=Team102DB;User ID=cloudy;Password=compute;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentPk);

                entity.ToTable("APPOINTMENT");

                entity.HasIndex(e => e.AppointmentPk)
                    .HasName("IX_APPOINTMENT");

                entity.Property(e => e.AppointmentPk).HasColumnName("AppointmentPK");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Duration)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Fkclient)
                    .HasColumnName("FKClient")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Fkintake).HasColumnName("FKIntake");

                entity.Property(e => e.Fkmassage).HasColumnName("FKMassage");

                entity.Property(e => e.Fkouttake).HasColumnName("FKOuttake");

                entity.Property(e => e.Fkstaff).HasColumnName("FKStaff");

                entity.Property(e => e.Time).HasColumnType("time(0)");

                entity.HasOne(d => d.FkclientNavigation)
                    .WithMany(p => p.AppointmentFkclientNavigation)
                    .HasForeignKey(d => d.Fkclient)
                    .HasConstraintName("FK_APPOINTMENT_USER_CLIENT");

                entity.HasOne(d => d.FkintakeNavigation)
                    .WithMany(p => p.Appointment)
                    .HasForeignKey(d => d.Fkintake)
                    .HasConstraintName("FK_APPOINTMENT_INTAKE");

                entity.HasOne(d => d.FkmassageNavigation)
                    .WithMany(p => p.Appointment)
                    .HasForeignKey(d => d.Fkmassage)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_APPOINTMENT_MASSAGE");

                entity.HasOne(d => d.FkouttakeNavigation)
                    .WithMany(p => p.Appointment)
                    .HasForeignKey(d => d.Fkouttake)
                    .HasConstraintName("FK_APPOINTMENT_OUTTAKE");

                entity.HasOne(d => d.FkstaffNavigation)
                    .WithMany(p => p.AppointmentFkstaffNavigation)
                    .HasForeignKey(d => d.Fkstaff)
                    .HasConstraintName("FK_APPOINTMENT_USER_STAFF");
            });

            modelBuilder.Entity<Intake>(entity =>
            {
                entity.HasKey(e => e.IntakePk);

                entity.ToTable("INTAKE");

                entity.Property(e => e.IntakePk).HasColumnName("IntakePK");

                entity.Property(e => e.Fkappointment).HasColumnName("FKAppointment");

                entity.Property(e => e.Fkclient).HasColumnName("FKClient");

                entity.Property(e => e.Fkstaff).HasColumnName("FKStaff");

                entity.Property(e => e.FocusAreas)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Medications)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sensitives)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Surgeries)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkappointmentNavigation)
                    .WithMany(p => p.Intake)
                    .HasForeignKey(d => d.Fkappointment)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_INTAKE_APPOINTMENT");

                entity.HasOne(d => d.FkclientNavigation)
                    .WithMany(p => p.IntakeFkclientNavigation)
                    .HasForeignKey(d => d.Fkclient)
                    .HasConstraintName("FK_INTAKE_USER_CLIENT");

                entity.HasOne(d => d.FkstaffNavigation)
                    .WithMany(p => p.IntakeFkstaffNavigation)
                    .HasForeignKey(d => d.Fkstaff)
                    .HasConstraintName("FK_INTAKE_USER_STAFF");
            });

            modelBuilder.Entity<Massage>(entity =>
            {
                entity.HasKey(e => e.MassagePk);

                entity.ToTable("MASSAGE");

                entity.Property(e => e.MassagePk).HasColumnName("MassagePK");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.Imagename)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Outtake>(entity =>
            {
                entity.HasKey(e => e.SurveyPk);

                entity.ToTable("OUTTAKE");

                entity.Property(e => e.SurveyPk).HasColumnName("SurveyPK");

                entity.Property(e => e.Comments)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Fkappointment).HasColumnName("FKAppointment");

                entity.Property(e => e.Fkclient).HasColumnName("FKClient");

                entity.Property(e => e.Fkstaff).HasColumnName("FKStaff");

                entity.HasOne(d => d.FkappointmentNavigation)
                    .WithMany(p => p.Outtake)
                    .HasForeignKey(d => d.Fkappointment)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OUTTAKE_APPOINTMENT");

                entity.HasOne(d => d.FkclientNavigation)
                    .WithMany(p => p.OuttakeFkclientNavigation)
                    .HasForeignKey(d => d.Fkclient)
                    .HasConstraintName("FK_OUTTAKE_USER_CLIENT");

                entity.HasOne(d => d.FkstaffNavigation)
                    .WithMany(p => p.OuttakeFkstaffNavigation)
                    .HasForeignKey(d => d.Fkstaff)
                    .HasConstraintName("FK_OUTTAKE_USER_STAFF");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserPk)
                    .HasName("PK_CLIENT");

                entity.ToTable("USER");

                entity.Property(e => e.UserPk).HasColumnName("UserPK");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsAdmin).HasDefaultValueSql("((0))");

                entity.Property(e => e.NameFirst)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NameLast)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
