<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReligionDetail.ascx.cs"
    Inherits="TreeBrowser.Silverlight.WebApplication.ReligionDetail" %>
<div>
    <csla:CslaDataSource ID="ReadOnlyLineagesDataSource" runat="server" TypeName="TreeBrowser.Entities.ReadOnlyLineages, TreeBrowser.Entities"
        OnSelectObject="ReadOnlyLineagesDataSource_SelectObject" TypeSupportsPaging="False"
        TypeSupportsSorting="False" />
    <csla:CslaDataSource ID="RootLineageDataSource" runat="server" TypeName="TreeBrowser.Entities.Lineage, TreeBrowser.Entities"
        OnSelectObject="RootLineageDataSource_SelectObject" TypeSupportsPaging="False"
        TypeSupportsSorting="False" />
    <asp:FormView ID="RootLineageView" runat="server" Height="50px" Width="536px" AutoGenerateRows="False"
        DataSourceID="RootLineageDataSource" DataKeyNames="Id" BorderStyle="None">
        <ItemTemplate>
            <p>
                <b>
                    <%# Eval("Name") %></b></p>
            <p>
                <%# Eval("FoundingText") %></p>
            <%# Eval("Notes") %>
        </ItemTemplate>
    </asp:FormView>
    <br />
    <br />
    <asp:Label ID="ChildReligionsLabel" runat="server">Child Religions:</asp:Label>
    <br />
    <asp:GridView ID="LineagesGridView" runat="server" AllowPaging="False" AutoGenerateColumns="False"
        DataSourceID="ReadOnlyLineagesDataSource" DataKeyNames="Id">
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id"
                Visible="False" />
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="Default.aspx?lineageId={0}"
                DataTextField="Name" HeaderText="Name" />
            <asp:BoundField DataField="DisplayStartYear" HeaderText="Start Date" ReadOnly="True" />
            <asp:BoundField DataField="DisplayEndYear" HeaderText="End Date" ReadOnly="True" />
            <asp:CommandField ShowDeleteButton="False" SelectText="View" />
        </Columns>
    </asp:GridView>
</div>
