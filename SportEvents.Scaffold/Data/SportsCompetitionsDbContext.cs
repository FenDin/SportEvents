using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SportEvents.Scaffold.Models.Db;

namespace SportEvents.Scaffold.Data;

public partial class SportsCompetitionsDbContext : DbContext
{
    public SportsCompetitionsDbContext(DbContextOptions<SportsCompetitionsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventsCompetition> EventsCompetitions { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<ImagesUser> ImagesUsers { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<ParticipantsCompetition> ParticipantsCompetitions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    public virtual DbSet<SchoolsSportsSubType> SchoolsSportsSubTypes { get; set; }

    public virtual DbSet<Sport> Sports { get; set; }

    public virtual DbSet<SportSubtype> SportSubtypes { get; set; }

    public virtual DbSet<SportType> SportTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<vCompetitionParticipant> vCompetitionParticipants { get; set; }

    public virtual DbSet<vCompetitionsFull> vCompetitionsFulls { get; set; }

    public virtual DbSet<vContact> vContacts { get; set; }

    public virtual DbSet<vEventsCompetitionsFull> vEventsCompetitionsFulls { get; set; }

    public virtual DbSet<vParticipantsFull> vParticipantsFulls { get; set; }

    public virtual DbSet<vSchoolSportSubType> vSchoolSportSubTypes { get; set; }

    public virtual DbSet<vSportSubTypesFull> vSportSubTypesFulls { get; set; }

    public virtual DbSet<vUsersFull> vUsersFulls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Competit__3213E83F4FCFDA85");

            entity.ToTable("Competition");

            entity.HasIndex(e => e.idSportSubType, "IX_Competition_idSportSubType");

            entity.HasIndex(e => e.title, "UX_Competition_Title").IsUnique();

            entity.Property(e => e.dateEnd).HasColumnType("datetime");
            entity.Property(e => e.dateStart).HasColumnType("datetime");
            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.title).HasMaxLength(255);

            entity.HasOne(d => d.idSportSubTypeNavigation).WithMany(p => p.Competitions)
                .HasForeignKey(d => d.idSportSubType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Competiti__idSpo__6B1B9438");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Contact__3213E83F0E2CF20C");

            entity.ToTable("Contact");

            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.firstname).HasMaxLength(255);
            entity.Property(e => e.lastname).HasMaxLength(255);
            entity.Property(e => e.middlename).HasMaxLength(255);
            entity.Property(e => e.passwordHash).HasMaxLength(255);
            entity.Property(e => e.phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Event__3213E83F75899F3F");

            entity.ToTable("Event");

            entity.HasIndex(e => e.title, "UX_Competition_Title").IsUnique();

            entity.Property(e => e.dateEnd).HasColumnType("datetime");
            entity.Property(e => e.dateStart).HasColumnType("datetime");
            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.title).HasMaxLength(255);
        });

        modelBuilder.Entity<EventsCompetition>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__EventsCo__3213E83F8B29B569");

            entity.HasIndex(e => e.idCompetition, "IX_EC_idCompetition");

            entity.HasIndex(e => e.idEvent, "IX_EC_idEvent");

            entity.HasIndex(e => new { e.idEvent, e.idCompetition }, "UX_EventsCompetitions").IsUnique();

            entity.HasOne(d => d.idCompetitionNavigation).WithMany(p => p.EventsCompetitions)
                .HasForeignKey(d => d.idCompetition)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventsCom__idCom__6D03DCAA");

            entity.HasOne(d => d.idEventNavigation).WithMany(p => p.EventsCompetitions)
                .HasForeignKey(d => d.idEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventsCom__idEve__6C0FB871");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Image__3213E83FA1842F42");

            entity.ToTable("Image");

            entity.Property(e => e.dateCreated)
                .HasDefaultValueSql("(sysdatetime())", "DF_ImageCreated")
                .HasColumnType("datetime");
            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.title).HasMaxLength(255);
            entity.Property(e => e.url).HasMaxLength(255);
        });

        modelBuilder.Entity<ImagesUser>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ImagesUs__3213E83FC9373DAF");

            entity.HasIndex(e => e.idContact, "IX_ImagesUsers_idContact");

            entity.HasIndex(e => e.idImage, "IX_ImagesUsers_idImage");

            entity.HasIndex(e => new { e.idImage, e.idContact }, "UX_ImagesUsers").IsUnique();

            entity.HasOne(d => d.idContactNavigation).WithMany(p => p.ImagesUsers)
                .HasForeignKey(d => d.idContact)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImagesUse__idCon__74A4FE72");

            entity.HasOne(d => d.idImageNavigation).WithMany(p => p.ImagesUsers)
                .HasForeignKey(d => d.idImage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImagesUse__idIma__73B0DA39");
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Particip__3213E83FB7626B4F");

            entity.ToTable("Participant");

            entity.HasIndex(e => e.idContact, "IX_Participant_idContact");

            entity.HasIndex(e => e.idSchool, "IX_Participant_idSchool");

            entity.HasIndex(e => e.idContact, "UX_Participant_Contact").IsUnique();

            entity.HasOne(d => d.idContactNavigation).WithOne(p => p.Participant)
                .HasForeignKey<Participant>(d => d.idContact)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Participa__idCon__6EEC251C");

            entity.HasOne(d => d.idSchoolNavigation).WithMany(p => p.Participants)
                .HasForeignKey(d => d.idSchool)
                .HasConstraintName("FK__Participa__idSch__6DF800E3");
        });

        modelBuilder.Entity<ParticipantsCompetition>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Particip__3213E83F3CD36ACA");

            entity.HasIndex(e => e.idCompetition, "IX_PC_idCompetition");

            entity.HasIndex(e => e.idParticipant, "IX_PC_idParticipant");

            entity.HasIndex(e => new { e.idCompetition, e.idParticipant }, "UX_ParticipantsCompetitions").IsUnique();

            entity.HasOne(d => d.idCompetitionNavigation).WithMany(p => p.ParticipantsCompetitions)
                .HasForeignKey(d => d.idCompetition)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Participa__idCom__6FE04955");

            entity.HasOne(d => d.idParticipantNavigation).WithMany(p => p.ParticipantsCompetitions)
                .HasForeignKey(d => d.idParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Participa__idPar__70D46D8E");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Role__3213E83FFB93F059");

            entity.ToTable("Role");

            entity.HasIndex(e => e.title, "UX_Role_Title").IsUnique();

            entity.Property(e => e.title).HasMaxLength(255);
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__School__3213E83FBF5A7C01");

            entity.ToTable("School");

            entity.HasIndex(e => e.title, "UX_School_Title").IsUnique();

            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.title).HasMaxLength(255);
        });

        modelBuilder.Entity<SchoolsSportsSubType>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__SchoolsS__3213E83F4B9D6082");

            entity.HasIndex(e => e.idSchool, "IX_SSS_idSchool");

            entity.HasIndex(e => e.idSportSubType, "IX_SSS_idSportSubType");

            entity.HasIndex(e => new { e.idSportSubType, e.idSchool }, "UX_SchoolsSportsSubtypes").IsUnique();

            entity.HasOne(d => d.idSchoolNavigation).WithMany(p => p.SchoolsSportsSubTypes)
                .HasForeignKey(d => d.idSchool)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SchoolsSp__idSch__6A276FFF");

            entity.HasOne(d => d.idSportSubTypeNavigation).WithMany(p => p.SchoolsSportsSubTypes)
                .HasForeignKey(d => d.idSportSubType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SchoolsSp__idSpo__69334BC6");
        });

        modelBuilder.Entity<Sport>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Sport__3213E83FD896FF1F");

            entity.ToTable("Sport");

            entity.HasIndex(e => e.title, "UX_Sport_Title").IsUnique();

            entity.Property(e => e.title).HasMaxLength(255);
        });

        modelBuilder.Entity<SportSubtype>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__SportSub__3213E83FDEF42CF5");

            entity.ToTable("SportSubtype");

            entity.HasIndex(e => e.idSportType, "IX_SportSubType_idSportType");

            entity.Property(e => e.title).HasMaxLength(255);

            entity.HasOne(d => d.idSportTypeNavigation).WithMany(p => p.SportSubtypes)
                .HasForeignKey(d => d.idSportType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SportSubt__idSpo__683F278D");
        });

        modelBuilder.Entity<SportType>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__SportTyp__3213E83F2DC84A5B");

            entity.ToTable("SportType");

            entity.HasIndex(e => e.idSport, "IX_SportType_idSport");

            entity.Property(e => e.title).HasMaxLength(255);

            entity.HasOne(d => d.idSportNavigation).WithMany(p => p.SportTypes)
                .HasForeignKey(d => d.idSport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SportType__idSpo__674B0354");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__User__3213E83F8A3B8B6D");

            entity.ToTable("User");

            entity.HasIndex(e => e.idRole, "IX_User_idRole");

            entity.HasIndex(e => e.idContact, "UX_User_Contact").IsUnique();

            entity.HasOne(d => d.idContactNavigation).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.idContact)
                .HasConstraintName("FK__User__idContact__71C891C7");

            entity.HasOne(d => d.idRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.idRole)
                .HasConstraintName("FK__User__idRole__72BCB600");
        });

        modelBuilder.Entity<vCompetitionParticipant>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCompetitionParticipants");

            entity.Property(e => e.CompetitionDateEnd).HasColumnType("datetime");
            entity.Property(e => e.CompetitionDateStart).HasColumnType("datetime");
            entity.Property(e => e.CompetitionDescription).HasMaxLength(255);
            entity.Property(e => e.CompetitionTitle).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.MiddleName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.SchoolDescription).HasMaxLength(255);
            entity.Property(e => e.SchoolTitle).HasMaxLength(255);
            entity.Property(e => e.SportSubTypeTitle).HasMaxLength(255);
            entity.Property(e => e.SportTitle).HasMaxLength(255);
            entity.Property(e => e.SportTypeTitle).HasMaxLength(255);
        });

        modelBuilder.Entity<vCompetitionsFull>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCompetitionsFull");

            entity.Property(e => e.CompetitionDateEnd).HasColumnType("datetime");
            entity.Property(e => e.CompetitionDateStart).HasColumnType("datetime");
            entity.Property(e => e.CompetitionDescription).HasMaxLength(255);
            entity.Property(e => e.CompetitionTitle).HasMaxLength(255);
            entity.Property(e => e.SportSubTypeTitle).HasMaxLength(255);
            entity.Property(e => e.SportTitle).HasMaxLength(255);
            entity.Property(e => e.SportTypeTitle).HasMaxLength(255);
        });

        modelBuilder.Entity<vContact>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vContact");

            entity.Property(e => e.ContactId).ValueGeneratedOnAdd();
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.MiddleName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<vEventsCompetitionsFull>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vEventsCompetitionsFull");

            entity.Property(e => e.CompetitionDateEnd).HasColumnType("datetime");
            entity.Property(e => e.CompetitionDateStart).HasColumnType("datetime");
            entity.Property(e => e.CompetitionTitle).HasMaxLength(255);
            entity.Property(e => e.EventDateEnd).HasColumnType("datetime");
            entity.Property(e => e.EventDateStart).HasColumnType("datetime");
            entity.Property(e => e.EventTitle).HasMaxLength(255);
            entity.Property(e => e.SportSubTypeTitle).HasMaxLength(255);
            entity.Property(e => e.SportTitle).HasMaxLength(255);
            entity.Property(e => e.SportTypeTitle).HasMaxLength(255);
        });

        modelBuilder.Entity<vParticipantsFull>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vParticipantsFull");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.MiddleName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.SchoolDescription).HasMaxLength(255);
            entity.Property(e => e.SchoolTitle).HasMaxLength(255);
        });

        modelBuilder.Entity<vSchoolSportSubType>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vSchoolSportSubTypes");

            entity.Property(e => e.SchoolDescription).HasMaxLength(255);
            entity.Property(e => e.SchoolTitle).HasMaxLength(255);
            entity.Property(e => e.SportSubTypeTitle).HasMaxLength(255);
            entity.Property(e => e.SportTitle).HasMaxLength(255);
            entity.Property(e => e.SportTypeTitle).HasMaxLength(255);
        });

        modelBuilder.Entity<vSportSubTypesFull>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vSportSubTypesFull");

            entity.Property(e => e.SportSubTypeTitle).HasMaxLength(255);
            entity.Property(e => e.SportTitle).HasMaxLength(255);
            entity.Property(e => e.SportTypeTitle).HasMaxLength(255);
        });

        modelBuilder.Entity<vUsersFull>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vUsersFull");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.MiddleName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RoleTitle).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
