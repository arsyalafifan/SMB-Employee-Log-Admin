using Employee_Log_Admin.Properties.DataSources;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Z.Dapper.Plus;

namespace Employee_Log_Admin
{
    public partial class ImportData : Form
    {

        public SqlConnection ConnString = new SqlConnection ("Server=DESKTOP-0IPJ4CN; Database=employee_log(development); User Id=SuperAdmin; Password=Smbntd@83212001;");
        public ImportData()
        {
            InitializeComponent();
        }

        private void cmbSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = tableCollection[cmbSheet.SelectedItem.ToString()];
            //dataGridView1.DataSource = dt;

            if(dt != null)
            {
                List<ClassImportEmployeeData> employeeDatas = new List<ClassImportEmployeeData>();
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    ClassImportEmployeeData employeeData = new ClassImportEmployeeData();
                    employeeData.Cardno = dt.Rows[i]["Cardno"].ToString();
                    employeeData.Code = dt.Rows[i]["Code"].ToString();
                    employeeData.Name = dt.Rows[i]["Name"].ToString();
                    employeeData.CompanyStructure = dt.Rows[i]["CompanyStructure"].ToString();
                    employeeData.JoinDate = dt.Rows[i]["JoinDate"].ToString();
                    employeeDatas.Add(employeeData);        
                }
                classImportEmployeeDataBindingSource.DataSource = employeeDatas;
            }
        }

        DataTableCollection tableCollection;

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Excel Workbook|*.xlsx|Excel 97-2003 Workbook|*.xls"
            }) 
            { 
                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textFileName.Text = openFileDialog.FileName;
                    using(var stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        using(IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });
                            tableCollection = result.Tables;
                            cmbSheet.Items.Clear();
                            foreach(DataTable table in tableCollection)
                            {
                                cmbSheet.Items.Add(table.TableName);
                            }
                        }
                    }
                }
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                DapperPlusManager.Entity<ClassImportEmployeeData>().Table("EmployeeData");
                List<ClassImportEmployeeData> employeeDatas = classImportEmployeeDataBindingSource.DataSource as List<ClassImportEmployeeData>;
                if(employeeDatas != null)
                {
                    string ConnEmployeeImportDataString = "Server=DESKTOP-0IPJ4CN; Database=employee_log(development); User Id=SuperAdmin; Password=Smbntd@83212001;";
                    using (IDbConnection db = new SqlConnection(ConnEmployeeImportDataString)) 
                    {
                        db.BulkInsert(employeeDatas);
                    };
                    MessageBox.Show("Employee data has successfully imported!", "Success to Import Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                string sqlNoDuplicateEmployeeData = "WITH cte AS (SELECT Cardno, Code, Name, CompanyStructure, ROW_NUMBER() OVER (PARTITION BY Cardno, Code, Name ORDER BY JoinDate) row_num FROM EmployeeData) DELETE FROM cte WHERE row_num > 1;";
                ConnString.Open();

                SqlCommand cmd = new SqlCommand(sqlNoDuplicateEmployeeData, ConnString);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ConnString.Close();
            }

        }
    }
}
