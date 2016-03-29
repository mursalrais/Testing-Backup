<%@ Page Title="Project Status by Activity" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="ProjectStatusByActivity.aspx.cs" Inherits="MCAWebAndAPI.Web.ProjectStatusByActivity" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <div class="col-md-12">
            <div id="project-status-by-activity-grid"></div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <div id="project-status-by-activity-chart"></div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <div id="project-s-curve-chart"></div>
        </div>
    </div>

    <script type="text/javascript">

        $(document).ready(function () {
            EPMO.Grids.displayActivityGrid("project-status-by-activity-grid");
            EPMO.Charts.displaySCurveChart("project-s-curve-chart");
            EPMO.Charts.displayProjectStatusByActivityChart("project-status-by-activity-chart");
        });

    </script>

</asp:Content>

