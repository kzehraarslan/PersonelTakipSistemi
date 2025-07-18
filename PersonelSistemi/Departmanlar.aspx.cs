using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web;

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
            if (!AlanlarGecerliMi()) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "INSERT INTO Departmanlar (DepartmanAdi, CalismaSaatiBaslangic, CalismaSaatiBitis) VALUES (@Adi, @Baslangic, @Bitis)";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Adi", txtDepartmanAdi.Text.Trim());
                cmd.Parameters.AddWithValue("@Baslangic", TimeSpan.Parse(txtCalismaSaatiBaslangic.Text.Trim()));
                cmd.Parameters.AddWithValue("@Bitis", TimeSpan.Parse(txtCalismaSaatiBitis.Text.Trim()));

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMesaj.ForeColor = System.Drawing.Color.Green;
            lblMesaj.Text = "Departman başarıyla eklendi.";
            Temizle();
            DoldurDepartmanlar();
        }

        protected void gvDepartman_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvDepartman.SelectedRow;
            int id = Convert.ToInt32(gvDepartman.DataKeys[row.RowIndex].Value);
            ViewState["DepartmanID"] = id;


            txtDepartmanAdi.Text = HttpUtility.HtmlDecode(row.Cells[1].Text);
            txtCalismaSaatiBaslangic.Text = row.Cells[2].Text;
            txtCalismaSaatiBitis.Text = row.Cells[3].Text;

            btnEkle.Visible = false;
            btnGuncelle.Visible = true;
            btnIptal.Visible = true;

            lblMesaj.ForeColor = System.Drawing.Color.Blue;
            lblMesaj.Text = "Düzenleme modundasınız.";
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (ViewState["DepartmanID"] == null)
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Güncellenecek departman seçilmedi.";
                return;
            }

            if (!AlanlarGecerliMi()) return;

            int id = Convert.ToInt32(ViewState["DepartmanID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "UPDATE Departmanlar SET DepartmanAdi=@Adi, CalismaSaatiBaslangic=@Baslangic, CalismaSaatiBitis=@Bitis WHERE DepartmanID=@ID";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Adi", txtDepartmanAdi.Text.Trim());
                cmd.Parameters.AddWithValue("@Baslangic", TimeSpan.Parse(txtCalismaSaatiBaslangic.Text.Trim()));
                cmd.Parameters.AddWithValue("@Bitis", TimeSpan.Parse(txtCalismaSaatiBitis.Text.Trim()));
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMesaj.ForeColor = System.Drawing.Color.Green;
            lblMesaj.Text = "Departman başarıyla güncellendi.";
            Temizle();
            DoldurDepartmanlar();
        }

        protected void gvDepartman_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string rol = (Session["Rol"] ?? "").ToString();
            if (!string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Silme işlemi için yetkiniz yok.";
                return;
            }

            int id = Convert.ToInt32(gvDepartman.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "DELETE FROM Departmanlar WHERE DepartmanID=@ID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMesaj.ForeColor = System.Drawing.Color.Green;
            lblMesaj.Text = "Departman başarıyla silindi.";
            DoldurDepartmanlar();
            Temizle();
        }

        private bool AlanlarGecerliMi()
        {
            if (string.IsNullOrWhiteSpace(txtDepartmanAdi.Text))
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Departman adı boş olamaz.";
                return false;
            }

            if (!TimeSpan.TryParse(txtCalismaSaatiBaslangic.Text.Trim(), out _))
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Başlangıç saati HH:mm formatında olmalı.";
                return false;
            }

            if (!TimeSpan.TryParse(txtCalismaSaatiBitis.Text.Trim(), out _))
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Bitiş saati HH:mm formatında olmalı.";
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
    }
}