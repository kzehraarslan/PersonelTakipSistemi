using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PersonelSistemi
{
    public partial class AnaSayfa : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["KullaniciAdi"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblKullanici.Text = Session["KullaniciAdi"].ToString();
                lblTarih.Text = DateTime.Now.ToString("dd MMMM yyyy, dddd");
                lblSaat.Text = DateTime.Now.ToString("HH:mm:ss");

                GetStatistics();
            }
        }

        private void GetStatistics()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Toplam personel
                SqlCommand cmdPersonel = new SqlCommand("SELECT COUNT(*) FROM PersonelBilgileri", conn);
                int toplamPersonel = (int)cmdPersonel.ExecuteScalar();
                lblToplamPersonel.Text = toplamPersonel.ToString();

                // Toplam departman
                SqlCommand cmdDepartman = new SqlCommand("SELECT COUNT(*) FROM Departmanlar", conn);
                int toplamDepartman = (int)cmdDepartman.ExecuteScalar();
                lblToplamDepartman.Text = toplamDepartman.ToString();

                // Ortalama maaş (null ise 0)
                SqlCommand cmdOrtalamaMaas = new SqlCommand("SELECT ISNULL(AVG(Maas), 0) FROM PersonelBilgileri", conn);
                decimal ortalamaMaas = (decimal)cmdOrtalamaMaas.ExecuteScalar();
                lblOrtalamaMaas.Text = ortalamaMaas.ToString("C2"); // Para birimi formatı
            }
        }
    }
}