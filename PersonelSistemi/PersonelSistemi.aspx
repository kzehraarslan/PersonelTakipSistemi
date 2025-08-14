<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PersonelSistemi.aspx.cs" Inherits="PersonelSistemi.PersonelSistemi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Personel Sistemi</title>
    <meta charset="utf-8" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <div class="container mt-4">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

                <!-- Personel Ekle / Güncelle Paneli -->
                <asp:Panel ID="pnlForm" runat="server" Visible="false">
                    <div class="card shadow p-4 mb-4">
                        <h3 class="mb-3">Personel Ekle / Güncelle</h3>

                        <asp:Label ID="lblMesaj" runat="server" CssClass="text-danger" />

                        <div class="row g-3">
                            <div class="col-md-6">
                                <asp:TextBox ID="txtAd" runat="server" CssClass="form-control" Placeholder="Ad" />
                            </div>
                            <div class="col-md-6">
                                <asp:TextBox ID="txtSoyad" runat="server" CssClass="form-control" Placeholder="Soyad" />
                            </div>
                            <div class="col-md-6">
                                <asp:TextBox ID="txtTelefon" runat="server" CssClass="form-control" Placeholder="Telefon" />
                            </div>
                            <div class="col-md-6">
                                <asp:TextBox ID="txtAdres" runat="server" CssClass="form-control" Placeholder="Adres" />
                            </div>
                            <div class="col-md-6">
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Email" />
                            </div>
                            <div class="col-md-6">
                                <asp:TextBox ID="txtMaas" runat="server" CssClass="form-control" Placeholder="Maaş" />
                            </div>
                            <div class="col-md-6">
                                <asp:DropDownList ID="ddlDepartman" runat="server" CssClass="form-select" />
                            </div>
                            <div class="col-md-6">
                                <asp:DropDownList ID="ddlUnvan" runat="server" CssClass="form-select" />
                            </div>
                        </div>

                        <div class="mt-4 d-flex gap-2">
                            <asp:Button ID="btnEkle" runat="server" Text="Personel Ekle" CssClass="btn btn-success" OnClick="btnEkle_Click" />
                            <asp:Button ID="btnGuncelle" runat="server" Text="Personel Güncelle" CssClass="btn btn-warning" Visible="false" OnClick="btnGuncelle_Click" />
                        </div>
                    </div>
                </asp:Panel>

                <!-- Personel Listesi -->
                <div class="card shadow p-4">
                    <h3 class="mb-3">Personel Listesi</h3>

                    <asp:GridView ID="gvPersonel" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-striped table-bordered"
                        DataKeyNames="PersonelID"
                        OnSelectedIndexChanged="gvPersonel_SelectedIndexChanged"
                        OnRowDeleting="gvPersonel_RowDeleting"
                        OnRowDataBound="gvPersonel_RowDataBound"
                        EmptyDataText="Kayıt bulunamadı.">

                        <Columns>
                            <asp:CommandField ShowSelectButton="true" />
                            <asp:CommandField ShowDeleteButton="true" />
                            <asp:BoundField DataField="PersonelID" HeaderText="ID" ReadOnly="true" />
                            <asp:BoundField DataField="Ad" HeaderText="Ad" />
                            <asp:BoundField DataField="Soyad" HeaderText="Soyad" />
                            <asp:BoundField DataField="Telefon" HeaderText="Telefon" />
                            <asp:BoundField DataField="Adres" HeaderText="Adres" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:BoundField DataField="Maas" HeaderText="Maaş" />
                            <asp:BoundField DataField="DepartmanAdi" HeaderText="Departman" ReadOnly="true" />
                            <asp:BoundField DataField="UnvanAdi" HeaderText="Unvan" ReadOnly="true" />
                        </Columns>
                    </asp:GridView>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>