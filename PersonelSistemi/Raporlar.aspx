<%@ Page Title="Departman Maaş Raporları" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Raporlar.aspx.cs" Inherits="PersonelSistemi.Raporlar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Departman Maaş Raporları</title>
    <style>
        #charts_wrapper {
            display: flex;
            justify-content: center;
            align-items: flex-start;
            gap: 30px;
            width: 100%;
            margin-top: 20px;
        }
        #chart_div, #chart_unvan {
            flex: 1;
            min-width: 400px;
            height: 400px;
        }
        #chart_mesai, #chart_mesai_gunluk {
            width: 100%;
            min-height: 400px;
            margin-top: 20px;
        }
        .uyari-text {
            font-size: 0.9rem;
            color: #d9534f;
            margin-top: 5px;
            display: block;
        }
        .rapor-buttons {
            display: flex;
            justify-content: center;
            gap: 10px;
            margin-bottom: 20px;
        }
    </style>

    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript">
        google.charts.load('current', { packages: ['corechart'] });
        google.charts.setOnLoadCallback(drawCharts);

        function drawCharts() {
            drawDepartmanChart();
            drawUnvanChart();
        }

        function drawDepartmanChart() {
            var jsonData = document.getElementById('<%= hfChartData.ClientID %>').value;
            if (!jsonData || jsonData === "[]") return;

            var dataArr = JSON.parse(jsonData);
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Departman');
            data.addColumn('number', 'Toplam Maaş');
            data.addColumn({ type: 'string', role: 'tooltip' });

            for (var i = 0; i < dataArr.length; i++) {
                var dep = dataArr[i].Departman;
                var maas = dataArr[i].ToplamMaas;
                var tooltipText = "Departman: " + dep + "\nToplam Maaş: " + maas + " ₺";
                data.addRow([dep, maas, tooltipText]);
            }

            var options = {
                title: 'Departmanlara Göre Maaş Dağılımı',
                width: '100%',
                height: 400,
                legend: { position: 'bottom' },
                colors: ['#4E79A7', '#F28E2B', '#E15759'],
                chartArea: { width: '70%', height: '70%' },
                tooltip: { isHtml: false }
            };

            var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
            chart.draw(data, options);
        }

        function drawUnvanChart() {
            var jsonData = document.getElementById('<%= hfChartDataUnvan.ClientID %>').value;
            if (!jsonData || jsonData === "[]") return;

            var dataArr = JSON.parse(jsonData);
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Unvan');
            data.addColumn('number', 'Toplam Maaş');

            for (var i = 0; i < dataArr.length; i++) {
                data.addRow([dataArr[i].Unvan, dataArr[i].ToplamMaas]);
            }

            var options = {
                title: 'Unvanlara Göre Maaş Dağılımı',
                pieHole: 0.4,
                width: '100%',
                height: 400,
                legend: { position: 'right' }
            };

            var chart = new google.visualization.PieChart(document.getElementById('chart_unvan'));
            chart.draw(data, options);
        }

        function drawMesaiChart() {
            var jsonData = document.getElementById('<%= hfMesaiChartData.ClientID %>').value;
            if (!jsonData || jsonData === "[]") return;

            var dataArr = JSON.parse(jsonData);
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Departman');
            data.addColumn('number', 'Toplam Saat');

            for (var i = 0; i < dataArr.length; i++) {
                data.addRow([dataArr[i].Departman, dataArr[i].ToplamSaat]);
            }

            var options = {
                title: 'Departmanlara Göre Toplam Mesai Saatleri',
                width: '100%',
                height: 400,
                legend: { position: 'bottom' }
            };

            var chart = new google.visualization.ColumnChart(document.getElementById('chart_mesai'));
            chart.draw(data, options);
        }

        function drawGunlukMesaiChart() {
            var jsonData = document.getElementById('<%= hfMesaiGunlukChartData.ClientID %>').value;
            if (!jsonData || jsonData === "[]") return;

            var dataArr = JSON.parse(jsonData);
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Tarih');
            data.addColumn('number', 'Toplam Saat');

            for (var i = 0; i < dataArr.length; i++) {
                data.addRow([dataArr[i].Tarih, dataArr[i].ToplamSaat]);
            }

            var options = {
                title: 'Seçilen Ay İçin Günlük Toplam Mesai Saatleri',
                width: '100%',
                height: 400,
                hAxis: { title: 'Tarih', slantedText: true, slantedTextAngle: 45 },
                vAxis: { title: 'Toplam Saat' },
                legend: { position: 'none' },
                colors: ['#007bff']
            };

            var chart = new google.visualization.ColumnChart(document.getElementById('chart_mesai_gunluk'));
            chart.draw(data, options);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container text-center mt-4">
        <h3>Raporlar</h3>
        <hr />

        <div class="rapor-buttons">
            <asp:Button ID="btnMaasRapor" runat="server" Text="Maaş Raporları" CssClass="btn btn-primary" OnClick="btnMaasRapor_Click" />
            <asp:Button ID="btnMesaiRapor" runat="server" Text="Mesai Raporları" CssClass="btn btn-secondary" OnClick="btnMesaiRapor_Click" />
        </div>

        <asp:Panel ID="pnlMaasRaporlari" runat="server" Visible="true">
            <div class="row justify-content-center mb-2">
                <div class="col-md-3">
                    <asp:TextBox ID="txtBaslangic" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="txtBitis" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnGetir" runat="server" Text="Raporu Getir" CssClass="btn btn-primary w-100" OnClick="btnGetir_Click" />
                </div>
            </div>
            <small class="uyari-text">Tarih aralığı seçiniz</small>

            <div id="charts_wrapper">
                <div id="chart_unvan"></div>
                <div id="chart_div"></div>
            </div>

            <asp:HiddenField ID="hfChartData" runat="server" />
            <asp:HiddenField ID="hfChartDataUnvan" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlMesaiRaporlari" runat="server" Visible="false">
            <div class="row justify-content-center mb-2">
                <div class="col-md-3">
                    <asp:DropDownList ID="ddlAy" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-3">
                    <asp:DropDownList ID="ddlYil" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnMesaiGetir" runat="server" Text="Raporu Getir" CssClass="btn btn-info w-100" OnClick="btnMesaiGetir_Click" />
                </div>
            </div>
            <small class="uyari-text">Ay ve yıl seçiniz</small>
            <div id="chart_mesai"></div>
            <div id="chart_mesai_gunluk"></div>

            <asp:HiddenField ID="hfMesaiChartData" runat="server" />
            <asp:HiddenField ID="hfMesaiGunlukChartData" runat="server" />
        </asp:Panel>
    </div>
</asp:Content>