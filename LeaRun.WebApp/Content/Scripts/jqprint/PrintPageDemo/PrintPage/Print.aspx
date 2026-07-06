<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Print.aspx.cs" Inherits="PrintPage.Print" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>无标题页</title>
    
    <div style="display:none">
	<!-- MeadCo Security Manager -->
	<object id="secmgr" style="DISPLAY: none" codeBase="smsx.cab#Version=6,4,438,06" classid="clsid:5445BE81-B796-11D2-B931-002018654E2E" viewastext> 
	<param name="GUID" value="{DDAA1B6D-0527-4C9D-87AF-4126D517E7FA}">  <!-- 用第一步产生的 GUID 替换此处{ }内的 GUID -->
	<param name="PATH" value="sxlic.mlf">
	<param name="REVISION" value="0" >
	<param name="PerUser" value="true" > 
	</object> 
	<!-- MeadCo ScriptX Factory Component -->
	<object id="factory" classid="clsid:1663ED61-23EB-11D2-B92F-008048FDD814" viewastext>
	</object>
</div>


   <script type="text/javascript">
        function SetPrintSettings() {
            // -- advanced features  ，未曾使用过，有待确认。
            factory.printing.SetMarginMeasure(2)    // measure margins in inches  
            //factory.SetPageRange(false, 1, 3)          // need pages from 1 to 3  
            factory.printing.printer = "HP DeskJet 870C"
            factory.printing.copies = 2
            factory.printing.collate = true
            factory.printing.paperSize = "A5"
            factory.printing.paperSource = "Manual feed"
            // -- basic features  使用过
            factory.printing.header = "This is MeadCo";
            factory.printing.footer = "Advanced Printing by ScriptX";
            factory.printing.portrait = false;     //方向，true
            factory.printing.leftMargin = 1.0;
            factory.printing.topMargin = 1.0;
            factory.printing.rightMargin = 1.0;
            factory.printing.bottomMargin = 1.0;

            //factory.printing.Print(false)      //直接打印，true ，好像不起作用，总是弹出选择打印机窗口
            //factory.printing.PageSetup()   //打印设置
            factory.printing.Preview();         //打印预览
        }
    </script>


</head>
<body>


<div style="text-align:center; margin-top:15px;" class="Noprint">
        <a href="javascript:SetPrintSettings()" >【 打印 】</a><br />
        <span style=" font-size:12px">（请用IE浏览器打印，纸张：A4纸、横向打印效果最佳）</span><br />
        <span style=" font-size:12px">（如果打印没有背景颜色请设置：Internet选项 -> 高级 －> 打印背景颜色 打勾）</span>
      </div>

<script type="text/javascript">
    function fun_print() {
        factory.printing.paperSize = "A5";
        factory.printing.header = "asdf";//页眉
        factory.printing.footer = "asddddd";//页脚
        factory.printing.leftMargin = 5.0;//左边距
        factory.printing.topMargin = 1.3;//上边距
        factory.printing.rightMargin = 1.0;//右边距
        factory.printing.bottomMargin = 1.3;//下边距

        factory.printing.portrait = false;//打印方向，true:纵向.false:横向
        //factory.DoPrint(false);//设置为false，直接打印
        factory.printing.Preview();
    }
    </script>
</body>
</html>
