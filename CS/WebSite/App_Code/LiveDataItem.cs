using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;

public class LiveDataItem : ITemplate {
    public LiveDataItem() {
    }

    public void InstantiateIn(Control container) {
        GridViewDataItemTemplateContainer gridContainer = container as GridViewDataItemTemplateContainer;

        ASPxLabel label = new ASPxLabel();
        label.ID = "lbText";
        gridContainer.Controls.Add(label);

        label.Width = new Unit(100, UnitType.Percentage);
        label.ClientInstanceName = String.Format("lbLiveText_{0}_{1}", gridContainer.VisibleIndex, gridContainer.Column.Index);

        label.Value = DataBinder.Eval(gridContainer.DataItem, gridContainer.Column.FieldName);
    }
}