namespace BLOGGER.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BloggerContext : DbContext
    {
        public BloggerContext()
            : base("name=BloggerContext")
        {
        }

        public virtual DbSet<Etiket> Etiket { get; set; }
        public virtual DbSet<Fotograf> Fotograf { get; set; }
        public virtual DbSet<Kategori> Kategori { get; set; }
        public virtual DbSet<Makale> Makale { get; set; }
        public virtual DbSet<MakaleEtiket> MakaleEtiket { get; set; }
        public virtual DbSet<ResimTur> ResimTur { get; set; }
        public virtual DbSet<Uye> Uye { get; set; }
        public virtual DbSet<Yetki> Yetki { get; set; }
        public virtual DbSet<Yorum> Yorum { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Etiket>()
                .HasMany(e => e.MakaleEtiket)
                .WithRequired(e => e.Etiket)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Fotograf>()
                .HasMany(e => e.Makale)
                .WithOptional(e => e.Fotograf)
                .HasForeignKey(e => e.FotoID);

            modelBuilder.Entity<Fotograf>()
                .HasMany(e => e.Uye)
                .WithOptional(e => e.Fotograf)
                .HasForeignKey(e => e.FotoID);

            modelBuilder.Entity<Makale>()
                .HasMany(e => e.MakaleEtiket)
                .WithRequired(e => e.Makale)
                .WillCascadeOnDelete(false);
        }
    }
}
