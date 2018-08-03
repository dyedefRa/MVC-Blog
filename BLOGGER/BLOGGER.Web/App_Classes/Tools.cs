using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;

namespace BLOGGER.Web.App_Classes
{
    public class Tools
    {
        public static Size BuyukBoyutSize
        {
            get
            {
                Size donecek = new Size();
                donecek.Height = Convert.ToInt32(ConfigurationManager.AppSettings["BuyukResimHeight"].ToString());
                donecek.Width = Convert.ToInt32(ConfigurationManager.AppSettings["BuyukResimWidth"].ToString());
                return donecek;
            }
        }

        public static Size OrtaBoyutSize
        {
            get
            {
                Size donecek = new Size();
                donecek.Height = Convert.ToInt32(ConfigurationManager.AppSettings["OrtaResimHeight"].ToString());
                donecek.Width = Convert.ToInt32(ConfigurationManager.AppSettings["OrtaResimWidth"].ToString());
                return donecek;
            }
        }
        public static Size KucukBoyutSize
        {
            get
            {
                Size donecek = new Size();
                donecek.Height = Convert.ToInt32(ConfigurationManager.AppSettings["KucukResimHeight"].ToString());
                donecek.Width = Convert.ToInt32(ConfigurationManager.AppSettings["KucukResimWidth"].ToString());
                return donecek;
            }
        }
    }
}