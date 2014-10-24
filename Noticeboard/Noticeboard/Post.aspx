<%@ Page Title="Current Post"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Post.aspx.cs"
    Inherits="Noticeboard.Post" %>

<asp:Content ID="ContentBody" ContentPlaceHolderID="MainContent" runat="server">
    <asp:FormView runat="server" ID="FormViewPost"
        DataKeyNames="PostId"
        SelectMethod="FormViewPost_GetItem"
        DeleteMethod="FormViewPost_DeleteItem"
        ItemType="Noticeboard.Models.Post">
        <ItemTemplate>
            <div class="row">
                <div class="well span11">
                    <div class="pull-right">
                        <span class="label label-warning">by <strong><%#: Item.AspNetUser.UserName %></strong></span>
                        <span class="label label-success">created on <%# Item.PostDate %></span>
                        <asp:LinkButton Text="Edit" runat="server"
                            ID="ButtonEditPost"
                            CssClass="btn btn-info btn-small"
                            CommandName="Edit" CommandArgument="<%#: Item.PostId %>"
                            OnCommand="ButtonEditPost_Command" />
                        <asp:LinkButton Text="Delete" runat="server"
                            CssClass="btn btn-danger btn-small"
                            ID="LinkButtonDeletePost"
                            OnClientClick="confirm('Are you sure you want to delete this post')"
                            CommandName="Delete"
                            Style='<%#: this.User.IsInRole("admin") ? "display: inner-block;" :  "display: none;" %>' />
                    </div>
                    <h3><%#: Item.Title %></h3>
                    <p><%#: Item.Content %></p>
                    <asp:PlaceHolder ID="ImagePlaceholder" runat="server"></asp:PlaceHolder>
                </div>
            </div>
            <asp:UpdatePanel runat="server" ID="UpdatePanelComments"
                UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:ListView runat="server" ID="ListViewComments"
                        ItemType="Noticeboard.Models.Comment"
                        DataKeyNames="CommentId"
                        InsertItemPosition="None"
                        OnItemInserted="ListViewComments_ItemInserted"
                        SelectMethod="ListViewComments_GetData"
                        UpdateMethod="ListViewComments_UpdateItem"
                        InsertMethod="ListViewComments_InsertItem"
                        DeleteMethod="ListViewComments_DeleteItem">
                        <LayoutTemplate>
                            <h3>Comments</h3>
                            <ul runat="server" id="itemPlaceholder"></ul>
                            <div class="pagerLine">
                                <asp:DataPager ID="DataPagerComments" runat="server" PageSize="4">
                                    <Fields>
                                        <asp:NextPreviousPagerField ShowFirstPageButton="True"
                                            ShowNextPageButton="False" ShowPreviousPageButton="False" />
                                        <asp:NumericPagerField />
                                        <asp:NextPreviousPagerField ShowLastPageButton="True"
                                            ShowNextPageButton="False" ShowPreviousPageButton="False" />
                                    </Fields>
                                </asp:DataPager>
                            </div>
                        </LayoutTemplate>

                        <ItemTemplate>
                            <div class="row">
                                <div id="commentHolder" class="well span11">
                                    <div class="pull-right">
                                        <span class="label label-warning">by <strong><%#: Item.AspNetUser.UserName %></strong></span>
                                        <span class="label label-success">created on <%# Item.CommentDate %></span>
                                        <asp:LinkButton Text="Edit" runat="server" ID="ButtonEditComment"
                                            CssClass="btn btn-info btn-small"
                                            CommandName="Edit" CommandArgument="<%#: Item.CommentId %>"
                                            OnCommand="ButtonEditComment_Command" />
                                        <asp:LinkButton Text="Delete" runat="server"
                                            ID="LinkButtonDeleteComment" CssClass="btn btn-danger btn-small"
                                            CommandName="Delete"
                                            Style='<%#: this.User.IsInRole("admin") ? "display: inner-block;" :  "display: none;" %>' />
                                    </div>
                                    <p><%#: Item.Content %></p>
                                </div>
                            </div>
                        </ItemTemplate>

                        <InsertItemTemplate>
                            <h4>Add New Comment</h4>
                            <asp:TextBox ID="TextBoxComment" runat="server"
                                Text="<%# BindItem.Content  %>"
                                TextMode="MultiLine" Rows="6" Width="600" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorContent" runat="server"
                                ControlToValidate="TextBoxComment"
                                ErrorMessage="<span class='label label-important'>Comment content should not be empty.</span>" />
                            <br />
                            <asp:LinkButton Text="Save" CommandName="Insert" runat="server"
                                ID="ButtonInsert"
                                CssClass="btn btn-info btn-small" />
                            <asp:LinkButton Text="Cancel" CommandName="Cancel" runat="server"
                                CausesValidation="false" CssClass="btn btn-danger btn-small" />
                        </InsertItemTemplate>

                        <EditItemTemplate>
                            <h4>Edit Comment</h4>
                            <asp:TextBox ID="TextBoxComment" runat="server"
                                Text="<%# BindItem.Content  %>"
                                TextMode="MultiLine" Rows="6" Width="600" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorContent" runat="server"
                                ControlToValidate="TextBoxComment"
                                ErrorMessage="<span class='label label-important'>Comment content should not be empty.</span>" />
                            <br />
                            <asp:LinkButton Text="Save" CommandName="Update" CssClass="btn btn-info btn-small"
                                CommandArgument="<%# Item.CommentId %>" runat="server" />
                            <asp:LinkButton Text="Cancel" CommandName="Cancel" runat="server"
                                CssClass="btn btn-danger btn-small" CausesValidation="false" />
                        </EditItemTemplate>
                    </asp:ListView>

                    <asp:LinkButton ID="ButtonInsertComment" runat="server"
                        OnClick="ButtonInsertComment_Click" Text="Add Comment" CssClass="btn btn-primary" />
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UpdatePanelButton">
                <ContentTemplate>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ItemTemplate>
    </asp:FormView>
</asp:Content>
