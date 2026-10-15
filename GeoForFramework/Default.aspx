<%@ Page Title="WGS84 to SWEREF99 TM" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GeoForFramework._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <h1>WGS84 to SWEREF99 TM</h1>
        <p class="lead">Convert longitude/latitude in WGS84 to the Swedish national grid coordinates in SWEREF99 TM.</p>

        <div class="row mt-4">
            <div class="col-md-6">
                <div class="card">
                    <div class="card-body">
                        <h2 class="h4">Input</h2>
                        <div class="mb-3">
                            <label class="form-label" for="LatitudeTextBox">Latitude (WGS84)</label>
                            <asp:TextBox ID="LatitudeTextBox" runat="server" CssClass="form-control" Text="57.708870" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label" for="LongitudeTextBox">Longitude (WGS84)</label>
                            <asp:TextBox ID="LongitudeTextBox" runat="server" CssClass="form-control" Text="11.974560" />
                        </div>
                        <asp:Button ID="ConvertButton" runat="server" Text="Convert" CssClass="btn btn-primary" OnClick="ConvertButton_Click" />
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card">
                    <div class="card-body">
                        <h2 class="h4">Output</h2>
                        <div class="mb-3">
                            <label class="form-label">Easting</label>
                            <asp:TextBox ID="EastingTextBox" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Northing</label>
                            <asp:TextBox ID="NorthingTextBox" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Result</label>
                            <asp:Label ID="ResultLabel" runat="server" CssClass="form-control-plaintext" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
