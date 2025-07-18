<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Izinler.aspx.cs" Inherits="PersonelSistemi.Izinler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>İzinler</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h3>Personel İzin Takibi</h3>

        <asp:Label ID="lblMesaj" runat="server" CssClass="text-danger" />

        <div class="row g-3">
            <div class="col-md-3">
                <asp:DropDownList ID="ddlPersonel" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtBaslangic" runat="server" CssClass="form-control" Placeholder="Başlangıç (gg.aa.yyyy)" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtBitis" runat="server" CssClass="form-control" Placeholder="Bitiş (gg.aa.yyyy)" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtIzinTipi" runat="server" CssClass="form-control" Placeholder="İzin Tipi" />
            </div>
        </div>

        <div class="row g-3 mt-2">
            <div class="col-md-12">
                <asp:TextBox ID="txtAciklama" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" Placeholder="Açıklama" />
            </div>
        </div>

        <div class="mt-3">
            <asp:Button ID="btnKaydet" runat="server" Text="Kaydet / Güncelle" CssClass="btn btn-success" OnClick="btnKaydet_Click" />
            <asp:Button ID="btnIptal" runat="server" Text="İptal" CssClass="btn btn-secondary ms-2" OnClick="btnIptal_Click" Visible="false" />
        </div>

        <hr />

        <asp:GridView ID="gvIzinler" runat="server" CssClass="table table-bordered mt-3"
                      AutoGenerateColumns="false" DataKeyNames="IzinID"
                      OnSelectedIndexChanged="gvIzinler_SelectedIndexChanged"
                      OnRowDeleting="gvIzinler_RowDeleting">
            <Columns>
                <asp:BoundField DataField="AdSoyad" HeaderText="Ad Soyad" ReadOnly="true" />
                <asp:BoundField DataField="IzinBaslangic" HeaderText="Başlangıç" DataFormatString="{0:dd.MM.yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="IzinBitis" HeaderText="Bitiş" DataFormatString="{0:dd.MM.yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="IzinTipi" HeaderText="İzin Tipi" />
                <asp:BoundField DataField="Aciklama" HeaderText="Açıklama" />
                <asp:CommandField ShowSelectButton="true" SelectText="Düzenle" ShowDeleteButton="true" DeleteText="Sil" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>