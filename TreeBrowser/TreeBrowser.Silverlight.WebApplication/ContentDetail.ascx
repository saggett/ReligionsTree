<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentDetail.ascx.cs"
    Inherits="TreeBrowser.Silverlight.WebApplication.ContentDetail" %>
<div>
    <csla:CslaDataSource ID="HtmlContentDataSource" runat="server" TypeName="TreeBrowser.Entities.HtmlContent, TreeBrowser.Entities"
        OnSelectObject="HtmlContentDataSource_SelectObject" TypeSupportsPaging="False"
        TypeSupportsSorting="False" />
    <asp:FormView ID="HtmlContentView" runat="server" Height="50px" Width="536px" AutoGenerateRows="False"
        DataSourceID="HtmlContentDataSource" DataKeyNames="Id" BorderStyle="None">
        <ItemTemplate>
            <div style="font-weight: bold; margin-bottom: 8px">
                <%# Eval("Name") %>
            </div>
            <div>
                <%# Eval("Content") %>
            </div>
        </ItemTemplate>
    </asp:FormView>
</div>
