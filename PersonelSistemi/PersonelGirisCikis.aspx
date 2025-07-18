<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PersonelGirisCikis.aspx.cs" Inherits="PersonelSistemi.PersonelGirisCikis" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
    <title>Giriş-Çıkış Takibi</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h3>Personel Giriş / Çıkış Takibi</h3>

        <asp:Label ID="lblMesaj" runat="server" CssClass="text-danger mb-3 d-block" />

        <!-- Kayıt Ekleme Alanı -->
        <div class="row g-3 mb-4">
            <div class="col-md-4">
                <asp:DropDownList ID="ddlPersonel" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-4">
                <asp:TextBox ID="txtGiris" runat="server" CssClass="form-control" Placeholder="Giriş Saati (HH:mm)" />
            </div>
            <div class="col-md-4">
                <asp:TextBox ID="txtCikis" runat="server" CssClass="form-control" Placeholder="Çıkış Saati (HH:mm)" />
            </div>
        </div>

        <div class="mb-5">
            <asp:Button ID="btnKaydet" runat="server" Text="Kaydet / Güncelle" CssClass="btn btn-success" OnClick="btnKaydet_Click" />
            <asp:Button ID="btnIptal" runat="server" Text="İptal" CssClass="btn btn-secondary ms-2" OnClick="btnIptal_Click" Visible="false" />
        </div>

        <hr />

        <!-- Filtreleme Alanları -->
        <h5>Filtreleme</h5>
        <div class="row g-3 mb-3">
            <div class="col-md-4">
                <asp:DropDownList ID="ddlFiltrePersonel" runat="server" CssClass="form-select" AutoPostBack="false" />
            </div>
            <div class="col-md-4">
                <asp:TextBox ID="txtFiltreBaslangic" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-4">
                <asp:TextBox ID="txtFiltreBitis" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
        </div>
        <div class="mb-4">
            <asp:Button ID="btnFiltrele" runat="server" Text="Filtrele" CssClass="btn btn-primary" OnClick="btnFiltrele_Click" />
            <asp:Button ID="btnBugun" runat="server" Text="Bugünün Kayıtları" CssClass="btn btn-info ms-2" OnClick="btnBugun_Click" />
            <asp:Button ID="btnTumKayitlar" runat="server" Text="Tüm Kayıtlar" CssClass="btn btn-secondary ms-2" OnClick="btnTumKayitlar_Click" />
        </div>

        <!-- Kayıtlar GridView -->
        <asp:GridView ID="gvKayitlar" runat="server" CssClass="table table-bordered table-striped"
                      AutoGenerateColumns="false" DataKeyNames="KayitID"
                      OnRowEditing="gvKayitlar_RowEditing"
                      OnRowCancelingEdit="gvKayitlar_RowCancelingEdit"
                      OnRowUpdating="gvKayitlar_RowUpdating"
                      OnRowDeleting="gvKayitlar_RowDeleting"
                      OnRowDataBound="gvKayitlar_RowDataBound"
                      AllowPaging="true" PageSize="10"
                      OnPageIndexChanging="gvKayitlar_PageIndexChanging">

            <Columns>
                <asp:BoundField DataField="AdSoyad" HeaderText="Ad Soyad" ReadOnly="true" ItemStyle-Width="20%" />
                <asp:BoundField DataField="Tarih" HeaderText="Tarih" DataFormatString="{0:dd.MM.yyyy}" ReadOnly="true" ItemStyle-Width="15%" />

                <asp:TemplateField HeaderText="Giriş" ItemStyle-Width="15%">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditGiris" runat="server" Text='<%# Bind("GirisSaatiFormatted") %>' CssClass="form-control" />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("GirisSaatiFormatted") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Çıkış" ItemStyle-Width="15%">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditCikis" runat="server" Text='<%# Bind("CikisSaatiFormatted") %>' CssClass="form-control" />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <%# Eval("CikisSaatiFormatted") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="GecikmeDakika" HeaderText="Gecikme (dk)" ReadOnly="true" ItemStyle-Width="10%" />
                <asp:BoundField DataField="CalismaSuresi" HeaderText="Çalışma Süresi" ReadOnly="true" ItemStyle-Width="15%" />

                <asp:TemplateField HeaderText="Durum" ItemStyle-Width="10%">
                    <ItemTemplate>
                        <asp:Label ID="lblDurum" runat="server" Text='<%# Eval("Durum") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="DurumID" HeaderText="DurumID" Visible="false" />

                <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>