Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.Web.ASPxGridView
Imports DevExpress.Web.ASPxEditors

Public Class LiveDataItem
	Implements ITemplate
	Public Sub New()
	End Sub

	Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
		Dim gridContainer As GridViewDataItemTemplateContainer = TryCast(container, GridViewDataItemTemplateContainer)

		Dim label As New ASPxLabel()
		label.ID = "lbText"
		gridContainer.Controls.Add(label)

		label.Width = New Unit(100, UnitType.Percentage)
		label.ClientInstanceName = String.Format("lbLiveText_{0}_{1}", gridContainer.VisibleIndex, gridContainer.Column.Index)

		label.Value = DataBinder.Eval(gridContainer.DataItem, gridContainer.Column.FieldName)
	End Sub
End Class