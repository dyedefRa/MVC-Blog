namespace BLOGGER.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Makale")]
    public partial class Makale
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Makale()
        {
            MakaleEtiket = new HashSet<MakaleEtiket>();
            Yorum = new HashSet<Yorum>();
        }

        public int Id { get; set; }

        [StringLength(500)]
        public string Baslik { get; set; }

        public string Icerik { get; set; }

        public int? FotoID { get; set; }

        public DateTime? OlusturmaTarihi { get; set; }

        public int? KategoriID { get; set; }

        public int? UyeID { get; set; }

        public int? Okunma { get; set; }

        public int? Begenme { get; set; }

        public bool? Aktif { get; set; }

        public virtual Fotograf Fotograf { get; set; }

        public virtual Kategori Kategori { get; set; }

        public virtual Uye Uye { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MakaleEtiket> MakaleEtiket { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Yorum> Yorum { get; set; }
    }
}
