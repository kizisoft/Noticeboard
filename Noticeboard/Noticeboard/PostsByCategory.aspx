<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PostsByCategory.aspx.cs" Inherits="Noticeboard.PostsByCategory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <a href="Default.aspx" class="btn btn-inverse pull-right">Go Back</a>
    <h3><asp:Label runat="server" ID="LabelCategoryTitle"></asp:Label></h3>
    
    <asp:ListView ID="PostsList"
        runat="server"
        ItemType="Noticeboard.Models.Post"
        SelectMethod="PostsList_GetData">
        <LayoutTemplate>
            <div class="well">
                <table class="table table-hover">
                    <colgroup>
                        <col style="width: 50%"/>
                        <col style="width: 15%"/>
                        <col style="width: 10%"/>
                        <col style="width: 25%"/>
                    </colgroup>
                    <thead>
                        <tr>
                            <th>Post</th>
                            <th class="cell-center">Author</th>
                            <th class="cell-center">Comments</th>
                            <th>Last Comment</th>
                        </tr>
                    </thead>
                    <tbody id="itemPlaceholder" runat="server"></tbody>
                </table>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <a href="Post.aspx?id=<%# Item.PostId %>"><%#: Item.Title%></a>
                </td>
                <td class="cell-center"><%#: Item.AspNetUser.UserName %></td>
                <td class="cell-center"><%# Item.Comments.Count %></td>
                <td>
                    by <%#: Item.Comments.Count > 0 ? Item.Comments.OrderByDescending(c => c.CommentDate).FirstOrDefault().AspNetUser.UserName : "......" %>
                    <br />
                    on <%#: Item.Comments.Count > 0 ? Item.Comments.OrderByDescending(c => c.CommentDate).FirstOrDefault().CommentDate.ToString(): "......" %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>

</asp:Content>
