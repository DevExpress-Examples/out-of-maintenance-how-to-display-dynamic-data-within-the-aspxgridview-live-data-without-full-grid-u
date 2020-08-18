<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.15.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxCallback" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.15.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.15.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript"> 

        function RefreshGrid() {
            var firstRowIndex = grid.GetTopVisibleIndex();
            var visibleRowCount = grid.GetVisibleRowsOnPage();

            callback.PerformCallback(firstRowIndex + '|' + visibleRowCount);
        }

        var timeout;
        function ScheduleGridUpdate() {
            window.clearTimeout(timeout);
            timeout = window.setTimeout(
                function () { RefreshGrid(); },
                2000
            );
        }

        function SetLiveCellValue(visibleIndex, columnIndex, value) {
            var label = ASPxClientControl.GetControlCollection().GetByName("lbLiveText_" + visibleIndex + "_" + columnIndex);
            if (label != null) {
                label.SetValue(value);
            }
        }

        function callback_CallbackComplete(s, e) {
            var result = e.result.split('|');
            var columnIndexes = result[0].split('#');
            var rowValues = result[1].split('#');

            var firstRowIndex = grid.GetTopVisibleIndex();

            for (var i = 0; i < rowValues.length; i++) {
                var visibleIndex = firstRowIndex + i;
                var values = rowValues[i].split('~');

                for (var j = 0; j < columnIndexes.length; j++) {
                    SetLiveCellValue(visibleIndex, columnIndexes[j], values[j]);
                }
            }
            
            ScheduleGridUpdate();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <dx:ASPxGridView ID="gvProducts" runat="server" ClientInstanceName="grid" AutoGenerateColumns="False"
            KeyFieldName="ProductID" OnInit="gvProducts_Init">
            <Columns>
                <dx:GridViewDataTextColumn FieldName="ProductID" ReadOnly="True" VisibleIndex="0">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="ProductName" VisibleIndex="1">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="UnitPrice" VisibleIndex="2">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="UnitsInStock" VisibleIndex="3">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="UnitsOnOrder" VisibleIndex="4">
                </dx:GridViewDataTextColumn>
            </Columns>
            <ClientSideEvents Init="function(s, e) { ScheduleGridUpdate(); }" EndCallback="function(s, e) { ScheduleGridUpdate(); }" />
        </dx:ASPxGridView>
        <dx:ASPxCallback ID="cbLiveDataUpdate" runat="server" ClientInstanceName="callback"
            OnCallback="cbLiveDataUpdate_Callback">
            <ClientSideEvents CallbackComplete="callback_CallbackComplete" />
        </dx:ASPxCallback>
    </div>
    </form>
</body>
</html>
