using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace DNS.Utilities
{
    /// <summary>
    /// 导入Excel帮助类
    /// </summary>
    public class ImportExcel
    {
        /// <summary>
        /// Excel检查版本
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string ConnectionString(string fileName)
        {
            bool isExcel2003 = fileName.EndsWith(".xls");
            string connectionString = string.Format(
                isExcel2003
                    ? "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;"
                    : "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\"",
                fileName);
            return connectionString;
        }
        /// <summary>
        /// Excel导入数据源
        /// </summary>
        /// <param name="sheet">sheet</param>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string sheet, string filename)
        {
            OleDbConnection myConn = new OleDbConnection(ConnectionString(filename));
            try
            {
                DataSet ds;
                string strCom = " SELECT * FROM [Sheet1$]";
                myConn.Open();
                OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);
                ds = new DataSet();
                myCommand.Fill(ds);
                myConn.Close();
                return ds.Tables[0];
            }
            catch (Exception)
            {
                myConn.Close();
                myConn.Dispose();
                throw;
            }
        }

        public static DataTable ExcelToDataTableNPOI(string filename)
        {
            HSSFWorkbook hssfworkbook;
            try
            {
                using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                Sheet sheet = hssfworkbook.GetSheetAt(0);
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                DataTable dt = new DataTable();
                for (int j = 0; j < 27; j++)
                {
                    dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
                }
                while (rows.MoveNext())
                {
                    Row row = (HSSFRow)rows.Current;
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        Cell cell = row.GetCell(i);
                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString().Replace(" ", "").Replace("	", "");
                        }
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
    }
}
