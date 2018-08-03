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
//PAGED LIST ICIN GEREKLI OLAN KUTUPHANELER
//MAKALE LISTELE OLAYINDA YAPICAM O YUZDEN MAKALE LISTELE METODUNDADA BIRAZ DEGISIKLIKLER OLACAK
using PagedList;
using PagedList.Mvc;

namespace BLOGGER.Web.Controllers
{
    public class AdminController : Controller
    {
        BloggerContext ctx = new BloggerContext();
        Repository<Kategori> kategori = new Repository<Kategori>();
        Repository<Fotograf> fotograf = new Repository<Fotograf>();
        Repository<Makale> makale = new Repository<Makale>();
        Repository<Etiket> etiket = new Repository<Etiket>();
        Repository<MakaleEtiket> makaleetiket = new Repository<MakaleEtiket>();
        Repository<Uye> uye = new Repository<Uye>();
        Repository<Yorum> yorum = new Repository<Yorum>();
        Repository<Yetki> yetki = new Repository<Yetki>();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        //PARAMETRESI + TOPAGEDLIST OLAYI TAMAMEN PAGEDLIST ICIN 
        //2.FARKLILIK BURADA . 3 ISE VIEW BOLUMUNDE
        public ActionResult MakaleListele(int Page=1)
        {
            var data = makale.Select().Where(x=>x.Aktif==true).OrderByDescending(x=>x.OlusturmaTarihi).ToPagedList(Page,10);
            return View(data);
        }
        public ActionResult makaleEkle()
        {
            ViewBag.KategoriID = new SelectList(kategori.Select(), "Id", "Adi");
            return View();
        }
      
        public ActionResult MakaleDetay(int id)
        {
            return View(makale.GetByID(id));
        }

        //Tinymce DE GIRDI HATASI VERIYOR HTML KOD BARINDIRIDIG ICIN BUNUN COZUMU OLARAK DENEDIM
        //[ValidateInput(false)]
       
        [HttpPost]
        public ActionResult makaleEkle(Makale temp, HttpPostedFileBase foto, string Etiketler)
        {

            string resimAd = Guid.NewGuid().ToString();
            if (temp.Baslik == null || temp.Icerik == null)
            {
                Session["makaleSorun"] = "Lütfen makalenin adını veya içeriğini boş bırakmayınız.";

                return RedirectToAction("makaleEkle");
            }
            if (foto == null)
            {
                Session["makaleSorun"] = "Lütfen makaleye resim yükleyiniz.";

                return RedirectToAction("makaleEkle");
            }
            else
            {
                Image rsm = Image.FromStream(foto.InputStream);
                Bitmap btOrta = new Bitmap(rsm, Tools.OrtaBoyutSize.Width, Tools.OrtaBoyutSize.Height);
                Bitmap btBuyuk = new Bitmap(rsm, Tools.BuyukBoyutSize.Width, Tools.BuyukBoyutSize.Height);
                string ortaYol = "/Uploads/MakaleResim/Orta/" + resimAd + Path.GetExtension(foto.FileName);
                string buyukYol = "/Uploads/MakaleResim/Buyuk/" + resimAd + Path.GetExtension(foto.FileName);
                btOrta.Save(Server.MapPath(ortaYol));
                btBuyuk.Save(Server.MapPath(buyukYol));

                Fotograf ft = new Fotograf();

                //MAAKE RESMI =3
                ft.ResimTurID = 3;
                ft.Orta = ortaYol;
                ft.Buyuk = buyukYol;
                ft.Aktif = true;
                ft.Onay = true;



                if (!fotograf.Insert(ft))
                {
                    Session["makaleSorun"] = "Resim yüklenemedi lütfen tekrar deneyiniz";

                    return RedirectToAction("makaleEkle");
                }

              
                int yuklenenResimID = fotograf.Select().FirstOrDefault(x => x.Orta.Contains(resimAd)).Id;
                temp.FotoID = yuklenenResimID;
                temp.Aktif = true;
                temp.Begenme = 0;
                temp.Okunma = 0;
                temp.OlusturmaTarihi = DateTime.Now;
                temp.UyeID = Convert.ToInt32(Session["aktifUyeId"]);

                if (!makale.Insert(temp))
                {
                    Session["makaleSorun"] = "Makale yüklenemedi lütfen tekrar deneyiniz";

                    return RedirectToAction("makaleEkle");

                }
                int makaleID = makale.Select().FirstOrDefault(x => x.FotoID == yuklenenResimID).Id;

                if (!string.IsNullOrEmpty(Etiketler))
                {
                    string[] etikets = Etiketler.Split(',');
                    foreach (var etk in etikets)
                    {
                        Etiket et = new Etiket() { Adi = etk };
                        et.Aktif = true;
                        etiket.Insert(et);







                        int etiketID = etiket.Select().FirstOrDefault(x => x.Adi == etk).Id;

                        MakaleEtiket me = new MakaleEtiket();
                        me.EtiketID = etiketID;
                        me.MakaleID = makaleID;
                        me.Aktif = true;
                        makaleetiket.Insert(me);


                    }

                }



            }
            Session["makaleSorun"] = "Makale başarıyla eklendi..";

            return RedirectToAction("makaleEkle");
        }

        public ActionResult MakaleDuzenle(int id)
        {
            Makale temp = makale.GetByID(id);

            if (temp != null)
            {
                ViewBag.KategoriID = new SelectList(kategori.Select(), "Id", "Adi", temp.KategoriID);
                return View(temp);
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult MakaleDuzenle(Makale temp, HttpPostedFileBase Fotog)
        {
            //BURADA MAKALE IDYI ALDK
            Makale yeni = makale.GetByID(temp.Id);

            if (Fotog != null)
            {
                


                Image img = Image.FromStream(Fotog.InputStream);
                Bitmap bOrta = new Bitmap(img, Tools.OrtaBoyutSize);
                Bitmap bBuyuk = new Bitmap(img, Tools.BuyukBoyutSize);
                string resimAd = Guid.NewGuid().ToString() + Path.GetExtension(Fotog.FileName);
                string buyukYol = "/Uploads/MakaleResim/Buyuk/" + resimAd;
                string ortaYol = "/Uploads/MakaleResim/Orta/" + resimAd;
                Fotograf eskiFoto = yeni.Fotograf;
                Fotograf yenifoto = new Fotograf();
                yenifoto.Buyuk = buyukYol;
                yenifoto.Orta = ortaYol;
                yenifoto.Aktif = true;
                yenifoto.Onay = false;
                //Makale Resim belirteci
                yenifoto.ResimTurID = 3;
                



            


                // YENI RESIM DATABASEYE YUKLENDI
                if (!fotograf.Insert(yenifoto))
                {
                    Session["makaleDuzenleSorun"] = "Fotoğraf değiştirme de sorun yaşandı";
                    return RedirectToAction("makaleDuzenle");
                }
                string eskibuyukYol = yeni.Fotograf.Buyuk;
                string eskiOrtaYol = yeni.Fotograf.Orta;

                //RESIMLERI SERVERE CEK
                bOrta.Save(Server.MapPath("/Uploads/MakaleResim/Orta/" + resimAd));
                bBuyuk.Save(Server.MapPath("/Uploads/MakaleResim/Buyuk/" + resimAd));

                //MAKALEMİZİN BOŞ KALAN FOTO IDSINI DOLDURDUK
                int yenifotoId = fotograf.Select().FirstOrDefault(x => x.Buyuk == buyukYol).Id;
                yeni.FotoID = yenifotoId;


                //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ SAVECHANGE DE KAYDETME YOK!
                makale.SAVE();

                //ESKI RESMIN ID SINI VE KENDISINI SILDIK.
                if (!fotograf.Delete(eskiFoto.Id))
                {
                    Session["makaleDuzenleSorun"] = "Eski fotograf silinemedi..";
                    return RedirectToAction("makaleDuzenle");
                }

                //RESMI SERVERDAN SILDIK
                System.IO.File.Delete(Server.MapPath(eskibuyukYol));

                System.IO.File.Delete(Server.MapPath(eskiOrtaYol));

               
                

                
            }

            yeni.Icerik = temp.Icerik;
            yeni.Baslik = temp.Baslik;
            yeni.KategoriID = temp.KategoriID;
            if (!makale.SAVE())
            {
                Session["makaleDuzenleSorun"] = "Makale Değiştirilemedi..";
                return RedirectToAction("makaleDuzenle");
            }

            Session["makaleDuzenleSorun"] = "Makale başarıyla güncellendi";


            return RedirectToAction("makaleDuzenle");



        }

        public ActionResult MakaleSil(int id)
        {
            Makale silenecek = makale.GetByID(id);
            if (silenecek==null)
            {
                return HttpNotFound();
            }

            if (!makale.Delete(id))
            {
                Session["makaleSil"] = "Silme işleminde sorun oluştu.";
            }
            Session["makaleSil"] = "Silme işlemi başarıyla tamamlandı.";

            return RedirectToAction("MakaleListele", "Admin");
        }

        public ActionResult OnayliUyeler()
        {
            var data = uye.Select().Where(x => x.Onay == true&& x.Aktif==true).OrderByDescending(y=>y.OlusturulmaTarihi);
            return View(data);
        }
        public ActionResult OnaysizUyeler()
        {
            var data = uye.Select().Where(x => x.Onay == false && x.Aktif == true).OrderByDescending(y => y.OlusturulmaTarihi).ToList();
            return View(data);
        }
        public ActionResult ButunUyeler()
        {
            var data = uye.Select();
            return View(data);
        }

        public ActionResult UyeSil(int id)
        {
            var data = uye.GetByID(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult UyeSil(string id)
        {
            int uyeID = Convert.ToInt32(id);
            Uye temp = uye.GetByID(uyeID);
            if (!uye.Delete(uyeID))
            {
                return HttpNotFound();
            }
           
            Session["uyeSilmeOlayi"] ="Kullanıcı Adı  '"+ temp.KullaniciAdi + "' olan  '"+temp.AdSoyad +"' adlı kişisi başarıyla silindi.";
            return RedirectToAction("OnayliUyeler");
        }

        //UYE OYNAY DUURUMU
        public ActionResult OnayDurumu(int id)
        {
            Uye temp = uye.GetByID(id);
            return View(temp);
        }

        //UYE OYNAY DUURUMU
        [HttpPost]
        public ActionResult OnayDurumu(string id)
        {
            int uyeID = Convert.ToInt32(id);
            Uye temp = uye.GetByID(uyeID);
            if (temp.Onay == true)
            {
               
                temp.Onay = false;
                if (uye.SAVE())
                {
                    Session["uyeOnayOlayi"] ="'"+ temp.KullaniciAdi + "' kullanıcı adına sahip , '" + temp.AdSoyad + "' adlı üye başarıyla onaysız listesine eklendi ..";
                }
                return RedirectToAction("OnayliUyeler");
            }
           else
            {
                temp.Onay = true;
                if (uye.SAVE())
                {
                    Session["uyeOnayOlayi"] = "'" + temp.KullaniciAdi + "' kullanıcı adına sahip , '" + temp.AdSoyad + "' adlı üye başarıyla onaylılar listesine eklendi ..";
                }
                return RedirectToAction("OnaysizUyeler");
            }
        }

        public ActionResult YorumSekmesi()
        {
            var data = yorum.Select().OrderByDescending(x=>x.OlusturmaTarihi);
            return View(data);
        }
        public ActionResult OnaysizYorumlar()
        {
            var data = yorum.Select().Where(x=>x.Onay==false).OrderByDescending(x => x.OlusturmaTarihi).ToList();
            return View(data);
        }

        public ActionResult YorumSil(int id)
        {
            var data = yorum.GetByID(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult YorumSil(string id)
        {
            if (id==null)
            {
                return HttpNotFound();
            }
            int yorumId = Convert.ToInt32(id);
            if (yorumId==0)
            {
                return HttpNotFound();
            }
           
            if (!yorum.Delete(yorumId))
            {
                return HttpNotFound();
            }
            Session["yorumOlayi"] = "Yorum başarıyla silindi";
            return RedirectToAction("YorumSekmesi","Admin");
        }

        public ActionResult YorumDuzenle(int id=0)
        {
            if (id==0)
            {
                return HttpNotFound();
            }
 var data = yorum.GetByID(id);
            if (data==null)
            {
                return HttpNotFound();
            }

            return View(data);
        }
        [HttpPost]
        public ActionResult YorumDuzenle(Yorum temp)
        {
            Yorum eski = yorum.GetByID(temp.Id);
            eski.Icerik = temp.Icerik;
            if (yorum.SAVE())
            {
                Session["yorumOlayi"] = "Yorum başarıyla düzenlendi";
                return RedirectToAction("YorumSekmesi", "Admin");
            }
            Session["yorumOlayi"] = "Yorum düzenlemede sorun yaşandı..";
            return RedirectToAction("YorumSekmesi", "Admin");
        }

        public ActionResult YetkiListele()
        {
            var data = yetki.Select();
            return View(data);
        }
        public ActionResult YetkiSil(int id=0)
        {

            if (id.GetType()!=typeof(Int32))
            {
                return HttpNotFound();
            }
            var data = yetki.GetByID(id);
            if (data==null)
            {
                return HttpNotFound();
            }

            return View(data);
        }

        [HttpPost]
        public ActionResult YetkiSil(string id=null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            int yetkiID = Convert.ToInt32(id);
            if (yetkiID==0)
            {
                return HttpNotFound();
            }
           
            if (!yetki.Delete(yetkiID))
            {
                return HttpNotFound();
            }

            return RedirectToAction("YetkiListele", "Admin");
        }

        public ActionResult YetkiEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult YetkiEkle(Yetki temp)
        {
            if (temp.Yetkisi==null)
            {
                Session["yetkiSorun"] = "Yetki eklenmede sorun oluştu";
                return View();
            }
            temp.Aktif = true;
            if (!yetki.Insert(temp))
            {
                Session["yetkiSorun"] = "Yetki eklenmede sorun oluştu";
                return View();
            }
            Session["yetkiOlaylari"] = "Yetki eklendi";
            return RedirectToAction("YetkiListele");
        }

        public ActionResult YetkiVer(int id=0)
        {
             Uye temp = uye.GetByID(id);
            if (temp==null)
            {
                return HttpNotFound();
            }
           
            ViewBag.YetkiAdi = new SelectList(yetki.Select(), "Id", "Yetkisi",temp.YetkiID);
            return View(temp);
        }

        [HttpPost]
        public ActionResult YetkiVer(string id=null,int yetkisi=0)
        {
            if (string.IsNullOrEmpty(id)|| yetkisi==0)
            {
                return HttpNotFound();
            }
            Uye temp = uye.GetByID(Convert.ToInt32(id));
            Yetki temp2 = yetki.GetByID(yetkisi);
            if (temp==null||temp2==null)
            {
                return HttpNotFound();
            }
            temp.YetkiID = yetkisi;
            if (!uye.SAVE())
            {
                return HttpNotFound();
            }
            Session["yetkilendirmeOlayi"] = temp.KullaniciAdi + "  adlı kişi başarıyla '" + temp2.Yetkisi + "' yetkisine uyarlandı.. ";
            return RedirectToAction("OnayliUyeler");
        }
        public ActionResult UyeSec()
        {
            ViewBag.uyeler = new SelectList(uye.Select(), "Id", "KullaniciAdi");
            return View();
        }

      


    }
}