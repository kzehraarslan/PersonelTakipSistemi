<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Departmanlar.aspx.cs" Inherits="PersonelSistemi.Departmanlar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Departmanlar Yönetimi</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <div class="container mt-4">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

                <!-- FORM ALANI (Sadece Admin Görür) -->
                <div id="formPanel" runat="server" class="card shadow p-4 mb-4">
                    <h3 class="mb-3">Departman Ekle / Güncelle</h3>
                    <asp:Label ID="lblMesaj" runat="server" CssClass="fw-bold" />

                    <div class="row g-3 mt-2">
                        <div class="col-md-6">
                            <asp:TextBox ID="txtDepartmanAdi" runat="server" CssClass="form-control" Placeholder="Departman Adı" />
                        </div>
                        <div class="col-md-3">
                            <asp:TextBox ID="txtCalismaSaatiBaslangic" runat="server" CssClass="form-control" Placeholder="Başlangıç (HH:mm)" />
                        </div>
                        <div class="col-md-3">
                            <asp:TextBox ID="txtCalismaSaatiBitis" runat="server" CssClass="form-control" Placeholder="Bitiş (HH:mm)" />
                        </div>
                    </div>

                    <div class="mt-4 d-flex gap-2">
                        <asp:Button ID="btnEkle" runat="server" Text="Departman Ekle" CssClass="btn btn-success" OnClick="btnEkle_Click" />
                        <asp:Button ID="btnGuncelle" runat="server" Text="Departman Güncelle" CssClass="btn btn-warning" Visible="false" OnClick="btnGuncelle_Click" />
                        <asp:Button ID="btnIptal" runat="server" Text="İptal" CssClass="btn btn-secondary" Visible="false" OnClick="btnIptal_Click" />
                    </div>
                </div>

                <!-- LİSTE ALANI -->
                <div class="card shadow p-4">
                    <h3 class="mb-3">Departman Listesi</h3>

                    <asp:GridView ID="gvDepartman" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered"
                                  DataKeyNames="DepartmanID" OnSelectedIndexChanged="gvDepartman_SelectedIndexChanged"
                                  OnRowDeleting="gvDepartman_RowDeleting" OnRowDataBound="gvDepartman_RowDataBound"
                                  EmptyDataText="Kayıt bulunamadı.">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Düzenle" />
                            <asp:CommandField ShowDeleteButton="True" DeleteText="Sil" />
                            <asp:BoundField DataField="DepartmanID" HeaderText="ID" ReadOnly="true" />
                            <asp:BoundField DataField="DepartmanAdi" HeaderText="Departman Adı" />
                            <asp:TemplateField HeaderText="Başlangıç">
                                <ItemTemplate>
                                    <%# Eval("CalismaSaatiBaslangic", "{0:hh\\:mm}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Bitiş">
                                <ItemTemplate>
                                    <%# Eval("CalismaSaatiBitis", "{0:hh\\:mm}") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>