using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PersonelSistemi
{
    public partial class Login : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        protected void btnGiris_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = txtKullaniciAdi.Text.Trim();
            string sifre = txtSifre.Text.Trim();

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            {
                lblMesaj.Text = "Lütfen kullanıcı adı ve şifre giriniz.";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string sql = "SELECT Rol FROM Kullanicilar WHERE KullaniciAdi = @kullanici AND Sifre = @sifre";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@kullanici", kullaniciAdi);
                    cmd.Parameters.AddWithValue("@sifre", sifre);

                    conn.Open();
                    object rolObj = cmd.ExecuteScalar();

                    if (rolObj != null)
                    {
                        string rol = rolObj.ToString().Trim(); // Boşlukları sil

                        // Oturuma kesin doğru veriyi atıyoruz
                        Session["KullaniciAdi"] = kullaniciAdi;
                        Session["Rol"] = rol;

                        Response.Redirect("Anasayfa.aspx");
                    }
                    else
                    {
                        lblMesaj.Text = "Geçersiz kullanıcı adı veya şifre.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMesaj.Text = "Sistem hatası: " + ex.Message;
            }
        }
    }
}