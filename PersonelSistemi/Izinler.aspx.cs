using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace PersonelSistemi
{
    public partial class Izinler : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DoldurPersonel();
                DoldurIzinler();
            }
        }

        private void DoldurPersonel()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT PersonelID, Ad + ' ' + Soyad AS AdSoyad FROM PersonelBilgileri";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlPersonel.DataSource = dt;
                ddlPersonel.DataTextField = "AdSoyad";
                ddlPersonel.DataValueField = "PersonelID";
                ddlPersonel.DataBind();
            }

            ddlPersonel.Items.Insert(0, new ListItem("-- Personel Seç --", "0"));
        }

        private void DoldurIzinler()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT i.IzinID, i.IzinBaslangic, i.IzinBitis, i.IzinTipi, i.Aciklama,
                              p.Ad + ' ' + p.Soyad AS AdSoyad
                       FROM Izinler i
                       INNER JOIN PersonelBilgileri p ON i.PersonelID = p.PersonelID
                       ORDER BY i.IzinID DESC"; // en son kayıt en üstte

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvIzinler.DataSource = dt;
                gvIzinler.DataBind();
            }
        }

        protected void gvIzinler_SelectedIndexChanged(object sender, EventArgs e)
        {
            int izinId = Convert.ToInt32(gvIzinler.DataKeys[gvIzinler.SelectedIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT * FROM Izinler WHERE IzinID = @ID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", izinId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlPersonel.SelectedValue = reader["PersonelID"].ToString();
                    txtBaslangic.Text = Convert.ToDateTime(reader["IzinBaslangic"]).ToString("dd.MM.yyyy");
                    txtBitis.Text = Convert.ToDateTime(reader["IzinBitis"]).ToString("dd.MM.yyyy");
                    txtIzinTipi.Text = reader["IzinTipi"].ToString();
                    txtAciklama.Text = reader["Aciklama"].ToString();

                    ViewState["IzinID"] = izinId;
                    btnIptal.Visible = true;
                    btnKaydet.Text = "Güncelle";
                    lblMesaj.Text = "Düzenleme modundasınız. Güncelleyin veya iptal edin.";
                    lblMesaj.CssClass = "text-primary";
                }
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (ddlPersonel.SelectedValue == "0")
            {
                lblMesaj.Text = "Lütfen personel seçin.";
                return;
            }

            if (!DateTime.TryParseExact(txtBaslangic.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime baslangic) ||
                !DateTime.TryParseExact(txtBitis.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime bitis))
            {
                lblMesaj.Text = "Geçerli tarih girin.";
                return;
            }

            int personelId = int.Parse(ddlPersonel.SelectedValue);
            string izinTipi = txtIzinTipi.Text.Trim();
            string aciklama = txtAciklama.Text.Trim();

            int? izinId = ViewState["IzinID"] as int?;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd;

                if (izinId.HasValue)
                {
                    cmd = new SqlCommand(@"UPDATE Izinler SET PersonelID=@PersonelID, IzinBaslangic=@Baslangic,
                                           IzinBitis=@Bitis, IzinTipi=@Tipi, Aciklama=@Aciklama WHERE IzinID=@ID", conn);
                    cmd.Parameters.AddWithValue("@ID", izinId.Value);
                }
                else
                {
                    cmd = new SqlCommand(@"INSERT INTO Izinler (PersonelID, IzinBaslangic, IzinBitis, IzinTipi, Aciklama)
                                           VALUES (@PersonelID, @Baslangic, @Bitis, @Tipi, @Aciklama)", conn);
                }

                cmd.Parameters.AddWithValue("@PersonelID", personelId);
                cmd.Parameters.AddWithValue("@Baslangic", baslangic);
                cmd.Parameters.AddWithValue("@Bitis", bitis);
                cmd.Parameters.AddWithValue("@Tipi", izinTipi);
                cmd.Parameters.AddWithValue("@Aciklama", aciklama);
                cmd.ExecuteNonQuery();
            }

            lblMesaj.CssClass = "text-success";
            lblMesaj.Text = izinId.HasValue ? "İzin güncellendi." : "İzin eklendi.";

            TemizleForm();
            DoldurIzinler();
        }

        protected void gvIzinler_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int izinId = Convert.ToInt32(gvIzinler.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Izinler WHERE IzinID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", izinId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMesaj.CssClass = "text-success";
            lblMesaj.Text = "İzin silindi.";
            DoldurIzinler();
            TemizleForm();
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
            txtIzinTipi.Text = "";
            txtAciklama.Text = "";
            ViewState["IzinID"] = null;
            btnKaydet.Text = "Kaydet / Güncelle";
            btnIptal.Visible = false;
            lblMesaj.Text = "";
        }
    }
}