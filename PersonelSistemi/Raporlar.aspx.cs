using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PersonelSistemi
{
    public partial class Raporlar : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hfChartData.Value = "[]";
                hfChartDataUnvan.Value = "[]";
                hfMesaiChartData.Value = "[]";

                FillAyDropdown();
                FillYilDropdown();
            }
        }

        // --- MAAS RAPORLARI ---
        protected void btnGetir_Click(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(txtBaslangic.Text, out DateTime baslangic) ||
                !DateTime.TryParse(txtBitis.Text, out DateTime bitis))
            {
                hfChartData.Value = "[]";
                hfChartDataUnvan.Value = "[]";
                return;
            }

            LoadChartData(baslangic, bitis);
            LoadUnvanChartData(baslangic, bitis);

            ScriptManager.RegisterStartupScript(this, GetType(), "drawMaasCharts", "drawCharts();", true);
        }

        private void LoadChartData(DateTime baslangic, DateTime bitis)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT d.DepartmanAdi AS Departman, 
                           SUM(m.ToplamMaas) AS ToplamMaas
                    FROM Maaslar m
                    INNER JOIN PersonelBilgileri p ON m.PersonelID = p.PersonelID
                    INNER JOIN Departmanlar d ON p.DepartmanID = d.DepartmanID
                    WHERE m.BaslangicTarihi >= @Baslangic AND m.BitisTarihi <= @Bitis
                    GROUP BY d.DepartmanAdi
                    ORDER BY SUM(m.ToplamMaas) DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@Baslangic", baslangic);
                da.SelectCommand.Parameters.AddWithValue("@Bitis", bitis);

                DataTable dt = new DataTable();
                da.Fill(dt);

                var chartData = new System.Collections.Generic.List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    chartData.Add(new
                    {
                        Departman = row["Departman"].ToString(),
                        ToplamMaas = Convert.ToDecimal(row["ToplamMaas"])
                    });
                }

                hfChartData.Value = new JavaScriptSerializer().Serialize(chartData);
            }
        }

        private void LoadUnvanChartData(DateTime baslangic, DateTime bitis)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT u.UnvanAdi AS Unvan, 
                           SUM(m.ToplamMaas) AS ToplamMaas
                    FROM Maaslar m
                    INNER JOIN PersonelBilgileri p ON m.PersonelID = p.PersonelID
                    INNER JOIN Unvanlar u ON p.UnvanID = u.UnvanID
                    WHERE m.BaslangicTarihi >= @Baslangic AND m.BitisTarihi <= @Bitis
                    GROUP BY u.UnvanAdi
                    ORDER BY SUM(m.ToplamMaas) DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@Baslangic", baslangic);
                da.SelectCommand.Parameters.AddWithValue("@Bitis", bitis);

                DataTable dt = new DataTable();
                da.Fill(dt);

                var chartData = new System.Collections.Generic.List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    chartData.Add(new
                    {
                        Unvan = row["Unvan"].ToString(),
                        ToplamMaas = Convert.ToDecimal(row["ToplamMaas"])
                    });
                }

                hfChartDataUnvan.Value = new JavaScriptSerializer().Serialize(chartData);
            }
        }

        // --- MESAI RAPORLARI ---
        protected void btnMesaiGetir_Click(object sender, EventArgs e)
        {
            int ay = int.Parse(ddlAy.SelectedValue);
            int yil = int.Parse(ddlYil.SelectedValue);

            DateTime baslangic = new DateTime(yil, ay, 1);
            DateTime bitis = baslangic.AddMonths(1).AddDays(-1);

            LoadMesaiChartData(baslangic, bitis);
            LoadMesaiGunlukChartData(baslangic, bitis);
            ScriptManager.RegisterStartupScript(this, GetType(), "drawMesaiCharts", "drawMesaiChart(); drawGunlukMesaiChart();", true);
            // Mesai grafiğini çiz
            ScriptManager.RegisterStartupScript(this, GetType(), "drawMesaiChart", "drawMesaiChart();", true);
        }

        private void LoadMesaiChartData(DateTime baslangic, DateTime bitis)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT d.DepartmanAdi AS Departman,
                           SUM(DATEDIFF(HOUR, pgc.GirisSaati, pgc.CikisSaati)) AS ToplamSaat
                    FROM PersonelGirisCikis pgc
                    INNER JOIN PersonelBilgileri p ON pgc.PersonelID = p.PersonelID
                    INNER JOIN Departmanlar d ON p.DepartmanID = d.DepartmanID
                    WHERE pgc.Tarih BETWEEN @Baslangic AND @Bitis
                    GROUP BY d.DepartmanAdi
                    ORDER BY ToplamSaat DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@Baslangic", baslangic);
                da.SelectCommand.Parameters.AddWithValue("@Bitis", bitis);

                DataTable dt = new DataTable();
                da.Fill(dt);

                var chartData = new System.Collections.Generic.List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    chartData.Add(new
                    {
                        Departman = row["Departman"].ToString(),
                        ToplamSaat = Convert.ToInt32(row["ToplamSaat"])
                    });
                }

                hfMesaiChartData.Value = new JavaScriptSerializer().Serialize(chartData);
            }
        }

        // --- DROPDOWN DOLDURMA ---
        private void FillAyDropdown()
        {
            ddlAy.Items.Clear();
            ddlAy.Items.Add(new ListItem("Ocak", "1"));
            ddlAy.Items.Add(new ListItem("Şubat", "2"));
            ddlAy.Items.Add(new ListItem("Mart", "3"));
            ddlAy.Items.Add(new ListItem("Nisan", "4"));
            ddlAy.Items.Add(new ListItem("Mayıs", "5"));
            ddlAy.Items.Add(new ListItem("Haziran", "6"));
            ddlAy.Items.Add(new ListItem("Temmuz", "7"));
            ddlAy.Items.Add(new ListItem("Ağustos", "8"));
            ddlAy.Items.Add(new ListItem("Eylül", "9"));
            ddlAy.Items.Add(new ListItem("Ekim", "10"));
            ddlAy.Items.Add(new ListItem("Kasım", "11"));
            ddlAy.Items.Add(new ListItem("Aralık", "12"));
            ddlAy.SelectedValue = DateTime.Now.Month.ToString();
        }

        private void FillYilDropdown()
        {
            ddlYil.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < 10; i++)
            {
                int year = currentYear - i;
                ddlYil.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
            ddlYil.SelectedValue = currentYear.ToString();
        }
        private void LoadMesaiGunlukChartData(DateTime baslangic, DateTime bitis)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                SELECT 
                dep.DepartmanAdi,
                CAST(PersonelGirisCikis.Tarih AS DATE) AS Gun,
                SUM(DATEDIFF(MINUTE, GirisSaati, CikisSaati)/60.0) AS ToplamMesai
                FROM 
                PersonelGirisCikis
                JOIN 
                PersonelBilgileri Personel ON Personel.PersonelID = PersonelGirisCikis.PersonelID
                JOIN dbo.Departmanlar dep ON dep.DepartmanID = Personel.DepartmanID
                WHERE 
                PersonelGirisCikis.Tarih BETWEEN @Baslangic AND @Bitis
                GROUP BY 
                dep.DepartmanAdi,
                CAST(PersonelGirisCikis.Tarih AS DATE)
                ORDER BY 
                CAST(PersonelGirisCikis.Tarih AS DATE)";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@Baslangic", baslangic);
                da.SelectCommand.Parameters.AddWithValue("@Bitis", bitis);

                DataTable dt = new DataTable();
                da.Fill(dt);

                var chartData = new System.Collections.Generic.List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    chartData.Add(new
                    {
                        Tarih = row["Tarih"].ToString(),
                        ToplamSaat = Convert.ToInt32(row["ToplamSaat"])
                    });
                }

                hfMesaiGunlukChartData.Value = new JavaScriptSerializer().Serialize(chartData);
            }
        }

        // --- PANEL BUTONLARI ---
        protected void btnMaasRapor_Click(object sender, EventArgs e)
        {
            pnlMaasRaporlari.Visible = true;
            pnlMesaiRaporlari.Visible = false;
        }

        protected void btnMesaiRapor_Click(object sender, EventArgs e)
        {
            pnlMaasRaporlari.Visible = false;
            pnlMesaiRaporlari.Visible = true;
        }
    }
}