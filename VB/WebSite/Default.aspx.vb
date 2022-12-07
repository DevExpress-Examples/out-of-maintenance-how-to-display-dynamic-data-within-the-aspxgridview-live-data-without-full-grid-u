Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.Web
Imports System.Data
Imports System.Data.OleDb

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Private ReadOnly liveDataFields() As String = { "UnitPrice", "UnitsInStock", "UnitsOnOrder" }

	#Region "Example Data"
	Private Function ProcessSelectCommand(ByVal selectCommandText As String) As DataTable
		Dim connection As OleDbConnection
		Try
			connection = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\nwind.mdb;Persist Security Info=True")
			Dim adapter As New OleDbDataAdapter(selectCommandText, connection)
			Dim table As New DataTable()
			adapter.Fill(table)
			Return table
		Catch
			Return Nothing
		End Try
	End Function

	Private Sub ModifyProductsData()
		Dim data As DataTable = TryCast(Session("Products"), DataTable)

		Dim random As New Random()
		For Each row As DataRow In data.Rows
			If random.Next() Mod 2 = 0 Then
				row("UnitPrice") = Decimal.Round(Math.Max(CDec(row("UnitPrice")) + CDec(random.NextDouble() * 3) - 1, 0), 2)
				row("UnitsInStock") = Math.Max(CShort(Fix(row("UnitsInStock"))) + random.Next(3) - 1, 0)
				row("UnitsOnOrder") = Math.Max(CShort(Fix(row("UnitsOnOrder"))) + random.Next(3) - 1, 0)
			End If
		Next row
	End Sub

	Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
		If Session("Products") Is Nothing Then
			Session("Products") = ProcessSelectCommand("SELECT [ProductID], [ProductName], [UnitPrice], [UnitsInStock], [UnitsOnOrder] FROM [Products]")
		End If

		ModifyProductsData()

		gvProducts.DataSource = Session("Products")
		gvProducts.DataBind()
	End Sub

	#End Region

	Protected Sub gvProducts_Init(ByVal sender As Object, ByVal e As EventArgs)
		Dim gridView As ASPxGridView = TryCast(sender, ASPxGridView)
		For Each liveField As String In liveDataFields
			Dim col As GridViewDataColumn = TryCast(gridView.Columns(liveField), GridViewDataColumn)
			col.DataItemTemplate = New LiveDataItem()
		Next liveField
	End Sub
	Protected Sub cbLiveDataUpdate_Callback(ByVal source As Object, ByVal e As CallbackEventArgs)
		Dim parameters() As String = e.Parameter.Split("|"c)
		Dim firstIndex As Integer = Integer.Parse(parameters(0))
		Dim rowCount As Integer = Integer.Parse(parameters(1))

		e.Result = String.Empty

		'Live data column indexes
		For i As Integer = 0 To liveDataFields.Length - 1
			Dim col As GridViewDataColumn = TryCast(gvProducts.Columns(liveDataFields(i)), GridViewDataColumn)
			e.Result += If((i = 0), col.Index.ToString(), String.Format("#{0}", col.Index))
		Next i
		e.Result &= "|"c

		'Row values
		For i As Integer = 0 To rowCount - 1
			Dim visibleIndex As Integer = i + firstIndex

            Dim rowValues() As Object = CType(gvProducts.GetRowValues(visibleIndex, liveDataFields), Object())
            Dim ObjectToStringConverter As Converter(Of Object, String) = New Converter(Of Object, String)(Function(obj As Object) obj.ToString())
            Dim rowTexts() As String = Array.ConvertAll(Of Object, String)(rowValues, ObjectToStringConverter)
            Dim valuesText As String = String.Join("~", rowTexts)
			e.Result += If((i = 0), valuesText, String.Format("#{0}", valuesText))
		Next i
	End Sub
End Class