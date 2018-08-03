using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLOGGER.Data;
using BLOGGER.BusinessLayer;
using System.Drawing;
using BLOGGER.Web.App_Classes;
using System.IO;
using PagedList;
using PagedList.Mvc;
//CRYPTO ICIN GEERKELI KUTUPHANE

using System.Web.Helpers;

namespace BLOGGER.Web.Controllers
{
    public class HomeController : Controller
    {
       
        //KategoriDetay/3

        Repository<Makale> makaleler = new Repository<Makale>();
        Repository<Kategori> kategoriler = new Repository<Kategori>();
        Repository<Fotograf> fotograf = new Repository<Fotograf>();
        Repository<Uye> uyeler = new Repository<Uye>();
        Repository<Yorum> yorum = new Repository<Yorum>();


        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Hakkimizda()
        {
            return View();
        }
        public ActionResult Iletisim()
        {
            return View();
        }
        public ActionResult KategoriWidget()
        {
            var data = kategoriler.Select().Where(x=>x.Aktif==true).ToList();
            return View(data);
        }
        public ActionResult MakaleWidget(int Page = 1)
        {
            var data = makaleler.Select().Where(x => x.Aktif == true).OrderByDescending(x => x.OlusturmaTarihi).ToPagedList(Page, 5);
            return View(data);
        }

        public ActionResult MakaleDetay(int id)
        {
            Makale data = makaleler.GetByID(id);
            if (data==null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        public ActionResult UyeOl()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UyeOl(Uye temp, HttpPostedFileBase foto, string parolaTekrar)
        {
            string resimAd = Guid.NewGuid().ToString();
            //Anonimler için . Eger foto==null ise bu devereye girecek
            temp.FotoID = 7;
            if (temp.AdSoyad == null || temp.KullaniciAdi == null || temp.Parola == null || temp.Email == null)
            {
                Session["uyeSorun"] = "Lütfen hiç bir alanı boş geçmeyiniz";

                return RedirectToAction("UyeOl");
            }
            if (temp.Parola != parolaTekrar)
            {
                Session["uyeSorun"] = "Parolalar uyuşmuyor.";

                return RedirectToAction("UyeOl");
            }
            if (foto != null)
            {

                Image rsm = Image.FromStream(foto.InputStream);
                Bitmap btOrta = new Bitmap(rsm, Tools.OrtaBoyutSize.Width, Tools.OrtaBoyutSize.Height);
                Bitmap btKucuk = new Bitmap(rsm, Tools.KucukBoyutSize.Width, Tools.KucukBoyutSize.Height);
                string ortaYol = "/Uploads/ProfilResim/Orta/" + resimAd + Path.GetExtension(foto.FileName);
                string kucukYol = "/Uploads/ProfilResim/Kucuk/" + resimAd + Path.GetExtension(foto.FileName);
                btOrta.Save(Server.MapPath(ortaYol));
                btKucuk.Save(Server.MapPath(kucukYol));

                Fotograf ft = new Fotograf();

                //UYE RESMI =1
                ft.ResimTurID = 1;
                ft.Orta = ortaYol;
                ft.Kucuk = kucukYol;
                ft.Aktif = true;
                ft.Onay = true;
                


                if (!fotograf.Insert(ft))
                {
                    Session["uyeSorun"] = "Resim yüklenemedi lütfen tekrar deneyiniz";

                    return RedirectToAction("UyeOl");
                }


                int yuklenenResimID = fotograf.Select().FirstOrDefault(x => x.Orta.Contains(resimAd)).Id;
                temp.FotoID = yuklenenResimID;

            }
            temp.Aktif = true;
            temp.Onay = false;
            temp.OlusturulmaTarihi = DateTime.Now;
            //ADMIN DEGIL UYE DEDIK 2 DIYEREK
            temp.YetkiID = 2;
            temp.Parola = Crypto.Hash(temp.Parola, "MD5");
            if (!uyeler.Insert(temp))
            {
                Session["uyeSorun"] = "Uye kaydı işleminiz başarısız..";

                return RedirectToAction("UyeOl");
            }

            Session["uyeSorun"] = "Uye kaydı işleminiz başarıyla tamamlandı..";
            return RedirectToAction("UyeOl");

        }

        public ActionResult GirisYap()
        {
            return View();
        }

        //CRYPO HELPERS OLAYIIII @@@@
        [HttpPost]
        public ActionResult GirisYap(string kullaniciAdi, string parola, string beniHatirla)
        {
            var md5pass = Crypto.Hash(parola,"MD5");
            Uye temp = uyeler.Select().Where(x => x.KullaniciAdi == kullaniciAdi && x.Parola ==md5pass).FirstOrDefault();
            if (temp == null)
            {
                Session["girisSorun"] = "Giriş işlemi başarısız";
                return RedirectToAction("GirisYap");
            }
            else if (temp.Aktif == false)
            {
                Session["girisSorun"] = "Üyeliğiniz aktif değildir..";
                return RedirectToAction("GirisYap");
            }
            else
            {
                Session.RemoveAll();
                Session["aktifUyeAdi"] = temp.AdSoyad;
                Session["aktifUyeId"] = temp.Id;
                Session["aktifUyeYetkiId"] = temp.YetkiID;
                return RedirectToAction("Index");
            }
        }
        public ActionResult CikisYap()
        {
            Session["aktifUye"] = null;
            Session["aktifUyeId"] = null;
            Session["aktifUyeYetkiId"] = null;
            Session.RemoveAll();
            

            return RedirectToAction("Index");
        }

        public ActionResult Profil(int id)
        {
            var temp = uyeler.GetByID(id);
            if (temp==null)
            {
                return HttpNotFound();
            }
            return View(temp);
        }

        public ActionResult ProfilDuzenle(int id)
        {
            var temp = uyeler.GetByID(id);
            return View(temp);
        }

        [HttpPost]
        public ActionResult ProfilDuzenle(Uye temp, HttpPostedFileBase Fotog)
        {

            Uye eski = uyeler.GetByID(temp.Id);
            if (Fotog != null)
            {



                Image img = Image.FromStream(Fotog.InputStream);
                Bitmap bOrta = new Bitmap(img, Tools.OrtaBoyutSize);
                Bitmap bKucuk = new Bitmap(img, Tools.KucukBoyutSize);
                string resimAd = Guid.NewGuid().ToString() + Path.GetExtension(Fotog.FileName);
                string kucukYol = "/Uploads/ProfilResim/Kucuk/" + resimAd;
                string ortaYol = "/Uploads/ProfilResim/Orta/" + resimAd;


                Fotograf eskiFoto = eski.Fotograf;
                Fotograf yenifoto = new Fotograf();
                yenifoto.Kucuk = kucukYol;
                yenifoto.Orta = ortaYol;
                yenifoto.Aktif = true;
                yenifoto.Onay = true;
                //Makale Resim belirteci
                    yenifoto.ResimTurID = 1;







                //YENI RESIM DATABASEYE YUKLENDI
                    if (!fotograf.Insert(yenifoto))
                {
                    Session["profilSorun"] = "Fotoğraf değiştirme de sorun yaşandı";
                    return RedirectToAction("ProfilDuzenle",new { id = temp.Id });
                }
                string eskiKucukYol = eskiFoto.Kucuk;
                string eskiOrtaYol = eskiFoto.Orta;

                //RESIMLERI SERVERE CEK
                    bOrta.Save(Server.MapPath(ortaYol));
                bKucuk.Save(Server.MapPath(kucukYol));

                //MAKALEMİZİN BOŞ KALAN FOTO IDSINI DOLDURDUK
                    int yenifotoId = fotograf.Select().FirstOrDefault(x => x.Orta == ortaYol).Id;
                eski.FotoID = yenifotoId;


                //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ SAVECHANGE DE KAYDETME YOK!
                uyeler.SAVE();

                //ESKI RESMIN ID SINI VE KENDISINI SILDIK.
                if (!fotograf.Delete(eskiFoto.Id))
                {
                    Session["profilSorun"] = "Eski fotograf silinemedi..";
                    return RedirectToAction("ProfilDuzenle", new { id = temp.Id });
                }

                //RESMI SERVERDAN SILDIK
                    System.IO.File.Delete(Server.MapPath(eskiKucukYol));

                System.IO.File.Delete(Server.MapPath(eskiOrtaYol));





            }

            if (!string.IsNullOrEmpty(temp.Email))
            {
                eski.Email = temp.Email;
            }
            if (!string.IsNullOrEmpty(temp.AdSoyad))
            {
                eski.AdSoyad = temp.AdSoyad;
            }


            if (!uyeler.SAVE())
            {
                Session["profilSorun"] = "Profil bilgileri değiştirme de sorun yaşandı";
                return RedirectToAction("ProfilDuzenle", new { id = temp.Id });
            }

            Session["profilSorun"] = "Profil bilgileri başarıyla değiştirildi..";


            return RedirectToAction("ProfilDuzenle", new { id = temp.Id });
        }

            
        public ActionResult HesapSil(int id)
        {
            Uye temp = uyeler.GetByID(id);
            if (temp==null)
            {
                return HttpNotFound();
            }
            return View(temp);
        }

        [HttpPost]
        public ActionResult HesapSil(string id)
        { int uyeID = Convert.ToInt32(id);
            Uye temp = uyeler.GetByID(uyeID);
            if (temp==null)
            {
                return HttpNotFound();
            }
            if (uyeler.Delete(uyeID))
            {
                //SILINME GERCEKLESIRSE INDEXE GONDER 
                Session.RemoveAll();
                Session["hesapDurumu"] = "Hesap silme işlemi başarıyla tamamlandı..";
                
          

                return RedirectToAction("Index", "Home");
            }
            //SILINME SORUN YASATIRSA OLDUGUN SAYFADA KAL
            Session["hesapDurumu2"] = "Hesap silme işlemi başarısız";
            return RedirectToAction("HesapSil",new { id = uyeID });
            
        }

        [HttpPost]
        public ActionResult YorumGonder(string makaleID, string icerik)
        {
            int uyeID = Convert.ToInt32(Session["aktifUyeId"]);
            if (makaleID==null|| uyeID==0)
            {
                return RedirectToAction("MakaleDetay",new { id = Convert.ToInt32(makaleID) });
            }
            yorum.Insert(new Yorum() { Aktif = true, Icerik = icerik, MakaleID = Convert.ToInt32(makaleID), UyeID = uyeID, Onay = true ,OlusturmaTarihi=DateTime.Now});
            return RedirectToAction("MakaleDetay", new { id = Convert.ToInt32(makaleID) });
        }

        public ActionResult YorumSil(int Id,int makID)
        {
            if (Id==0||makID==0)
            {
                return HttpNotFound();
            }
            Yorum temp = yorum.GetByID(Id);
            if (temp==null)
            {
                return HttpNotFound();
            }
            yorum.Delete(Id);

            return RedirectToAction("MakaleDetay", new { id = makID });
        }
        public int OkunmaArtir(int Id)
        {
            Makale temp = makaleler.GetByID(Id);
            if (temp==null)
            {
                return 0;
            }
            temp.Okunma += 1;
            makaleler.SAVE();
            return temp.Okunma.Value;
        }


        //IPAGEDLIST<> VAR
        public ActionResult KategoriDetay(int id,int Page=1)
        {
            var data = makaleler.Select().Where(x => x.KategoriID == id).ToPagedList(Page,5);

            return View(data);
        }

        public ActionResult BlogAra(string aranan)
        {
            if (string.IsNullOrEmpty(aranan))
            {
                aranan = "";
            }
           var ara  = makaleler.Select().Where(x => x.Baslik.Contains(aranan)).OrderByDescending(Y => Y.OlusturmaTarihi).ToList();
            return View(ara);
        }

        public ActionResult SonYorumlar()
        {
            var data = yorum.Select().Where(x=>x.Onay==true).OrderByDescending(x => x.OlusturmaTarihi).Take(5).ToList();
            return View(data);
        }

        public ActionResult PopulerMakalelerWidget()
        {

            var data = makaleler.Select().Where(x => x.Aktif == true).OrderByDescending(x => x.Okunma).Take(5).ToList();
            return View(data);
        }

   

    }


}