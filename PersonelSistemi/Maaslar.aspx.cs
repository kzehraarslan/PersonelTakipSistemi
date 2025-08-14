using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace PersonelSistemi
{
    public partial class Maaslar : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        // Güncelleme için kayıt seçilen ID
        private int SecilenMaasID
        {
            get => ViewState["SecilenMaasID"] != null ? (int)ViewState["SecilenMaasID"] : 0;
            set => ViewState["SecilenMaasID"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DoldurPersonel();
                DoldurMaasKayitlari();
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

                ddlPersonel.Items.Insert(0, new ListItem("-- Personel Seçin --", "0"));
            }
        }

        protected void btnHesapla_Click(object sender, EventArgs e)
        {
            lblMesaj.Text = "";

            if (ddlPersonel.SelectedValue == "0")
            {
                lblMesaj.Text = "Lütfen personel seçin.";
                return;
            }

            if (!DateTime.TryParse(txtBaslangic.Text, out DateTime baslangic) ||
                !DateTime.TryParse(txtBitis.Text, out DateTime bitis))
            {
                lblMesaj.Text = "Tarih aralığını seçin.";
                return;
            }

            if (!decimal.TryParse(txtSaatlikUcret.Text, out decimal saatlikUcret))
            {
                lblMesaj.Text = "Saatlik ücreti doğru girin.";
                return;
            }

            if (bitis < baslangic)
            {
                lblMesaj.Text = "Bitiş tarihi başlangıç tarihinden küçük olamaz.";
                return;
            }

            int personelId = int.Parse(ddlPersonel.SelectedValue);
            int toplamDakika = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT SUM(DATEDIFF(MINUTE, GirisSaati, CikisSaati)) AS ToplamDakika
                               FROM PersonelGirisCikis
                               WHERE PersonelID = @PersonelID
                               AND Tarih BETWEEN @Baslangic AND @Bitis";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PersonelID", personelId);
                cmd.Parameters.AddWithValue("@Baslangic", baslangic);
                cmd.Parameters.AddWithValue("@Bitis", bitis);

                conn.Open();
                object sonuc = cmd.ExecuteScalar();

                if (sonuc != DBNull.Value && sonuc != null)
                {
                    toplamDakika = Convert.ToInt32(sonuc);
                }
            }

            if (toplamDakika == 0)
            {
                lblSonuc.Text = "Belirtilen tarih aralığında çalışma kaydı yok.";
                return;
            }

            int saat = toplamDakika / 60;
            int dakika = toplamDakika % 60;
            decimal toplamMaas = ((decimal)toplamDakika / 60) * saatlikUcret;

            lblSonuc.Text = $"Çalışma Süresi: <b>{saat} saat {dakika} dakika</b><br/>" +
                            $"Toplam Maaş: <b>{toplamMaas:C}</b>";

            // Yeni kayıt olarak ekle
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sqlInsert = @"INSERT INTO Maaslar (PersonelID, BaslangicTarihi, BitisTarihi, ToplamDakika, SaatlikUcret, ToplamMaas, OlusturmaTarihi)
                                     VALUES (@PersonelID, @Baslangic, @Bitis, @ToplamDakika, @SaatlikUcret, @ToplamMaas, @OlusturmaTarihi)";
                SqlCommand cmd = new SqlCommand(sqlInsert, conn);
                cmd.Parameters.AddWithValue("@PersonelID", personelId);
                cmd.Parameters.AddWithValue("@Baslangic", baslangic);
                cmd.Parameters.AddWithValue("@Bitis", bitis);
                cmd.Parameters.AddWithValue("@ToplamDakika", toplamDakika);
                cmd.Parameters.AddWithValue("@SaatlikUcret", saatlikUcret);
                cmd.Parameters.AddWithValue("@ToplamMaas", toplamMaas);
                cmd.Parameters.AddWithValue("@OlusturmaTarihi", DateTime.Now);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TemizleForm();
            DoldurMaasKayitlari();
        }

        private void DoldurMaasKayitlari()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT m.MaasID, m.PersonelID, p.Ad + ' ' + p.Soyad AS AdSoyad,
                               m.BaslangicTarihi, m.BitisTarihi, m.ToplamDakika, m.SaatlikUcret, m.ToplamMaas, m.OlusturmaTarihi
                               FROM Maaslar m
                               INNER JOIN PersonelBilgileri p ON m.PersonelID = p.PersonelID
                               ORDER BY m.OlusturmaTarihi DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("BaslangicTarihiFormatted", typeof(string));
                dt.Columns.Add("BitisTarihiFormatted", typeof(string));
                dt.Columns.Add("OlusturmaTarihiFormatted", typeof(string));
                dt.Columns.Add("ToplamSure", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    DateTime baslangic = Convert.ToDateTime(row["BaslangicTarihi"]);
                    DateTime bitis = Convert.ToDateTime(row["BitisTarihi"]);
                    DateTime olusturma = Convert.ToDateTime(row["OlusturmaTarihi"]);
                    int toplamDakika = Convert.ToInt32(row["ToplamDakika"]);

                    row["BaslangicTarihiFormatted"] = baslangic.ToString("dd.MM.yyyy");
                    row["BitisTarihiFormatted"] = bitis.ToString("dd.MM.yyyy");
                    row["OlusturmaTarihiFormatted"] = olusturma.ToString("dd.MM.yyyy HH:mm");
                    row["ToplamSure"] = $"{toplamDakika / 60} saat {toplamDakika % 60} dk";
                }

                gvMaaslar.DataSource = dt;
                gvMaaslar.DataBind();
            }
        }

        protected void gvMaaslar_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMaaslar.PageIndex = e.NewPageIndex;
            DoldurMaasKayitlari();
        }

        protected void gvMaaslar_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int maasId = Convert.ToInt32(gvMaaslar.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "DELETE FROM Maaslar WHERE MaasID = @MaasID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaasID", maasId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMesaj.Text = "Kayıt silindi.";
            DoldurMaasKayitlari();
        }

        // GridView Satır Komutları (Seç butonu için)
        protected void gvMaaslar_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int maasId = Convert.ToInt32(gvMaaslar.DataKeys[index].Value);

            if (e.CommandName == "Sec")
            {
                SecilenMaasID = maasId;
                DoldurForm(maasId);
            }
            else if (e.CommandName == "Sil")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string sql = "DELETE FROM Maaslar WHERE MaasID = @MaasID";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaasID", maasId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMesaj.Text = "Kayıt silindi.";
                DoldurMaasKayitlari();
            }
        }

        private void DoldurForm(int maasId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT PersonelID, BaslangicTarihi, BitisTarihi, SaatlikUcret
                               FROM Maaslar WHERE MaasID = @MaasID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaasID", maasId);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    ddlPersonel.SelectedValue = dr["PersonelID"].ToString();
                    txtBaslangic.Text = Convert.ToDateTime(dr["BaslangicTarihi"]).ToString("yyyy-MM-dd");
                    txtBitis.Text = Convert.ToDateTime(dr["BitisTarihi"]).ToString("yyyy-MM-dd");
                    txtSaatlikUcret.Text = dr["SaatlikUcret"].ToString();

                    btnHesapla.Visible = false;
                    btnGuncelle.Visible = true;
                    btnIptal.Visible = true;
                }
                dr.Close();
            }
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            lblMesaj.Text = "";

            if (SecilenMaasID == 0)
            {
                lblMesaj.Text = "Güncellenecek kayıt seçilmedi.";
                return;
            }

            if (ddlPersonel.SelectedValue == "0")
            {
                lblMesaj.Text = "Lütfen personel seçin.";
                return;
            }

            if (!DateTime.TryParse(txtBaslangic.Text, out DateTime baslangic) ||
                !DateTime.TryParse(txtBitis.Text, out DateTime bitis))
            {
                lblMesaj.Text = "Tarih aralığını seçin.";
                return;
            }

            if (!decimal.TryParse(txtSaatlikUcret.Text, out decimal saatlikUcret))
            {
                lblMesaj.Text = "Saatlik ücreti doğru girin.";
                return;
            }

            if (bitis < baslangic)
            {
                lblMesaj.Text = "Bitiş tarihi başlangıç tarihinden küçük olamaz.";
                return;
            }

            int personelId = int.Parse(ddlPersonel.SelectedValue);

            int toplamDakika = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT SUM(DATEDIFF(MINUTE, GirisSaati, CikisSaati)) AS ToplamDakika
                               FROM PersonelGirisCikis
                               WHERE PersonelID = @PersonelID
                               AND Tarih BETWEEN @Baslangic AND @Bitis";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PersonelID", personelId);
                cmd.Parameters.AddWithValue("@Baslangic", baslangic);
                cmd.Parameters.AddWithValue("@Bitis", bitis);

                conn.Open();
                object sonuc = cmd.ExecuteScalar();

                if (sonuc != DBNull.Value && sonuc != null)
                {
                    toplamDakika = Convert.ToInt32(sonuc);
                }
            }

            if (toplamDakika == 0)
            {
                lblSonuc.Text = "Belirtilen tarih aralığında çalışma kaydı yok.";
                return;
            }

            int saat = toplamDakika / 60;
            int dakika = toplamDakika % 60;
            decimal toplamMaas = ((decimal)toplamDakika / 60) * saatlikUcret;

            lblSonuc.Text = $"Çalışma Süresi: <b>{saat} saat {dakika} dakika</b><br/>" +
                            $"Toplam Maaş: <b>{toplamMaas:C}</b>";

            // Güncelleme sorgusu
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sqlUpdate = @"UPDATE Maaslar 
                                     SET PersonelID = @PersonelID,
                                         BaslangicTarihi = @Baslangic,
                                         BitisTarihi = @Bitis,
                                         ToplamDakika = @ToplamDakika,
                                         SaatlikUcret = @SaatlikUcret,
                                         ToplamMaas = @ToplamMaas
                                     WHERE MaasID = @MaasID";

                SqlCommand cmd = new SqlCommand(sqlUpdate, conn);
                cmd.Parameters.AddWithValue("@PersonelID", personelId);
                cmd.Parameters.AddWithValue("@Baslangic", baslangic);
                cmd.Parameters.AddWithValue("@Bitis", bitis);
                cmd.Parameters.AddWithValue("@ToplamDakika", toplamDakika);
                cmd.Parameters.AddWithValue("@SaatlikUcret", saatlikUcret);
                cmd.Parameters.AddWithValue("@ToplamMaas", toplamMaas);
                cmd.Parameters.AddWithValue("@MaasID", SecilenMaasID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMesaj.Text = "Kayıt başarıyla güncellendi.";
            TemizleForm();
            DoldurMaasKayitlari();
        }

        protected void btnIptal_Click(object sender, EventArgs e)
        {
            TemizleForm();
        }

        private void TemizleForm()
        {
            ddlPersonel.SelectedIndex = 0;
            txtBaslangic.Text = "";
            txtBitis.Text = "";
            txtSaatlikUcret.Text = "";
            lblSonuc.Text = "Henüz hesaplama yapılmadı.";
            lblMesaj.Text = "";
            SecilenMaasID = 0;

            btnHesapla.Visible = true;
            btnGuncelle.Visible = false;
            btnIptal.Visible = false;
        }
    }
}