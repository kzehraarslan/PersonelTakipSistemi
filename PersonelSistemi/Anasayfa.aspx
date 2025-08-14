<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AnaSayfa.aspx.cs" Inherits="PersonelSistemi.AnaSayfa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Ana Sayfa</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-4">
        <h2>Hoş geldiniz, <asp:Label ID="lblKullanici" runat="server" CssClass="text-primary fw-bold" /></h2>
        <p class="mb-4">Bugünün Tarihi: <asp:Label ID="lblTarih" runat="server" /></p>

        <div class="row g-3">
            <div class="col-md-3">
                <div class="card text-white bg-primary mb-3">
                    <div class="card-body">
                        <h5 class="card-title">Toplam Personel</h5>
                        <p class="card-text display-6" id="lblToplamPersonel"><asp:Label ID="lblToplamPersonel" runat="server" CssClass="fw-bold fs-2 text-white" /></p>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card text-white bg-success mb-3">
                    <div class="card-body">
                        <h5 class="card-title">Toplam Departman</h5>
                        <p class="card-text display-6" id="lblToplamDepartman"><asp:Label ID="lblToplamDepartman" runat="server" CssClass="fw-bold fs-2 text-white" /></p>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card text-white bg-warning mb-3">
                    <div class="card-body">
                        <h5 class="card-title">Ortalama Maaş</h5>
                        <p class="card-text display-6" id="lblOrtalamaMaas"><asp:Label ID="lblOrtalamaMaas" runat="server" CssClass="fw-bold fs-2 text-white" /></p>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card text-white bg-info mb-3">
                    <div class="card-body">
                        <h5 class="card-title">Şu Anki Saat</h5>
                        <p class="card-text display-6" id="lblSaat"><asp:Label ID="lblSaat" runat="server" CssClass="fw-bold fs-2 text-white" /></p>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>