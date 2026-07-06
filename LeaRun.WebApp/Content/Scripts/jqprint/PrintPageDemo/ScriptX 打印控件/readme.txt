Asp.Net(IE)  ScriptX/smsx.cab 打印控制插件 如何使用详细教程

注：该插件只限IE使用。

当前示例版本：ScriptX version 6,6,440,26 
Scriptx 描叙：http://www.meadroid.com/index.asp
官网帮助文档： http://www.meadroid.com/scriptx/docs/printdoc.asp#intro
官网下载地址： http://www.meadroid.com/scriptx/sxdownload.asp


第一步：
  将smsx.cab 复制到.net工程中，如send_js 文件夹中。
  
第二步：
  将下面这段代码放入body中：
<object id="factory" viewastext style="display:none"
classid="clsid:1663ed61-23eb-11d2-b92f-008048fdd814"
  codebase="send_js/smsx.cab#Version=6,6,440,26">
</object>
    注意 codebase 属性，它是指向smsx.cab文件的路径和版本号

第三步：
  设置样式在head标签下加入如下样式：
<style media="print" type="text/css">
    .Noprint{display:none;}
    .PageNext{page-break-after: always;}
    *{word-wrap: break-word; word-break: break-all;}
  </style>
    注意 media="print" 这个属性是说在打印时有效
  样式 Noprint：该样式的作用是在打印时隐藏样式标签（不在打印页显示该样式引用标签），如打印时，不需要打印，打印按钮，可将打印按钮加入样式 class="Noprint"
  样式 PageNext：该样式的作用是分页设置，如在需要分页的地方加入这样一段 <div class="PageNext"></div>  后，打印时就会自动在这个地方分页。
  样式 * 该样式的作用是文字自动换行。包括table、div等。
  
第四步：
  添加打印按钮：如下代码：
<div style="text-align:center; margin-top:15px;" class="Noprint">
        <a href="javascript:fun_print()" >【 打印 】</a><br />
        <span style=" font-size:12px">（请用IE浏览器打印，纸张：A4纸、横向打印效果最佳）</span><br />
        <span style=" font-size:12px">（如果打印没有背景颜色请设置：Internet选项 -> 高级 －> 打印背景颜色 打勾）</span>
      </div>
   
第五步：
  添加javascript脚本，如下代码：
  

<script type="text/javascript">
        function fun_print() {
            factory.printing.header = "";//页眉
            factory.printing.footer = "";//页脚
            factory.printing.leftMargin = 1.0;//左边距
            factory.printing.topMargin = 1.3;//上边距
            factory.printing.rightMargin = 1.0;//右边距
            factory.printing.bottomMargin = 1.3;//下边距
            
            factory.printing.portrait = false;//打印方向，true:纵向.false:横向
            factory.DoPrint(false);//设置为false，直接打印
        }
    </script>

 

 
