using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace PersonelSistemi
{
    public partial class PersonelSistemi : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string rol = Session["Rol"].ToString().Trim();

            if (!IsPostBack)
            {
                DoldurDepartmanlar();
                DoldurUnvanlar();
                DoldurPersonelListesi();
                btnGuncelle.Visible = false;

                // Sadece Admin'e formu göster
                pnlForm.Visible = string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase);
            }
        }

        private void DoldurDepartmanlar()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT DepartmanID, DepartmanAdi FROM Departmanlar", conn);
                conn.Open();
                ddlDepartman.DataSource = cmd.ExecuteReader();
                ddlDepartman.DataTextField = "DepartmanAdi";
                ddlDepartman.DataValueField = "DepartmanID";
                ddlDepartman.DataBind();
            }
            ddlDepartman.Items.Insert(0, new ListItem("--Departman Seç--", "0"));
        }

        private void DoldurUnvanlar()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT UnvanID, UnvanAdi FROM Unvanlar", conn);
                conn.Open();
                ddlUnvan.DataSource = cmd.ExecuteReader();
                ddlUnvan.DataTextField = "UnvanAdi";
                ddlUnvan.DataValueField = "UnvanID";
                ddlUnvan.DataBind();
            }
            ddlUnvan.Items.Insert(0, new ListItem("--Unvan Seç--", "0"));
        }

        private void DoldurPersonelListesi()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT p.PersonelID, p.Ad, p.Soyad, p.Telefon, p.Adres, p.Email, p.Maas,
                           d.DepartmanAdi, u.UnvanAdi
                    FROM PersonelBilgileri p
                    LEFT JOIN Departmanlar d ON p.DepartmanID = d.DepartmanID
                    LEFT JOIN Unvanlar u ON p.UnvanID = u.UnvanID";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvPersonel.DataSource = dt;
                gvPersonel.DataBind();
            }
        }

        protected void btnEkle_Click(object sender, EventArgs e)
        {
            if (!RolKontrolu("Admin")) return;

            if (!AlanlarGecerliMi()) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string insertSql = @"INSERT INTO PersonelBilgileri 
                                    (Ad, Soyad, Telefon, Adres, Email, Maas, DepartmanID, UnvanID)
                                    VALUES (@Ad, @Soyad, @Telefon, @Adres, @Email, @Maas, @DepartmanID, @UnvanID)";

                SqlCommand cmd = new SqlCommand(insertSql, conn);
                SetParameters(cmd);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMesaj.ForeColor = System.Drawing.Color.Green;
            lblMesaj.Text = "Personel başarıyla eklendi.";

            Temizle();
            DoldurPersonelListesi();
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (!RolKontrolu("Admin")) return;

            if (ViewState["PersonelID"] == null)
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Güncellenecek personel seçilmedi.";
                return;
            }

            if (!AlanlarGecerliMi()) return;

            int id = Convert.ToInt32(ViewState["PersonelID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string updateSql = @"UPDATE PersonelBilgileri SET
                                    Ad=@Ad, Soyad=@Soyad, Telefon=@Telefon, Adres=@Adres, Email=@Email, Maas=@Maas,
                                    DepartmanID=@DepartmanID, UnvanID=@UnvanID
                                    WHERE PersonelID=@PersonelID";

                SqlCommand cmd = new SqlCommand(updateSql, conn);
                SetParameters(cmd);
                cmd.Parameters.AddWithValue("@PersonelID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMesaj.ForeColor = System.Drawing.Color.Green;
            lblMesaj.Text = "Personel başarıyla güncellendi.";

            Temizle();
            DoldurPersonelListesi();
        }

        private bool AlanlarGecerliMi()
        {
            if (ddlDepartman.SelectedValue == "0" || ddlUnvan.SelectedValue == "0")
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Lütfen departman ve unvan seçiniz.";
                return false;
            }

            if (!decimal.TryParse(txtMaas.Text.Trim(), out _))
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Maaş geçerli bir sayı olmalı.";
                return false;
            }

            return true;
        }

        private void SetParameters(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Ad", txtAd.Text.Trim());
            cmd.Parameters.AddWithValue("@Soyad", txtSoyad.Text.Trim());
            cmd.Parameters.AddWithValue("@Telefon", txtTelefon.Text.Trim());
            cmd.Parameters.AddWithValue("@Adres", txtAdres.Text.Trim());
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@Maas", Convert.ToDecimal(txtMaas.Text.Trim()));
            cmd.Parameters.AddWithValue("@DepartmanID", Convert.ToInt32(ddlDepartman.SelectedValue));
            cmd.Parameters.AddWithValue("@UnvanID", Convert.ToInt32(ddlUnvan.SelectedValue));
        }

        protected void gvPersonel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!RolKontrolu("Admin")) return;

            GridViewRow row = gvPersonel.SelectedRow;
            ViewState["PersonelID"] = gvPersonel.DataKeys[row.RowIndex].Value;

            txtAd.Text = HttpUtility.HtmlDecode(row.Cells[3].Text);
            txtSoyad.Text = HttpUtility.HtmlDecode(row.Cells[4].Text);
            txtTelefon.Text = HttpUtility.HtmlDecode(row.Cells[5].Text);
            txtAdres.Text = HttpUtility.HtmlDecode(row.Cells[6].Text);
            txtEmail.Text = HttpUtility.HtmlDecode(row.Cells[7].Text);
            txtMaas.Text = HttpUtility.HtmlDecode(row.Cells[8].Text);

            ddlDepartman.ClearSelection();
            ddlUnvan.ClearSelection();

            string depText = HttpUtility.HtmlDecode(row.Cells[9].Text);
            string unvText = HttpUtility.HtmlDecode(row.Cells[10].Text);

            ListItem depItem = ddlDepartman.Items.FindByText(depText);
            if (depItem != null) ddlDepartman.SelectedValue = depItem.Value;

            ListItem unvItem = ddlUnvan.Items.FindByText(unvText);
            if (unvItem != null) ddlUnvan.SelectedValue = unvItem.Value;

            btnEkle.Visible = false;
            btnGuncelle.Visible = true;

            lblMesaj.ForeColor = System.Drawing.Color.Blue;
            lblMesaj.Text = "Düzenleme modundasınız, güncelleyin veya iptal edin.";
        }

        protected void gvPersonel_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (!RolKontrolu("Admin")) return;

            int id = Convert.ToInt32(gvPersonel.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM PersonelBilgileri WHERE PersonelID=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMesaj.ForeColor = System.Drawing.Color.Green;
            lblMesaj.Text = "Personel başarıyla silindi.";

            DoldurPersonelListesi();
            Temizle();
        }

        protected void gvPersonel_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string rol = (Session["Rol"] ?? "").ToString().Trim();

            if (e.Row.RowType == DataControlRowType.DataRow && !string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                // Silme ve Seç butonlarını kaldırıyoruz
                if (e.Row.Cells.Count > 1)
                {
                    e.Row.Cells[0].Controls.Clear(); // Select
                    e.Row.Cells[1].Controls.Clear(); // Delete
                }
            }
        }

        private void Temizle()
        {
            txtAd.Text = "";
            txtSoyad.Text = "";
            txtTelefon.Text = "";
            txtAdres.Text = "";
            txtEmail.Text = "";
            txtMaas.Text = "";
            ddlDepartman.SelectedIndex = 0;
            ddlUnvan.SelectedIndex = 0;
            btnEkle.Visible = true;
            btnGuncelle.Visible = false;
            ViewState["PersonelID"] = null;
            lblMesaj.Text = "";
        }

        private bool RolKontrolu(string rolGerekli)
        {
            string rol = (Session["Rol"] ?? "").ToString().Trim();
            if (!string.Equals(rol, rolGerekli, StringComparison.OrdinalIgnoreCase))
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = $"{rolGerekli} yetkisi gereklidir.";
                return false;
            }
            return true;
        }
    }
}