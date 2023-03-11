using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Employee_Log_Admin
{
    public partial class viewData : Form
    {
        string connStr = "Server=10.8.137.195; Database=employee_log; User Id=SuperAdmin; Password=Smbntd@83212001;";
        SqlConnection conn;
        SqlDataAdapter da;
        DataTable dt;
        public viewData()
        {
            InitializeComponent();
        }

        private void viewData_Load(object sender, EventArgs e)
        {
            conn = new SqlConnection(connStr);
            conn.Open();
            da = new SqlDataAdapter("SELECT * FROM EmployeeData", conn);
            dt = new DataTable();

            da.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();

            // SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM EmployeeData", ConnString);
            //DataSet ds = new DataSet();
            //da.Fill(ds);
            //dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }

        private void txtSearchName_TextChanged(object sender, EventArgs e)
        {
            //SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM EmployeeData WHERE Name like '" +txtSearchName+ "%'", "Server=10.8.137.195; Database=employee_log; User Id=SuperAdmin; Password=Smbntd@83212001;");
            // SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM EmployeeData", ConnString);
            //DataSet ds = new DataSet();
            //dt = new DataTable();

            //da.Fill(dt);
            //dataGridView1.DataSource = dt;

            conn = new SqlConnection(connStr);
            conn.Open();
            da = new SqlDataAdapter("SELECT * FROM EmployeeData WHERE Name like '"+txtSearchName.Text+"%'", conn);
            dt = new DataTable();

            da.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }
    }
}

