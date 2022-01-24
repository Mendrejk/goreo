using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace goreo
{
    public partial class postgresContext : DbContext
    {
        public postgresContext()
        {
        }

        public postgresContext(DbContextOptions<postgresContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Badge> Badges { get; set; }
        public virtual DbSet<Booklet> Booklets { get; set; }
        public virtual DbSet<BookletsBadge> BookletsBadges { get; set; }
        public virtual DbSet<BookletsRoute> BookletsRoutes { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<LocationsMountainGroup> LocationsMountainGroups { get; set; }
        public virtual DbSet<MountainGroup> MountainGroups { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<RoutesSection> RoutesSections { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("adminpack")
                .HasAnnotation("Relational:Collation", "Polish_Poland.1250");

            modelBuilder.Entity<Badge>(entity =>
            {
                entity.ToTable("badges");

                entity.HasIndex(e => e.Image, "badges_image_key")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "badges_name_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Booklet>(entity =>
            {
                entity.ToTable("booklets");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();
            });

            modelBuilder.Entity<BookletsBadge>(entity =>
            {
                entity.HasKey(e => new { e.BookletId, e.BadgeId })
                    .HasName("booklets_badges_pkey");

                entity.ToTable("booklets_badges");

                entity.Property(e => e.BookletId).HasColumnName("booklet_id");

                entity.Property(e => e.BadgeId).HasColumnName("badge_id");

                entity.Property(e => e.EarnDate).HasColumnName("earn_date");

                entity.HasOne(d => d.Badge)
                    .WithMany(p => p.BookletsBadges)
                    .HasForeignKey(d => d.BadgeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("booklets_badges_badge_id_fkey");

                entity.HasOne(d => d.Booklet)
                    .WithMany(p => p.BookletsBadges)
                    .HasForeignKey(d => d.BookletId)
                    .HasConstraintName("booklets_badges_booklet_id_fkey");
            });

            modelBuilder.Entity<BookletsRoute>(entity =>
            {
                entity.HasKey(e => new { e.BookletId, e.RouteId })
                    .HasName("booklets_routes_pkey");

                entity.ToTable("booklets_routes");

                entity.Property(e => e.BookletId).HasColumnName("booklet_id");

                entity.Property(e => e.RouteId).HasColumnName("route_id");

                entity.Property(e => e.EntryDate).HasColumnName("entry_date");

                entity.Property(e => e.isConfirmed)
                    .IsRequired()
                    .HasColumnName("is_confirmed");

                entity.HasOne(d => d.Booklet)
                    .WithMany(p => p.BookletsRoutes)
                    .HasForeignKey(d => d.BookletId)
                    .HasConstraintName("booklets_routes_booklet_id_fkey");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.BookletsRoutes)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("booklets_routes_route_id_fkey");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("locations");

                entity.HasIndex(e => e.Image, "locations_image_key")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "locations_name_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.XCoordinate).HasColumnName("x_coordinate");

                entity.Property(e => e.YCoordinate).HasColumnName("y_coordinate");
            });

            modelBuilder.Entity<LocationsMountainGroup>(entity =>
            {
                entity.HasKey(e => new { e.LocationId, e.MountainGroupName })
                    .HasName("locations_mountain_groups_pkey");

                entity.ToTable("locations_mountain_groups");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.MountainGroupName)
                    .HasMaxLength(255)
                    .HasColumnName("mountain_group_name");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.LocationsMountainGroups)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("locations_mountain_groups_location_id_fkey");

                entity.HasOne(d => d.MountainGroupNameNavigation)
                    .WithMany(p => p.LocationsMountainGroups)
                    .HasForeignKey(d => d.MountainGroupName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("locations_mountain_groups_mountain_group_name_fkey");
            });

            modelBuilder.Entity<MountainGroup>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("mountain_groups_pkey");

                entity.ToTable("mountain_groups");

                entity.HasIndex(e => e.Number, "mountain_groups_number_key")
                    .IsUnique();

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("number");
            });

            modelBuilder.Entity<RoutesSection>(entity =>
            {
                entity.HasKey(e => new { e.RouteId, e.SectionId })
                    .HasName("routes_sections_pkey");

                entity.ToTable("routes_sections");

                entity.Property(e => e.RouteId).HasColumnName("route_id");

                entity.Property(e => e.SectionId).HasColumnName("section_id");
                
                entity.Property(e => e.OrderNumber).HasColumnName("order_number");

                entity.Property(e => e.IsCounted).HasColumnName("is_counted");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.RoutesSections)
                    .HasForeignKey(d => d.RouteId)
                    .HasConstraintName("routes_sections_route_id_fkey");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.RoutesSections)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("routes_sections_section_id_fkey");
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("routes");

                entity.HasIndex(e => e.Name, "routes_name_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Routes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("routes_user_id_fkey");
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.ToTable("sections");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Approach).HasColumnName("approach");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Distance).HasColumnName("distance");

                entity.Property(e => e.LocationFrom).HasColumnName("location_from");

                entity.Property(e => e.LocationTo).HasColumnName("location_to");

                entity.Property(e => e.MountainTrail)
                    .HasMaxLength(255)
                    .HasColumnName("mountain_trail");

                entity.Property(e => e.Points).HasColumnName("points");

                entity.HasOne(d => d.LocationFromNavigation)
                    .WithMany(p => p.SectionLocationFromNavigations)
                    .HasForeignKey(d => d.LocationFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sections_location_from_fkey");

                entity.HasOne(d => d.LocationToNavigation)
                    .WithMany(p => p.SectionLocationToNavigations)
                    .HasForeignKey(d => d.LocationTo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sections_location_to_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.BookletId, "users_booklet_id_key")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "users_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.LeaderIdNo, "users_leader_id_no_key")
                    .IsUnique();

                entity.HasIndex(e => e.ProfileImage, "users_profile_image_key")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "users_username_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.BookletId).HasColumnName("booklet_id");

                entity.Property(e => e.CityOfResidence)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("city_of_residence");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsAdmin).HasColumnName("is_admin");

                entity.Property(e => e.IsLeader).HasColumnName("is_leader");

                entity.Property(e => e.LeaderIdNo)
                    .HasMaxLength(30)
                    .HasColumnName("leader_id_no");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.ProfileImage)
                    .HasMaxLength(255)
                    .HasColumnName("profile_image");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("surname");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("username");

                entity.HasOne(d => d.Booklet)
                    .WithOne(p => p.User)
                    .HasForeignKey<User>(d => d.BookletId)
                    .HasConstraintName("users_booklet_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}