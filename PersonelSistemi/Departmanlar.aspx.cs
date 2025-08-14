using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace PersonelSistemi
{
    public partial class Departmanlar : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                DoldurDepartmanlar();

                string rol = Session["Rol"].ToString().Trim();
                if (!string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    formPanel.Visible = false; // Kullanıcı kayıt alanını görmez
                }
            }
        }

        private void DoldurDepartmanlar()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT DepartmanID, DepartmanAdi, CalismaSaatiBaslangic, CalismaSaatiBitis FROM Departmanlar";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvDepartman.DataSource = dt;
                gvDepartman.DataBind();
            }
        }

        protected void btnEkle_Click(object sender, EventArgs e)
        {
            string rol = (Session["Rol"] ?? "").ToString().Trim();
            if (!string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                Mesaj("Ekleme yetkiniz yok.", "red");
                return;
            }

            if (!AlanlarGecerliMi()) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "INSERT INTO Departmanlar (DepartmanAdi, CalismaSaatiBaslangic, CalismaSaatiBitis) VALUES (@Adi, @Baslangic, @Bitis)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SetParams(cmd);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Mesaj("Departman başarıyla eklendi.", "green");
            Temizle();
            DoldurDepartmanlar();
        }

        protected void gvDepartman_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rol = (Session["Rol"] ?? "").ToString().Trim();
            if (!string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                Mesaj("Güncelleme yetkiniz yok.", "red");
                return;
            }

            GridViewRow row = gvDepartman.SelectedRow;
            ViewState["DepartmanID"] = gvDepartman.DataKeys[row.RowIndex].Value;

            txtDepartmanAdi.Text = HttpUtility.HtmlDecode(row.Cells[3].Text);
            txtCalismaSaatiBaslangic.Text = HttpUtility.HtmlDecode(row.Cells[4].Text);
            txtCalismaSaatiBitis.Text = HttpUtility.HtmlDecode(row.Cells[5].Text);

            btnEkle.Visible = false;
            btnGuncelle.Visible = true;
            btnIptal.Visible = true;

            Mesaj("Düzenleme modundasınız.", "blue");
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (ViewState["DepartmanID"] == null)
            {
                Mesaj("Güncellenecek departman seçilmedi.", "red");
                return;
            }

            if (!AlanlarGecerliMi()) return;

            int id = Convert.ToInt32(ViewState["DepartmanID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "UPDATE Departmanlar SET DepartmanAdi=@Adi, CalismaSaatiBaslangic=@Baslangic, CalismaSaatiBitis=@Bitis WHERE DepartmanID=@ID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SetParams(cmd);
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Mesaj("Departman başarıyla güncellendi.", "green");
            Temizle();
            DoldurDepartmanlar();
        }

        protected void gvDepartman_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string rol = (Session["Rol"] ?? "").ToString().Trim();
            if (!string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                Mesaj("Silme işlemi için yetkiniz yok.", "red");
                return;
            }

            int id = Convert.ToInt32(gvDepartman.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Departmanlar WHERE DepartmanID=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Mesaj("Departman başarıyla silindi.", "green");
            Temizle();
            DoldurDepartmanlar();
        }

        protected void gvDepartman_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string rol = (Session["Rol"] ?? "").ToString().Trim();

            if (e.Row.RowType == DataControlRowType.DataRow && !string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                if (e.Row.Cells.Count > 1)
                {
                    e.Row.Cells[0].Controls.Clear();
                    e.Row.Cells[1].Controls.Clear();
                }
            }
        }

        private void SetParams(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Adi", txtDepartmanAdi.Text.Trim());
            cmd.Parameters.AddWithValue("@Baslangic", TimeSpan.Parse(txtCalismaSaatiBaslangic.Text.Trim()));
            cmd.Parameters.AddWithValue("@Bitis", TimeSpan.Parse(txtCalismaSaatiBitis.Text.Trim()));
        }

        private bool AlanlarGecerliMi()
        {
            if (string.IsNullOrWhiteSpace(txtDepartmanAdi.Text))
            {
                Mesaj("Departman adı boş olamaz.", "red");
                return false;
            }

            if (!TimeSpan.TryParse(txtCalismaSaatiBaslangic.Text.Trim(), out _))
            {
                Mesaj("Başlangıç saati HH:mm formatında olmalı.", "red");
                return false;
            }

            if (!TimeSpan.TryParse(txtCalismaSaatiBitis.Text.Trim(), out _))
            {
                Mesaj("Bitiş saati HH:mm formatında olmalı.", "red");
                return false;
            }

            return true;
        }

        protected void btnIptal_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void Temizle()
        {
            txtDepartmanAdi.Text = "";
            txtCalismaSaatiBaslangic.Text = "";
            txtCalismaSaatiBitis.Text = "";

            btnEkle.Visible = true;
            btnGuncelle.Visible = false;
            btnIptal.Visible = false;
            lblMesaj.Text = "";
            ViewState["DepartmanID"] = null;
        }

        private void Mesaj(string text, string color)
        {
            lblMesaj.Text = text;
            lblMesaj.ForeColor = System.Drawing.Color.FromName(color);
        }
    }
}