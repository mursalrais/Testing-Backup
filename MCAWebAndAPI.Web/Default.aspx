<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MCAWebAndAPI.Web._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-12">
        <div class="portlet">
            <div class="portlet-header">
                <h3>
                <i class="fa fa-table"></i>Test Kendo Grid: Task List
                </h3>
            </div>
            <div class="portlet-content">
                <div class="row">
                    <div class="col-md-9">
                        <div class="col-md-6">
                            <div class="form-group">
                            <label for="activity-dropdown">Activity</label>
                            <input id="activity-dropdown" style="width: 100%" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                            <label for="sub-activity-dropdown">Sub-Activity</label>
                            <input id="sub-activity-dropdown" style="width: 100%" />
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">
                            <div class="form-group">
                            <label for="quarter-dropdown">Quarter</label>
                            <input id="quarter-dropdown" style="width: 100%" />
                            </div>
                    </div>
                </div>
                <div class="row">
                    <div id="task-by-activity-kendo-grid">
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
