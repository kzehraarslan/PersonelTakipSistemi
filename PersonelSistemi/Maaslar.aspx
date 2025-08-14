<%@ Page Title="Maaş Hesaplama" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Maaslar.aspx.cs" Inherits="PersonelSistemi.Maaslar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
    <title>Maaş Hesaplama</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h3>Maaş Hesaplama</h3>
        <hr />

        <asp:Label ID="lblMesaj" runat="server" CssClass="text-danger d-block mb-3" />

        <div class="row g-3 mb-4">
            <div class="col-md-3">
                <asp:DropDownList ID="ddlPersonel" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtBaslangic" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtBitis" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtSaatlikUcret" runat="server" CssClass="form-control" Placeholder="Saatlik Ücret" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnHesapla" runat="server" Text="Hesapla ve Kaydet" CssClass="btn btn-primary w-100" OnClick="btnHesapla_Click" />
                <asp:Button ID="btnGuncelle" runat="server" Text="Güncelle" CssClass="btn btn-success w-100 mt-2" OnClick="btnGuncelle_Click" Visible="false" />
                <asp:Button ID="btnIptal" runat="server" Text="İptal" CssClass="btn btn-secondary w-100 mt-2" OnClick="btnIptal_Click" Visible="false" />
            </div>
        </div>

        <div class="alert alert-info" role="alert">
            <asp:Label ID="lblSonuc" runat="server" Text="Henüz hesaplama yapılmadı." />
        </div>

        <hr />
        <h5>Maaş Kayıtları</h5>
        <asp:GridView ID="gvMaaslar" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" AllowPaging="true" PageSize="10"
    OnPageIndexChanging="gvMaaslar_PageIndexChanging"
    OnRowCommand="gvMaaslar_RowCommand"
    DataKeyNames="MaasID">

    <Columns>
        <asp:BoundField DataField="AdSoyad" HeaderText="Personel" ReadOnly="true" />
        <asp:BoundField DataField="BaslangicTarihiFormatted" HeaderText="Başlangıç" ReadOnly="true" />
        <asp:BoundField DataField="BitisTarihiFormatted" HeaderText="Bitiş" ReadOnly="true" />
        <asp:BoundField DataField="ToplamSure" HeaderText="Çalışma Süresi" ReadOnly="true" />
        <asp:BoundField DataField="SaatlikUcret" HeaderText="Saatlik Ücret" DataFormatString="{0:C}" ReadOnly="true" />
        <asp:BoundField DataField="ToplamMaas" HeaderText="Toplam Maaş" DataFormatString="{0:C}" ReadOnly="true" />
        <asp:BoundField DataField="OlusturmaTarihiFormatted" HeaderText="Kayıt Tarihi" ReadOnly="true" />

        <asp:TemplateField HeaderText="İşlemler">
            <ItemTemplate>
                <asp:Button ID="btnSec" runat="server" Text="Seç" CssClass="btn btn-sm btn-outline-primary mx-1"
                    CommandName="Sec" CommandArgument="<%# Container.DataItemIndex %>" />

                <asp:Button ID="btnSil" runat="server" Text="Sil" CssClass="btn btn-sm btn-outline-danger"
                    CommandName="Sil" CommandArgument="<%# Container.DataItemIndex %>" OnClientClick="return confirm('Bu kaydı silmek istediğinize emin misiniz?');" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
    </div>
</asp:Content>