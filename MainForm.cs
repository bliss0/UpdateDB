using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Updates
{
    public partial class MainForm : Form
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=ShopDB;Integrated Security=True";
        string commandString = "SELECT * FROM Customers";

        DataTable customers = new DataTable("Customers");

        SqlDataAdapter adapter;

        public MainForm()
        {
            InitializeComponent();

            this.CenterToScreen();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            adapter = new SqlDataAdapter(commandString, connectionString);
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            ConfigureCustomersAdapter(adapter);
            adapter.FillSchema(customers, SchemaType.Mapped);

            customers.Columns[0].AutoIncrementSeed = -1;
            customers.Columns[0].AutoIncrementStep = -1;

            adapter.Fill(customers);

            dataGridView1.DataSource = customers;

            adapter.RowUpdated += adapter_RowUpdated;
        }

        void adapter_RowUpdated(object sender, SqlRowUpdatedEventArgs e)
        {
            if (e.StatementType == StatementType.Insert)
            {
                var insertedRow = e.Row;

                try
                {
                    insertedRow.Table.Columns[0].ReadOnly = false;

                    insertedRow[0] = e.Command.Parameters["NewCustomerNo"].Value;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    insertedRow.Table.Columns[0].ReadOnly = true;
                }
            }
        }

        private static void ConfigureCustomersAdapter(SqlDataAdapter customersAdapter)
        {
            #region Configure UpdateCommand

            string commandString = "UPDATE Customers " +
                              "SET FName = @FName," +
                              "LName = @LName," +
                              "MName= @Mname," +
                              "Address1 = @Address1," +
                              "Address2 = @Address2," +
                              "City = @City," +
                              "Phone = @Phone," +
                              "DateInSystem = @DateInSystem " +
                              "WHERE CustomerNo = @CustomerNo";

            customersAdapter.UpdateCommand = new SqlCommand(commandString,
                                                            customersAdapter.SelectCommand.Connection);

            var updateParameters = customersAdapter.UpdateCommand.Parameters;
            updateParameters.Add("CustomerNo", SqlDbType.Int, 0, "CustomerNo");
            updateParameters.Add("FName", SqlDbType.NVarChar, 20, "FName");
            updateParameters.Add("LName", SqlDbType.NVarChar, 20, "Lname");
            updateParameters.Add("MName", SqlDbType.NVarChar, 20, "MName");
            updateParameters.Add("Address1", SqlDbType.NVarChar, 20, "Address1");
            updateParameters.Add("Address2", SqlDbType.NVarChar, 20, "Address2");
            updateParameters.Add("City", SqlDbType.NVarChar, 20, "City");
            updateParameters.Add("Phone", SqlDbType.NVarChar, 20, "Phone");
            updateParameters.Add("DateInSystem", SqlDbType.Date, 0, "DateInSystem");

            #endregion

            #region Configure DeleteCommand

            customersAdapter.DeleteCommand = new SqlCommand("DELETE Customers WHERE CustomerNo = @CustomerNo",
                                            customersAdapter.SelectCommand.Connection);

            var deleteParameters = customersAdapter.DeleteCommand.Parameters;
            deleteParameters.Add("@CustomerNo", SqlDbType.Int, 0, "CustomerNo");

            #endregion

            #region Configure InsertCommand

            customersAdapter.InsertCommand =
            new SqlCommand("INSERT Customers " +
                           "VALUES (@FName, @LName, @MName, @Address1, @Address2, @City, @Phone, @DateInSystem);"+
                           
                           "DECLARE @NewCustomer int; "+
                           "SET @NewCustomerNo =  @@IDENTITY;",
                           customersAdapter.SelectCommand.Connection);
            
            var insertParameters = customersAdapter.InsertCommand.Parameters;
            insertParameters.Add("FName", SqlDbType.NVarChar, 20, "FName");
            insertParameters.Add("LName", SqlDbType.NVarChar, 20, "Lname");
            insertParameters.Add("MName", SqlDbType.NVarChar, 20, "MName");
            insertParameters.Add("Address1", SqlDbType.NVarChar, 20, "Address1");
            insertParameters.Add("Address2", SqlDbType.NVarChar, 20, "Address2");
            insertParameters.Add("City", SqlDbType.NVarChar, 20, "City");
            insertParameters.Add("Phone", SqlDbType.NVarChar, 20, "Phone");
            insertParameters.Add("DateInSystem",     
                                 SqlDbType.Date,    
                                 0,                 
                                 "DateInSystem");   

            var outputParameter = insertParameters.Add("NewCustomerNo", SqlDbType.Int);
            outputParameter.Direction = ParameterDirection.Output;
            
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                adapter.Update(customers);
                MessageBox.Show("Информация успешно обновлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

       
    }
}
