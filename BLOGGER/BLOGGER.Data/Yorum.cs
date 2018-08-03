namespace BLOGGER.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Yorum")]
    public partial class Yorum
    {
        public int Id { get; set; }

        [StringLength(600)]
        public string Icerik { get; set; }

        public int? UyeID { get; set; }

        public int? MakaleID { get; set; }

        public DateTime? OlusturmaTarihi { get; set; }

        public bool? Aktif { get; set; }

        public bool? Onay { get; set; }

        public virtual Makale Makale { get; set; }
    }
}
