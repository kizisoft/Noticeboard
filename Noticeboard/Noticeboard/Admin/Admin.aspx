<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Admin.aspx.cs" Inherits="Noticeboard.Admin.Admin" %>

<asp:Content ID="ContentAdminPanel" ContentPlaceHolderID="MainContent" runat="server">
    <asp:LinkButton runat="server" CommandName="Redirect" OnCommand="Redirect_Command" CommandArgument="~/Admin/Users"> Users</asp:LinkButton>
    <asp:LinkButton runat="server" CommandName="Redirect" OnCommand="Redirect_Command" CommandArgument="~/Admin/Categories"> Categories</asp:LinkButton>
    <asp:LinkButton runat="server" CommandName="Redirect" OnCommand="Redirect_Command" CommandArgument="~/Admin/Posts"> Posts</asp:LinkButton>
    <asp:LinkButton runat="server" CommandName="Redirect" OnCommand="Redirect_Command" CommandArgument="~/Admin/Comments"> Comments</asp:LinkButton>
</asp:Content>
