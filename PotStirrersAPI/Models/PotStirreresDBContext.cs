using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PotStirrersAPI.Models
{
    public partial class PotStirreresDBContext : DbContext
    {
        public PotStirreresDBContext()
        {
        }

        public PotStirreresDBContext(DbContextOptions<PotStirreresDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AllCpuWin> AllCpuWins { get; set; } = null!;
        public virtual DbSet<AllPvpWin> AllPvpWins { get; set; } = null!;
        public virtual DbSet<AppVersion> AppVersions { get; set; } = null!;
        public virtual DbSet<Chest> Chests { get; set; } = null!;
        public virtual DbSet<ChestType> ChestTypes { get; set; } = null!;
        public virtual DbSet<DailyCpuWin> DailyCpuWins { get; set; } = null!;
        public virtual DbSet<Device> Devices { get; set; } = null!;
        public virtual DbSet<DiceSkin> DiceSkins { get; set; } = null!;
        public virtual DbSet<GameAnalytic> GameAnalytics { get; set; } = null!;
        public virtual DbSet<GameAnalyticView> GameAnalyticViews { get; set; } = null!;
        public virtual DbSet<GiveawayKey> GiveawayKeys { get; set; } = null!;
        public virtual DbSet<IngredientSkin> IngredientSkins { get; set; } = null!;
        public virtual DbSet<LoggedIn> LoggedIns { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Player> Players { get; set; } = null!;
        public virtual DbSet<PlayerProfile> PlayerProfiles { get; set; } = null!;
        public virtual DbSet<Title> Titles { get; set; } = null!;
        public virtual DbSet<UserDiceUnlock> UserDiceUnlocks { get; set; } = null!;
        public virtual DbSet<UserIngredientUnlock> UserIngredientUnlocks { get; set; } = null!;
        public virtual DbSet<WeeklyCpuWin> WeeklyCpuWins { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=potstirrersserver.database.windows.net;Database=PotStirreresDB;User Id=Sql_James;Password=Version101#;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllCpuWin>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("AllCpuWins");

                entity.Property(e => e.Edmxid).HasColumnName("EDMXID");
            });

            modelBuilder.Entity<AllPvpWin>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("AllPvpWins");

                entity.Property(e => e.AllPvpwins).HasColumnName("AllPVPWins");

                entity.Property(e => e.Edmxid).HasColumnName("EDMXID");
            });

            modelBuilder.Entity<AppVersion>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AppVersion");

                entity.Property(e => e.AppVersion1)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("AppVersion");
            });

            modelBuilder.Entity<Chest>(entity =>
            {
                entity.ToTable("Chest");

                entity.Property(e => e.ChestSize).HasDefaultValueSql("((1))");

                entity.Property(e => e.ChestTypeId).HasDefaultValueSql("((1))");

                entity.Property(e => e.FinishUnlock).HasColumnType("datetime");

                entity.HasOne(d => d.ChestType)
                    .WithMany(p => p.Chests)
                    .HasForeignKey(d => d.ChestTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Chest_ChestType");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Chests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Chest_Player");
            });

            modelBuilder.Entity<ChestType>(entity =>
            {
                entity.ToTable("ChestType");

                entity.Property(e => e.ChestTypeId).ValueGeneratedNever();

                entity.Property(e => e.ChestTypeName).HasMaxLength(50);
            });

            modelBuilder.Entity<DailyCpuWin>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("DailyCpuWins");

                entity.Property(e => e.Edmxid).HasColumnName("EDMXID");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");

                entity.Property(e => e.DeviceId).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Device_Player");
            });

            modelBuilder.Entity<DiceSkin>(entity =>
            {
                entity.ToTable("DiceSkin");

                entity.Property(e => e.DiceSkinId).ValueGeneratedNever();

                entity.Property(e => e.DiceSkinName).HasMaxLength(50);

                entity.Property(e => e.Rarity).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<GameAnalytic>(entity =>
            {
                entity.HasKey(e => e.GameId);

                entity.Property(e => e.GameEndTime).HasColumnType("datetime");

                entity.Property(e => e.GameStartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Player1)
                    .WithMany(p => p.GameAnalyticPlayer1s)
                    .HasForeignKey(d => d.Player1Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GameAnalytics_Player");

                entity.HasOne(d => d.Player2)
                    .WithMany(p => p.GameAnalyticPlayer2s)
                    .HasForeignKey(d => d.Player2Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GameAnalytics_Player1");
            });

            modelBuilder.Entity<GameAnalyticView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("GameAnalyticView");

                entity.Property(e => e.Edmxid).HasColumnName("EDMXID");

                entity.Property(e => e.GameEndDate).HasColumnType("date");

                entity.Property(e => e.Player1).HasMaxLength(256);

                entity.Property(e => e.Player2).HasMaxLength(256);

                entity.Property(e => e.VsCpu).HasColumnName("VsCPU");
            });

            modelBuilder.Entity<GiveawayKey>(entity =>
            {
                entity.ToTable("GiveawayKey");

                entity.Property(e => e.ClaimedTime).HasColumnType("datetime");

                entity.Property(e => e.KeyCode).HasMaxLength(10);

                entity.HasOne(d => d.ClaimedBy)
                    .WithMany(p => p.GiveawayKeys)
                    .HasForeignKey(d => d.ClaimedById)
                    .HasConstraintName("FK_GiveawayKey_Player");
            });

            modelBuilder.Entity<IngredientSkin>(entity =>
            {
                entity.ToTable("IngredientSkin");

                entity.Property(e => e.IngredientSkinId).ValueGeneratedNever();

                entity.Property(e => e.IngredientSkinName).HasMaxLength(50);

                entity.Property(e => e.Rarity).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<LoggedIn>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginDate });

                entity.ToTable("LoggedIn");

                entity.Property(e => e.LoginDate).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LoggedIns)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoggedIn_Player");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.Property(e => e.Body).HasMaxLength(256);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FromId).HasDefaultValueSql("((5))");

                entity.Property(e => e.Subject).HasMaxLength(64);

                entity.HasOne(d => d.From)
                    .WithMany(p => p.MessageFroms)
                    .HasForeignKey(d => d.FromId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_Player1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MessageUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_Player");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("Player");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EffectsVolume).HasDefaultValueSql("((0.5))");

                entity.Property(e => e.Email)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.GameVolume).HasDefaultValueSql("((0.5))");

                entity.Property(e => e.Level).HasDefaultValueSql("((1))");

                entity.Property(e => e.MasterVolume).HasDefaultValueSql("((0.5))");

                entity.Property(e => e.Password).HasMaxLength(256);

                entity.Property(e => e.TurnVolume).HasDefaultValueSql("((0.5))");

                entity.Property(e => e.Username).HasMaxLength(256);

                entity.Property(e => e.VoiceVolume).HasDefaultValueSql("((0.5))");

                entity.HasMany(d => d.DiceSkins)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "SelectedDie",
                        l => l.HasOne<DiceSkin>().WithMany().HasForeignKey("DiceSkinId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SelectedDice_DiceSkin"),
                        r => r.HasOne<Player>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SelectedDice_Player"),
                        j =>
                        {
                            j.HasKey("UserId", "DiceSkinId");

                            j.ToTable("SelectedDice");
                        });

                entity.HasMany(d => d.Friends)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "Friend",
                        l => l.HasOne<Player>().WithMany().HasForeignKey("FriendId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Friends_Player"),
                        r => r.HasOne<Player>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Friends_Friends"),
                        j =>
                        {
                            j.HasKey("UserId", "FriendId");

                            j.ToTable("Friends");
                        });

                entity.HasMany(d => d.IngredientSkins)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "SelectedIngredient",
                        l => l.HasOne<IngredientSkin>().WithMany().HasForeignKey("IngredientSkinId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SelectedIngredient_IngredientSkin"),
                        r => r.HasOne<Player>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SelectedIngredient_Player"),
                        j =>
                        {
                            j.HasKey("UserId", "IngredientSkinId");

                            j.ToTable("SelectedIngredient");
                        });

                entity.HasMany(d => d.Titles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "SelectedTitle",
                        l => l.HasOne<Title>().WithMany().HasForeignKey("TitleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SelectedTitle_Title"),
                        r => r.HasOne<Player>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SelectedTitle_Player"),
                        j =>
                        {
                            j.HasKey("UserId", "TitleId");

                            j.ToTable("SelectedTitle");
                        });

                entity.HasMany(d => d.TitlesNavigation)
                    .WithMany(p => p.UsersNavigation)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserTitleUnlock",
                        l => l.HasOne<Title>().WithMany().HasForeignKey("TitleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_User_Title_Unlock_Title"),
                        r => r.HasOne<Player>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_User_Title_Unlock_Player"),
                        j =>
                        {
                            j.HasKey("UserId", "TitleId").HasName("PK_User_Title_Unlock_1");

                            j.ToTable("User_Title_Unlock");
                        });

                entity.HasMany(d => d.Users)
                    .WithMany(p => p.Friends)
                    .UsingEntity<Dictionary<string, object>>(
                        "Friend",
                        l => l.HasOne<Player>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Friends_Friends"),
                        r => r.HasOne<Player>().WithMany().HasForeignKey("FriendId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Friends_Player"),
                        j =>
                        {
                            j.HasKey("UserId", "FriendId");

                            j.ToTable("Friends");
                        });
            });

            modelBuilder.Entity<PlayerProfile>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("PlayerProfile");

                entity.Property(e => e.AllPvpwins).HasColumnName("AllPVPWins");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Username).HasMaxLength(256);
            });

            modelBuilder.Entity<Title>(entity =>
            {
                entity.ToTable("Title");

                entity.Property(e => e.TitleId).ValueGeneratedNever();

                entity.Property(e => e.EarnDescription).HasMaxLength(256);

                entity.Property(e => e.TitleName).HasMaxLength(64);
            });

            modelBuilder.Entity<UserDiceUnlock>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.DiceSkinId })
                    .HasName("PK_User_Dice_Unlock_1");

                entity.ToTable("User_Dice_Unlock");

                entity.HasOne(d => d.DiceSkin)
                    .WithMany(p => p.UserDiceUnlocks)
                    .HasForeignKey(d => d.DiceSkinId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Dice_Unlock_DiceSkin");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDiceUnlocks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Dice_Unlock_Player");
            });

            modelBuilder.Entity<UserIngredientUnlock>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.IngredientSkinId })
                    .HasName("PK_User_Ingredient_Unlock_1");

                entity.ToTable("User_Ingredient_Unlock");

                entity.HasOne(d => d.IngredientSkin)
                    .WithMany(p => p.UserIngredientUnlocks)
                    .HasForeignKey(d => d.IngredientSkinId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Ingredient_Unlock_IngredientSkin");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserIngredientUnlocks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Ingredient_Unlock_Player");
            });

            modelBuilder.Entity<WeeklyCpuWin>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("WeeklyCpuWins");

                entity.Property(e => e.Edmxid).HasColumnName("EDMXID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
