namespace BLOGGER.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fotograf")]
    public partial class Fotograf
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fotograf()
        {
            Makale = new HashSet<Makale>();
            Uye = new HashSet<Uye>();
        }

        public int Id { get; set; }

        [StringLength(150)]
        public string Kucuk { get; set; }

        [StringLength(150)]
        public string Orta { get; set; }

        [StringLength(150)]
        public string Buyuk { get; set; }

        public bool? Aktif { get; set; }

        public bool? Onay { get; set; }

        public int? ResimTurID { get; set; }

        public virtual ResimTur ResimTur { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Makale> Makale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Uye> Uye { get; set; }
    }
}
