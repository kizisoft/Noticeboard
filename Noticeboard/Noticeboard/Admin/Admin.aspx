<%@ Page Title="Admin panel" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Admin.aspx.cs" Inherits="Noticeboard.Admin.Admin" %>

<asp:Content ID="ContentAdminPanel" ContentPlaceHolderID="MainContent" runat="server" class="btn-group">
    <asp:LinkButton runat="server" CommandName="Redirect" OnCommand="Redirect_Command" CommandArgument="~/Admin/Users" class="btn btn-default"> Users</asp:LinkButton>
    <asp:LinkButton runat="server" CommandName="Redirect" OnCommand="Redirect_Command" CommandArgument="~/Admin/Categories" class="btn btn-default"> Categories</asp:LinkButton>
    <asp:LinkButton runat="server" CommandName="Redirect" OnCommand="Redirect_Command" CommandArgument="~/Admin/Posts" class="btn btn-default"> Posts</asp:LinkButton>
    <asp:LinkButton runat="server" CommandName="Redirect" OnCommand="Redirect_Command" CommandArgument="~/Admin/Comments" class="btn btn-default"> Comments</asp:LinkButton>
</asp:Content>
