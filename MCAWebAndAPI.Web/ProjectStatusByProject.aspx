<%@ Page Title="Project Status by Project" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="ProjectStatusByProject.aspx.cs" Inherits="MCAWebAndAPI.Web.ProjectStatusByProject" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <div class="col-md-12">
            <div id="project-status-by-project-grid"></div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <div id="project-status-by-project-chart"></div>
        </div>
    </div>
    
    <script type="text/javascript">

        $(document).ready(function () {
            EPMO.Grids.displayProjectGrid("project-status-by-project-grid");
            EPMO.Charts.displayProjectStatusByProjectChart("project-status-by-project-chart");
        });

    </script>

</asp:Content>

