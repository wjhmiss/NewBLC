using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.MachineModule.Controllers
{
    /// <summary>
    /// 机床设置控制器
    /// </summary>
    public class BY_MachineController : PublicController<BY_Machine>
    {
        ushort cnchandle = 0;
        /// <summary>
        /// 发货单打印
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult MachineScan()
        {
            return View();
        }
        public ActionResult SubmitScan(string OrderCodeLine, string KeyValue)
        {
            string[] strtemp = OrderCodeLine.Split('-');
            if (strtemp == null || strtemp.Length != 3)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入正确的工单号+行号！" }.ToString());
            }
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@OrderCode", strtemp[0] + "-" + strtemp[1]));
            parameter.Add(DbFactory.CreateDbParameter("@LineCode", strtemp[2]));
            BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntityByWhere<BY_OrderDetail>(" and OrderCode=@OrderCode AND LineCode=@LineCode", parameter.ToArray());
            if (by_orderdetail== null || by_orderdetail.OrderDetailId == null)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入正确的工单号+行号！" }.ToString());
            }

            BY_Machine by_machine = DataFactory.Database().FindEntity<BY_Machine>(KeyValue);
            #region 机床
            
            try
            {
                //Focas1.focas_ret ret = (Focas1.focas_ret)Focas1.cnc_allclibhndl3("10.0.0.156", 8193, 10, out cnchandle);
                if (cnchandle == 0)
                {
                    Focas1.focas_ret ret = MachineSet.SetFocas(by_machine.MachineIP, 8193, 240, out cnchandle);
                    if (ret != Focas1.EW_OK)
                    {
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "连接机床失败！" }.ToString());
                    }
                }
                MachineSet.WriteFocas("515", (by_orderdetail.Weigh +5).ToString(), cnchandle);
                MachineSet.WriteFocas("516", (by_orderdetail.Length + 5).ToString(), cnchandle);
                MachineSet.WriteFocas("517", by_orderdetail.Weigh.ToString(), cnchandle);
                MachineSet.WriteFocas("518", by_orderdetail.Length.ToString(), cnchandle);
                MachineSet.WriteFocas("535", "0", cnchandle);
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "连接机床失败！" }.ToString());
            }
            finally
            {
                //MachineSet.CloseFocas(cnchandle);
            }
            #endregion 机床

            return Content(new JsonMessage { Success = false, Code = "1", Message = "发送成功！" }.ToString());

        }
    }
}