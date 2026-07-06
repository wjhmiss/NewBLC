using DNS.Business;
using DNS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Web;

namespace DNS.WebApp
{
    /// <summary>
    /// 通用方法
    /// </summary>
    public class BaseCommon
    {
        /// <summary>
        /// 拼接表单（返回html）
        /// </summary>
        /// <param name="ColumnCount"></param>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public static string CreateBuildForm(int ColumnCount)
        {
            string ModuleId = DESEncrypt.Decrypt(CookieHelper.GetCookie("ModuleId"));
            Base_FormAttributeBll base_formattributebll = new Base_FormAttributeBll();
            return base_formattributebll.CreateBuildFormTable(ColumnCount, ModuleId);
        }

        public static string IsRegistration()
        {
            try
            {
                if (DateTime.Now < Convert.ToDateTime("2015-05-20"))
                {
                    return "1990-01-01";
                }
                if (HttpContext.Current.Application["MacInfo"].ToString() == "0")
                {
                    return "1990-01-01";
                }
                if (ConfigHelper.AppSettings("MacInfo") == "")
                {
                    return "1990-01-01";
                }
                string temp = DESEncrypt.Decrypt(ConfigHelper.AppSettings("MacInfo"));
                //if (CheckRegistration(temp.Substring(0,temp.Length - 11)))
                //{
                if (temp.Substring(temp.Length - 11, 10) == "9999-99-99")
                {
                    return "Registration";
                }
                //判断异常，最多试用60天
                if (CommonHelper.TrytoDate(temp.Substring(temp.Length - 11, 10)))
                {
                    if (DateTime.Now.AddDays(60) < Convert.ToDateTime(temp.Substring(temp.Length - 11, 10)))
                    {
                        return "1990-01-01";
                    }
                }
                if (CommonHelper.TrytoDate(temp.Substring(temp.Length - 11, 10)))
                {
                    return temp.Substring(temp.Length - 11, 10);
                }
                //}

                return "1990-01-01";
            }
            catch (Exception ex)
            {
                return "1990-01-01";
            }

        }

        public static string GetInfo()
        {

            string cpuInfo = "";//cpu序列号   

            ManagementClass cimobject = new ManagementClass("Win32_Processor");

            ManagementObjectCollection moc = cimobject.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
            }

            //获取硬盘ID   
            string HDid = string.Empty;
            ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc1 = cimobject1.GetInstances();
            foreach (ManagementObject mo in moc1)
            {
                HDid = (string)mo.Properties["Model"].Value;
            }
            //string MAC = string.Empty;
            //ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //ManagementObjectCollection moc2 = mc.GetInstances();
            //foreach (ManagementObject mo in moc2)
            //{
            //    if ((bool)mo["IPEnabled"] == true)

            //        MAC = mo["MacAddress"].ToString();

            //    mo.Dispose();

            //}
            return (cpuInfo + HDid).Replace(" ", "").Replace("-", "").Replace(":", "").Replace("@", "").Replace("&", "").Replace(".", "").Replace("?", "").Replace(",", "").Replace("*", "").Replace("%", "").Replace("^", "").Replace(",", "");
        }

        public static bool CheckRegistration(string macinfo)
        {
            List<string> Registrations = new List<string>();
            Registrations.Add("BFEBFBFF000306A9ST500LM012HNM500MBB0CD292A778D0");
            return Registrations.Contains(macinfo);
        }


    }
}