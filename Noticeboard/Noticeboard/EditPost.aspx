<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="EditPost.aspx.cs"
    Inherits="Noticeboard.EditPost" %>

<asp:Content ID="ContentBody" ContentPlaceHolderID="MainContent" runat="server">
    <asp:FormView runat="server" ID="FormViewEditPost"
        ItemType="Noticeboard.Models.Post"
        SelectMethod="FormViewEditPost_GetItem"
        UpdateMethod="FormViewEditPost_UpdateItem"
        DefaultMode="Edit"
        DataKeyNames="PostId">

        <EditItemTemplate>
            <h4 id="PostHeadline" runat="server"></h4>
            <p>Title:</p>
            <asp:TextBox ID="TextBoxPostTitle" runat="server"
                Text="<%# BindItem.Title %>" Width="600" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorTitle" runat="server" 
                ControlToValidate="TextBoxPostTitle"
                ErrorMessage="<span class='label label-important'>Post title should not be empty.</span>" />
            <br />
            <p>Content:</p>
            <asp:TextBox ID="TextBoxPostContent" runat="server"
                Text="<%# BindItem.Content %>"
                TextMode="MultiLine" Rows="6" Width="600" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorContent" runat="server" 
                ControlToValidate="TextBoxPostContent"
                ErrorMessage="<span class='label label-important'>Post content should not be empty.</span>" />
            <br />
            <p>Category</p>
            <asp:DropDownList runat="server"
                ID="DropDownListCategories"
                ItemType="Noticeboard.Models.Category"
                DataTextField="Title"
                DataValueField="CategoryId"
                Width="600">
            </asp:DropDownList>
            <br />
            <p>Upload file</p>
            <asp:FileUpload ID="FileUploadControl" runat="server" />
            <br />
            <asp:LinkButton Text="Save" CommandName="Update"
                CommandArgument="<%#: Item.PostId %>" runat="server"
                CssClass="btn btn-info" />
            <asp:LinkButton ID="ButtonCancel" Text="Cancel"
                CssClass="btn btn-danger"
                runat="server" OnClick="ButtonCancel_Click" CausesValidation="false" />
        </EditItemTemplate>

    </asp:FormView>
</asp:Content>
