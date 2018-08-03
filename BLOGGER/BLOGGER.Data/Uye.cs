namespace BLOGGER.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Uye")]
    public partial class Uye
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Uye()
        {
            Makale = new HashSet<Makale>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string KullaniciAdi { get; set; }

        [StringLength(100)]
        public string Parola { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50)]
        public string AdSoyad { get; set; }

        public DateTime? OlusturulmaTarihi { get; set; }

        public int? FotoID { get; set; }

        public int? YetkiID { get; set; }

        public bool? Aktif { get; set; }

        public bool? Onay { get; set; }

        public virtual Fotograf Fotograf { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Makale> Makale { get; set; }

        public virtual Yetki Yetki { get; set; }
    }
}
