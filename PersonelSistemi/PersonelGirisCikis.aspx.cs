using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PersonelSistemi
{
    public partial class PersonelGirisCikis : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DoldurPersonel();
                DoldurFiltrePersonel();
                DoldurKayitlar(true); // Sayfa ilk açıldığında sadece bugünün kayıtları gösterilsin
                btnIptal.Visible = false;
            }
        }

        private void DoldurPersonel()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT PersonelID, Ad + ' ' + Soyad AS AdSoyad FROM PersonelBilgileri ORDER BY Ad, Soyad";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlPersonel.DataSource = dt;
                ddlPersonel.DataTextField = "AdSoyad";
                ddlPersonel.DataValueField = "PersonelID";
                ddlPersonel.DataBind();

                ddlPersonel.Items.Insert(0, new ListItem("-- Personel Seç --", "0"));
            }
        }

        private void DoldurFiltrePersonel()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT PersonelID, Ad + ' ' + Soyad AS AdSoyad FROM PersonelBilgileri ORDER BY Ad, Soyad";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlFiltrePersonel.DataSource = dt;
                ddlFiltrePersonel.DataTextField = "AdSoyad";
                ddlFiltrePersonel.DataValueField = "PersonelID";
                ddlFiltrePersonel.DataBind();

                ddlFiltrePersonel.Items.Insert(0, new ListItem("-- Tüm Personel --", "0"));
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (ddlPersonel.SelectedValue == "0")
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Lütfen bir personel seçin.";
                return;
            }

            if (!TimeSpan.TryParse(txtGiris.Text.Trim(), out TimeSpan giris) ||
                !TimeSpan.TryParse(txtCikis.Text.Trim(), out TimeSpan cikis))
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Saatleri HH:mm formatında girin.";
                return;
            }

            int personelId = int.Parse(ddlPersonel.SelectedValue);
            DateTime bugun = DateTime.Today;
            int? kayitId = KayitVarMi(personelId, bugun);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd;

                if (kayitId.HasValue)
                {
                    string sqlUpdate = @"UPDATE PersonelGirisCikis 
                                         SET GirisSaati = @Giris, CikisSaati = @Cikis 
                                         WHERE KayitID = @ID";
                    cmd = new SqlCommand(sqlUpdate, conn);
                    cmd.Parameters.AddWithValue("@ID", kayitId.Value);
                }
                else
                {
                    string sqlInsert = @"INSERT INTO PersonelGirisCikis (PersonelID, Tarih, GirisSaati, CikisSaati)
                                         VALUES (@PersonelID, @Tarih, @Giris, @Cikis)";
                    cmd = new SqlCommand(sqlInsert, conn);
                    cmd.Parameters.AddWithValue("@PersonelID", personelId);
                    cmd.Parameters.AddWithValue("@Tarih", bugun);
                }

                cmd.Parameters.AddWithValue("@Giris", giris);
                cmd.Parameters.AddWithValue("@Cikis", cikis);

                cmd.ExecuteNonQuery();
            }

            lblMesaj.ForeColor = System.Drawing.Color.Green;
            lblMesaj.Text = kayitId.HasValue ? "Kayıt başarıyla güncellendi." : "Kayıt başarıyla eklendi.";

            TemizleForm();
            btnIptal.Visible = false;
            btnKaydet.Text = "Kaydet / Güncelle";

            // Filtre alanlarını temizle
            ddlFiltrePersonel.SelectedIndex = 0;
            txtFiltreBaslangic.Text = "";
            txtFiltreBitis.Text = "";

            DoldurKayitlar(true);
        }

        private int? KayitVarMi(int personelId, DateTime tarih)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT KayitID FROM PersonelGirisCikis 
                               WHERE PersonelID = @PersonelID AND Tarih = @Tarih";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PersonelID", personelId);
                cmd.Parameters.AddWithValue("@Tarih", tarih);
                conn.Open();
                object sonuc = cmd.ExecuteScalar();

                return sonuc != null ? Convert.ToInt32(sonuc) : (int?)null;
            }
        }

        private void DoldurKayitlar(bool sadeceBugun)
        {
            int filtrePersonelId = 0;
            DateTime? filtreBaslangic = null;
            DateTime? filtreBitis = null;

            if (sadeceBugun)
            {
                filtreBaslangic = DateTime.Today;
                filtreBitis = DateTime.Today;
            }
            else
            {
                int.TryParse(ddlFiltrePersonel.SelectedValue, out filtrePersonelId);

                if (DateTime.TryParse(txtFiltreBaslangic.Text, out DateTime baslangic))
                    filtreBaslangic = baslangic;

                if (DateTime.TryParse(txtFiltreBitis.Text, out DateTime bitis))
                    filtreBitis = bitis;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
SELECT
    p.PersonelID,
    pgc.KayitID,
    pgc.Tarih,
    p.Ad + ' ' + p.Soyad AS AdSoyad,
    pgc.GirisSaati,
    pgc.CikisSaati,
    CASE
        WHEN izin.PersonelID IS NOT NULL THEN 5
        WHEN pgc.KayitID IS NULL THEN 4
        WHEN pgc.GirisSaati > '09:00' AND DATEDIFF(MINUTE, pgc.GirisSaati, pgc.CikisSaati) < 480 THEN 1
        WHEN pgc.GirisSaati > '09:00' THEN 1
        WHEN pgc.CikisSaati < '17:00' THEN 2
        ELSE 3
    END AS DurumID,
    CASE
        WHEN izin.PersonelID IS NOT NULL THEN N'İZİNLİ'
        WHEN pgc.KayitID IS NULL THEN N'Gelmedi'
        WHEN pgc.GirisSaati > '09:00' AND DATEDIFF(MINUTE, pgc.GirisSaati, pgc.CikisSaati) < 480 THEN N'Geç Geldi ve Eksik Çalıştı'
        WHEN pgc.GirisSaati > '09:00' THEN N'Geç Geldi'
        WHEN pgc.CikisSaati < '17:00' THEN N'Erken Çıktı'
        ELSE N'Zamanında'
    END AS Durum,
    CASE
        WHEN pgc.GirisSaati > '09:00' THEN DATEDIFF(MINUTE, '09:00', pgc.GirisSaati)
        ELSE 0
    END AS GecikmeDakika,
    DATEDIFF(MINUTE, pgc.GirisSaati, pgc.CikisSaati) AS CalismaDakika
FROM PersonelBilgileri p
LEFT JOIN PersonelGirisCikis pgc ON p.PersonelID = pgc.PersonelID
LEFT JOIN Izinler izin ON izin.PersonelID = p.PersonelID
    AND pgc.Tarih BETWEEN izin.IzinBaslangic AND izin.IzinBitis
WHERE
    (@PersonelID = 0 OR p.PersonelID = @PersonelID)
    AND (@Baslangic IS NULL OR pgc.Tarih >= @Baslangic)
    AND (@Bitis IS NULL OR pgc.Tarih <= @Bitis)
ORDER BY pgc.Tarih DESC, p.Ad, p.Soyad
";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PersonelID", filtrePersonelId);
                cmd.Parameters.AddWithValue("@Baslangic", (object)filtreBaslangic ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Bitis", (object)filtreBitis ?? DBNull.Value);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Yeni sütunlar ekle ve formatla
                if (!dt.Columns.Contains("CalismaSuresi"))
                    dt.Columns.Add("CalismaSuresi", typeof(string));
                if (!dt.Columns.Contains("GirisSaatiFormatted"))
                    dt.Columns.Add("GirisSaatiFormatted", typeof(string));
                if (!dt.Columns.Contains("CikisSaatiFormatted"))
                    dt.Columns.Add("CikisSaatiFormatted", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    int calismaDakika = row["CalismaDakika"] != DBNull.Value ? Convert.ToInt32(row["CalismaDakika"]) : 0;
                    int saat = calismaDakika / 60;
                    int dakika = calismaDakika % 60;
                    row["CalismaSuresi"] = $"{saat} saat {dakika} dakika";

                    row["GirisSaatiFormatted"] = row["GirisSaati"] != DBNull.Value
                        ? ((TimeSpan)row["GirisSaati"]).ToString(@"hh\:mm")
                        : "-";

                    row["CikisSaatiFormatted"] = row["CikisSaati"] != DBNull.Value
                        ? ((TimeSpan)row["CikisSaati"]).ToString(@"hh\:mm")
                        : "-";
                }

                gvKayitlar.DataSource = dt;
                gvKayitlar.DataBind();
            }
        }

        protected void btnBugun_Click(object sender, EventArgs e)
        {
            ddlFiltrePersonel.SelectedIndex = 0;
            txtFiltreBaslangic.Text = "";
            txtFiltreBitis.Text = "";

            DoldurKayitlar(true); // sadece bugünün kayıtları
        }

        protected void btnTumKayitlar_Click(object sender, EventArgs e)
        {
            ddlFiltrePersonel.SelectedIndex = 0;
            txtFiltreBaslangic.Text = "";
            txtFiltreBitis.Text = "";

            DoldurKayitlar(false); // tüm kayıtlar
        }

        protected void btnFiltrele_Click(object sender, EventArgs e)
        {
            DoldurKayitlar(false); // filtreye göre listele
        }

        protected void gvKayitlar_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblDurum = (Label)e.Row.FindControl("lblDurum");
                int durumId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "DurumID"));

                if (lblDurum != null)
                {
                    switch (durumId)
                    {
                        case 1:
                            lblDurum.ForeColor = System.Drawing.Color.Red;     // Geç Geldi
                            break;
                        case 2:
                            lblDurum.ForeColor = System.Drawing.Color.Orange;  // Erken Çıktı
                            break;
                        case 4:
                            lblDurum.ForeColor = System.Drawing.Color.Gray;    // Gelmedi
                            break;
                        case 5:
                            lblDurum.ForeColor = System.Drawing.Color.Blue;    // İzinli
                            break;
                        default:
                            lblDurum.ForeColor = System.Drawing.Color.Green;   // Zamanında
                            break;
                    }
                }
            }
        }

        protected void gvKayitlar_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvKayitlar.EditIndex = e.NewEditIndex;
            DoldurKayitlar(false);
            btnIptal.Visible = true;
            btnKaydet.Text = "Güncelle";
        }

        protected void gvKayitlar_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvKayitlar.EditIndex = -1;
            DoldurKayitlar(false);
            btnIptal.Visible = false;
            btnKaydet.Text = "Kaydet / Güncelle";
            TemizleForm();
        }

        protected void gvKayitlar_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvKayitlar.Rows[e.RowIndex];
            int kayitId = Convert.ToInt32(gvKayitlar.DataKeys[e.RowIndex].Value);

            TextBox txtGiris = (TextBox)row.FindControl("txtEditGiris");
            TextBox txtCikis = (TextBox)row.FindControl("txtEditCikis");

            if (!TimeSpan.TryParse(txtGiris.Text.Trim(), out TimeSpan giris) ||
                !TimeSpan.TryParse(txtCikis.Text.Trim(), out TimeSpan cikis))
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Saatleri HH:mm formatında girin.";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"UPDATE PersonelGirisCikis SET GirisSaati = @Giris, CikisSaati = @Cikis WHERE KayitID = @ID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Giris", giris);
                cmd.Parameters.AddWithValue("@Cikis", cikis);
                cmd.Parameters.AddWithValue("@ID", kayitId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            gvKayitlar.EditIndex = -1;
            DoldurKayitlar(false);

            lblMesaj.ForeColor = System.Drawing.Color.Green;
            lblMesaj.Text = "Kayıt başarıyla güncellendi.";

            btnIptal.Visible = false;
            btnKaydet.Text = "Kaydet / Güncelle";
            TemizleForm();
        }

        protected void gvKayitlar_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int kayitId = Convert.ToInt32(gvKayitlar.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "DELETE FROM PersonelGirisCikis WHERE KayitID = @ID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", kayitId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            DoldurKayitlar(false);

            lblMesaj.ForeColor = System.Drawing.Color.Green;
            lblMesaj.Text = "Kayıt başarıyla silindi.";
        }

        protected void gvKayitlar_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvKayitlar.PageIndex = e.NewPageIndex;
            DoldurKayitlar(false);
        }

        private void TemizleForm()
        {
            ddlPersonel.SelectedIndex = 0;
            txtGiris.Text = "";
            txtCikis.Text = "";
            lblMesaj.Text = "";
        }

        protected void btnIptal_Click(object sender, EventArgs e)
        {
            TemizleForm();
            btnIptal.Visible = false;
            btnKaydet.Text = "Kaydet / Güncelle";
            gvKayitlar.EditIndex = -1;
            DoldurKayitlar(false);
        }
    }
}