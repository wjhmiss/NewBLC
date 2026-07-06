using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DNS.Utilities
{
    /// <summary>
    /// 执行命令
    /// </summary>
    public static class MachineSet
    {
        public static Focas1.focas_ret SetFocas(string ip, ushort port, int timeout, out ushort FlibHndl)
        {
            try
            {
                Focas1.focas_ret ret = (Focas1.focas_ret)Focas1.cnc_allclibhndl3(ip, port, timeout, out FlibHndl);
                return ret;
            }
            catch (Exception ex)
            {
                FlibHndl = 0;
                return Focas1.focas_ret.EW_SOCKET;
            }
        }
        public static short CloseFocas(ushort cnchandle)
        {
            return Focas1.cnc_freelibhndl(cnchandle);
        }
        public static short WriteFocas(string KeyItem, string value, ushort cnchandle)
        {
            try
            {
                string a = value.Trim();
                string s = string.Empty;
                char[] aa = a.ToCharArray();
                bool point = false;
                int K = 9;
                if (0 <= Convert.ToDouble(a) && Convert.ToDouble(a) <= 999999999)
                {
                    for (int i = 0; i < aa.Length; i++)
                    {
                        if (aa[i].ToString() == ".")
                        {
                            K = i;
                            point = true;
                        }
                        else
                        {
                            s = s + aa[i];
                        }
                    }
                    if (point == false)
                    {
                        K = aa.Length;
                    }
                    short MVariable = Convert.ToInt16(KeyItem);
                    int b = 9 - K;
                    double c = Convert.ToDouble(s);
                    c = c * System.Math.Pow(10, 9 - s.Length);
                    return Focas1.cnc_wrmacro(cnchandle, MVariable, 10, (int)c, (short)b);
                }

                else if (Convert.ToDouble(a) < 0 && Convert.ToDouble(a) > -999999999)
                {
                    for (int i = 0; i < aa.Length; i++)
                    {
                        if (aa[i].ToString() == ".")
                        {
                            K = i - 1;
                            point = true;
                        }
                        else
                        {
                            s = s + aa[i];
                        }
                    }
                    if (point == false)
                    {
                        K = aa.Length - 1;
                    }
                    short MVariable = Convert.ToInt16(KeyItem);
                    int b = 9 - K;
                    double c = Convert.ToDouble(s);
                    c = c * System.Math.Pow(10, 9 - s.Length + 1);
                    return Focas1.cnc_wrmacro(cnchandle, MVariable, 10, (int)c, (short)b);
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        public static string WriteFocas(string KeyItem, ushort cnchandle)
        {
            try
            {
                Focas1.ODBM c1 = new Focas1.ODBM();
                short ret = 0;
                double A1 = 0;
                short MVariable = (short)Convert.ToSingle(KeyItem);
                ret = Focas1.cnc_rdmacro(cnchandle, MVariable, 10, c1);
                if (ret == Focas1.EW_OK)
                {
                    A1 = c1.mcr_val / System.Math.Pow(10, c1.dec_val);
                    return A1.ToString();
                }
                return "-1";
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
    }

}
