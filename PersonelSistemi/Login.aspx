<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PersonelSistemi.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Kullanıcı Girişi</title>

    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        body {
            background-color: #f8f9fa;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container">
        <div class="row justify-content-center align-items-center" style="height: 100vh;">
            <div class="col-md-4">
                <div class="card shadow p-4">
                    <h3 class="text-center mb-4">Giriş Yap</h3>

                    <asp:Label ID="lblMesaj" runat="server" CssClass="text-danger mb-2" />

                    <div class="mb-3">
                        <asp:TextBox ID="txtKullaniciAdi" runat="server" CssClass="form-control" Placeholder="Kullanıcı Adı" />
                    </div>

                    <div class="mb-3">
                        <asp:TextBox ID="txtSifre" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Şifre" />
                    </div>

                    <asp:Button ID="btnGiris" runat="server" Text="Giriş Yap" CssClass="btn btn-primary w-100" OnClick="btnGiris_Click" />
                </div>
            </div>
        </div>
    </form>

    <!-- Bootstrap JS -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>