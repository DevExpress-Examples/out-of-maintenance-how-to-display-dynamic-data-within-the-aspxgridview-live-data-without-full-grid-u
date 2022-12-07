using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using System.Data;
using System.Data.OleDb;

public partial class _Default : System.Web.UI.Page {
    readonly string[] liveDataFields = { "UnitPrice", "UnitsInStock", "UnitsOnOrder" };

    #region Example Data
    DataTable ProcessSelectCommand(string selectCommandText) {
        OleDbConnection connection;
        try {
            connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\nwind.mdb;Persist Security Info=True");
            OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommandText, connection);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }
        catch {
            return null;
        }
    }

    void ModifyProductsData() {
        DataTable data = Session["Products"] as DataTable;

        Random random = new Random();
        foreach (DataRow row in data.Rows) {
            if (random.Next() % 2 == 0) {
                row["UnitPrice"] = Decimal.Round(Math.Max((decimal)row["UnitPrice"] + (decimal)(random.NextDouble() * 3) - 1, 0), 2);
                row["UnitsInStock"] = Math.Max((short)row["UnitsInStock"] + random.Next(3) - 1, 0);
                row["UnitsOnOrder"] = Math.Max((short)row["UnitsOnOrder"] + random.Next(3) - 1, 0);
            }
        }
    }

    protected void Page_Init(object sender, EventArgs e) {
        if (Session["Products"] == null) {
            Session["Products"] = ProcessSelectCommand("SELECT [ProductID], [ProductName], [UnitPrice], [UnitsInStock], [UnitsOnOrder] FROM [Products]");
        }

        ModifyProductsData();

        gvProducts.DataSource = Session["Products"];
        gvProducts.DataBind();
    }

    #endregion

    protected void gvProducts_Init(object sender, EventArgs e) {
        ASPxGridView gridView = sender as ASPxGridView;
        foreach (string liveField in liveDataFields) {
            GridViewDataColumn col = gridView.Columns[liveField] as GridViewDataColumn;
            col.DataItemTemplate = new LiveDataItem();
        }
    }
    protected void cbLiveDataUpdate_Callback(object source, CallbackEventArgs e) {
        string[] parameters = e.Parameter.Split('|');
        int firstIndex = int.Parse(parameters[0]);
        int rowCount = int.Parse(parameters[1]);

        e.Result = String.Empty;

        //Live data column indexes
        for (int i = 0; i < liveDataFields.Length; i++) {
            GridViewDataColumn col = gvProducts.Columns[liveDataFields[i]] as GridViewDataColumn;
            e.Result += (i == 0) ? col.Index.ToString() : String.Format("#{0}", col.Index);
        }
        e.Result += '|';

        //Row values
        for (int i = 0; i < rowCount; i++) {
            int visibleIndex = i + firstIndex;

            object[] rowValues = (object[])gvProducts.GetRowValues(visibleIndex, liveDataFields);
            string[] rowTexts = Array.ConvertAll<object, string>(rowValues, Convert.ToString);
            string valuesText = String.Join("~", rowTexts);
            e.Result += (i == 0) ? valuesText : String.Format("#{0}", valuesText);
        }
    }
}