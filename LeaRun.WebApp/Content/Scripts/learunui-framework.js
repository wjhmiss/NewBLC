/**
 * jQuery LeaRunUI 4.1
 *北仑机械锻造厂 Copyright © Learun 2014
 */
//样式
var temp = 1;
var tempwidth;
var tempheight;
$(function () {
    //回车模拟tab键
    //$("body").live("keydown", function (e) {
    //    var e = e ? e : window.event;
    //    var tar = e.srcElement || e.target;
    //    if (e.which == 13) {
    //        if (!$(tar).hasClass("BarCode")) {
    //            window.event.keyCode = 9;
    //        }
    //    }
    //})
    //document.onkeydown=function(event){            
    //    var e = event || window.event || arguments.callee.caller.arguments[0];
    //    var tar = e.srcElement || e.target;
    //    if (e && e.keyCode == 13) {
    //        if (!$(tar).hasClass("BarCode")) {
    //            window.event.keyCode = 9;
    //        }
    //        var btlogin = document.getElementById("btlogin");
    //        if (btlogin) {
    //            btlogin.focus();
    //            btlogin.click();
    //        }
    //        var btnSearch = document.getElementById("btnSearch");
    //        if (btnSearch) {
    //            btnSearch.focus();
    //            btnSearch.click();
    //        }
            
    //    }
    //};
    //if (browserRedirect()) {
    //    location.href = '/WeiWebSite/WeiLogin/WeiIndex';
    //}
    //禁止后退键 作用于Firefox、Opera  
    document.onkeypress = banBackSpace;
    //禁止后退键  作用于IE、Chrome  
    document.onkeydown = banBackSpace;

    readyIndex();

})
function ToNumber(obj) {
    //1.首先动态创建一个容器标签元素，如DIV
    if (isNaN(obj)) {
        return 0;
    }
    else {
        return Number(obj);
    }
}
//var html = " <div>'我们在哪里'</div> "
//var enCodehtml = HtmlUtil.htmlEncode(html);
//console.log(enCodehtml); // &lt;div&gt;'我们在哪里'&lt;/div&gt; 

//// 用浏览器内部转换器实现html解码
//var decodeHtml = HtmlUtil.htmlDecode(enCodehtml);
//console.log(decodeHtml); // <div>'我们在哪里'</div> 
var HtmlUtil = {
    /*1.用浏览器内部转换器实现html转码*/
    htmlEncode: function (html) {
        //1.首先动态创建一个容器标签元素，如DIV
        var temp = document.createElement("div");
        //2.然后将要转换的字符串设置为这个元素的innerText(ie支持)或者textContent(旧版火狐，google支持)
        (temp.textContent != undefined) ? (temp.textContent = html) : (temp.innerText = html);
        //3.最后返回这个元素的innerHTML，即得到经过HTML编码转换的字符串了
        var output = temp.innerHTML;
        temp = null;
        return output;
    },
    /*2.用浏览器内部转换器实现html解码*/
    htmlDecode: function (text) {
        //1.首先动态创建一个容器标签元素，如DIV
        var temp = document.createElement("div");
        //2.然后将要转换的字符串设置为这个元素的innerHTML(ie，火狐，google都支持)
        temp.innerHTML = text;
        //3.最后返回这个元素的innerText(ie支持)或者textContent(火狐，google支持)，即得到经过HTML解码的字符串了。
        var output = temp.innerText || temp.textContent;
        temp = null;
        return output;
    },
    /*3.用正则表达式实现html转码*/
    htmlEncodeByRegExp: function (str) {
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&/g, "&amp;");
        s = s.replace(/</g, "&lt;");
        s = s.replace(/>/g, "&gt;");
        s = s.replace(/\s/g, "&nbsp;");
        s = s.replace(/\'/g, "&#39;");
        s = s.replace(/\"/g, "&quot;");
        return s;
    },
    /*4.用正则表达式实现html解码*/
    htmlDecodeByRegExp: function (str) {
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&amp;/g, "&");
        s = s.replace(/&lt;/g, "<");
        s = s.replace(/&gt;/g, ">");
        s = s.replace(/&nbsp;/g, " ");
        s = s.replace(/&#39;/g, "\'");
        s = s.replace(/&quot;/g, "\"");
        return s;
    }
};


//检测是否移动设备来访
function browserRedirect() { 
    var sUserAgent= navigator.userAgent.toLowerCase(); 
    var bIsIpad= sUserAgent.match(/ipad/i) == "ipad"; 
    var bIsIphoneOs= sUserAgent.match(/iphone os/i) == "iphone os"; 
    var bIsMidp= sUserAgent.match(/midp/i) == "midp"; 
    var bIsUc7= sUserAgent.match(/rv:1.2.3.4/i) == "rv:1.2.3.4"; 
    var bIsUc= sUserAgent.match(/ucweb/i) == "ucweb"; 
    var bIsAndroid= sUserAgent.match(/android/i) == "android"; 
    var bIsCE= sUserAgent.match(/windows ce/i) == "windows ce"; 
    var bIsWM= sUserAgent.match(/windows mobile/i) == "windows mobile"; 
    if (bIsIpad || bIsIphoneOs || bIsMidp || bIsUc7 || bIsUc || bIsAndroid || bIsCE || bIsWM) {
        var useragent = navigator.userAgent;
        if (useragent.match(/MicroMessenger/i) != 'MicroMessenger') {
            // 这里警告框会阻塞当前页面继续加载
            //alert('已禁止本次访问：您必须使用微信访问！');
            // 以下代码是用javascript强行关闭当前页面
            var opened = window.open('about:blank', '_self');
            opened.opener = null;
            opened.close();
        }

        return true;
    } else { 
        return false;
    } 
}
//处理键盘事件 禁止后退键（Backspace）密码或单行、多行文本框除外  
function banBackSpace(e) {
    var e = e || window.event || arguments.callee.caller.arguments[0];
    var tar = e.srcElement || e.target;
    if (e && e.keyCode == 119) {
        if ($("#tabs_iframe_8f1ac470-896c-46f8-af97-5176fe716491").contents().find("#lr-copy-Row").length > 0) {
            var rowcount = $("#tabs_iframe_8f1ac470-896c-46f8-af97-5176fe716491").contents().find("#gridTable").getGridParam("reccount");
            $("#tabs_iframe_8f1ac470-896c-46f8-af97-5176fe716491").contents().find("#gridTable").jqGrid('setSelection', rowcount);
            $("#tabs_iframe_8f1ac470-896c-46f8-af97-5176fe716491").contents().find("#lr-copy-Row").click();//jquery 方法1   
        }

    }

    var ev = e || window.event;//获取event对象     
    var obj = ev.target || ev.srcElement;//获取事件源     

    var t = obj.type || obj.getAttribute('type');//获取事件源类型    

    //获取作为判断条件的事件类型  
    var vReadOnly = obj.getAttribute('readonly');
    var vEnabled = obj.getAttribute('enabled');
    //处理null值情况  
    vReadOnly = (vReadOnly == null) ? false : vReadOnly;
    vEnabled = (vEnabled == null) ? true : vEnabled;

    //当敲Backspace键时，事件源类型为密码或单行、多行文本的，  
    //并且readonly属性为true或enabled属性为false的，则退格键失效  
    var flag1 = (ev.keyCode == 8 && (t == "password" || t == "text" || t == "textarea")
                && (vReadOnly == true || vEnabled != true)) ? true : false;

    //当敲Backspace键时，事件源类型非密码或单行、多行文本的，则退格键失效  
    var flag2 = (ev.keyCode == 8 && t != "password" && t != "text" && t != "textarea")
                ? true : false;

    //判断  
    if (flag2) {
        return false;
    }
    if (flag1) {
        return false;
    }

  
    //if (window.event.keyCode == 13 && window.event.ctrlKey) {
    //    window.event.keyCode = 13;
    //    return;
    //}
    if (e && e.keyCode == 13) {
        //if (!$(tar).hasClass("BarCode")) {
        //    window.event.keyCode = 9;
        //    return;
        //}
        
        var btnSearch = document.getElementById("btnSearch");
        if (btnSearch) {
            btnSearch.focus();
            btnSearch.click();
        }

    }


}


function readyIndex() {
    $(".main_menu li div").click(function () {
        $(".main_menu li div").removeClass('main_menu leftselected');
        $(this).addClass('main_menu leftselected');
    }).hover(function () {
        $(this).addClass("hoverleftselected");
    }, function () {
        $(this).removeClass("hoverleftselected");
    });
}
/**
加载布局
**/
function Loadlayout() {
    if ($('.layout').length > 0) {
        $("#layout").splitter({
            type: "v",
            outline: true,
            minLeft: 150, sizeLeft: 200, maxLeft: 350,
            anchorToWindow: true,
            accessKey: "L"
        });
    }
}
//Tab标签切换
function Tabchange(id) {
    $('.ScrollBar').find('.tabPanel').hide();
    $('.ScrollBar').find("#" + id).show();
    $(".tab_list_top div").removeClass("actived");
    $(".tab_list_top").find("#Tab" + id).addClass("actived"); //添加选中样式  
}

/********
接收地址栏参数
**********/
function GetQuery(key) {
    var search = location.search.slice(1); //得到get方式提交的查询字符串
    var arr = search.split("&");
    for (var i = 0; i < arr.length; i++) {
        var ar = arr[i].split("=");
        if (ar[0] == key) {
            if (unescape(ar[1]) == 'undefined') {
                return "";
            } else {
                return unescape(ar[1]);
            }
        }
    }
    return "";
}
/*
获取动态tab标签当前iframeID
*/
function tabiframeId() {
    var tabs_container = top.$("#tabs_container");
    return "tabs_iframe_" + tabs_container.find('.selected').attr('id').substr(5);
}
//关闭当前tab
function btn_back() {
    top.ThisCloseTab();
}
/*
中间加载对话窗
*/
function Loading(bool, text) {
    var ajaxbg = top.$("#loading_background,#loading");
    if (!!text) {
        top.$("#loading").css("left", (top.$('body').width() - top.$("#loading").width()) / 2);
        top.$("#loading span").html(text);
    } else {
        top.$("#loading").css("left", "42%");
        top.$("#loading span").html("正在拼了命为您加载…");
    }
    if (bool) {
        ajaxbg.show();
    } else {
        ajaxbg.hide();
    }
}
/* 
请求Ajax 带返回值
*/
function getAjax(url, postData, callBack) {
    $.ajax({
        type: 'post',
        dataType: "text",
        url: RootPath() + url,
        data: postData,
        cache: false,
        async: false,
        success: function (data) {
            callBack(data);
            //Loading(false);
        },
        error: function (data) {
            alert("error:" + JSON.stringify(data));
            Loading(false);
        }
    });
}
function AjaxJson(url, postData, callBack) {
    try {
        $.ajax({
            url: RootPath() + url,
            type: "post",
            data: postData,
            dataType: "json",
            async: false,
            success: function (data) {
                if (data.Code == "-1") {
                    Loading(false);
                    alertDialog(data.Message, -1);
                } else {
                    Loading(false);
                    callBack(data);
                }
            },
            error: function (data) {
                Loading(false);
                alertDialog(data.responseText, -1);
            }
        });
    } catch (e) {

    }
}


function AjaxJsonAsync(url, postData, callBack) {
    try {
        $.ajax({
            url: RootPath() + url,
            timeout: 20 * 1000,
            type: "post",
            data: postData,
            dataType: "json",
            async: true,
            success: function (data) {
                if (data.Code == "-1") {
                    Loading(false);
                    alertDialog(data.Message, -1);
                } else {
                    Loading(false);
                    callBack(data);
                }
            },
            error: function (data) {
                Loading(false);
                alertDialog(data.responseText, -1);
            }
        });
    } catch (e) {

    }
}


//调用confirmDialog时，弹出确认窗口
function AjaxJsonNoMassge(url, postData, callBack) {
    try {
        $.ajax({
            url: RootPath() + url,
            type: "post",
            data: postData,
            dataType: "json",
            async: false,
            success: function (data) {
                    
                    callBack(data);
            },
            error: function (data) {
                
                alert(data.responseText, -1);
            }
        });
        
    } catch (e) {

    }
}
/*
刷新当前页面
*/
function Replace() {
    location.reload();
    return false;
}
/*
href跳转页面
*/
function Urlhref(url) {
    location.href = url;
    return false;
}
/*
iframe同步连接
*/
function iframe_src(iframe, src) {
    Loading(true);
    $("#" + iframe).attr('src', RootPath() + src);
    $("#" + iframe).load(function () {
        Loading(false);
    });
}
/*
grid表格扩展Begin
*/

/**获取表格选择行
**/
function GetJqGridRowIndx(jgrid) {
    return $(jgrid).jqGrid('getGridParam', 'selrow')
}
/**获取JqGrid表格列值
jgrid：ID，code：列字段
**/
function GetJqGridRowValue(jgrid, code) {
    var KeyValue = "";
    var selectedRowIds = $(jgrid).jqGrid("getGridParam", "selarrrow");
    if (selectedRowIds != "") {
        var len = selectedRowIds.length;
        for (var i = 0; i < len ; i++) {
            var rowData = $(jgrid).jqGrid('getRowData', selectedRowIds[i]);
            KeyValue += rowData[code] + ",";
        }
        KeyValue = KeyValue.substr(0, KeyValue.length - 1);
    } else {
        var rowData = $(jgrid).jqGrid('getRowData', $(jgrid).jqGrid('getGridParam', 'selrow'));
        KeyValue = rowData[code];
    }
    return KeyValue;
}

function SumGetJqGridRowValue(jgrid, code) {
    var KeyValue = "";
    var selectedRowIds = $(jgrid).jqGrid("getGridParam", "selarrrow");
    if (selectedRowIds != "") {
        var len = selectedRowIds.length;
        for (var i = 0; i < len ; i++) {
            var rowData = $(jgrid).jqGrid('getRowData', selectedRowIds[i]);
            KeyValue = Number(KeyValue) + Number(rowData[code]);
        }
    } else {
        var rowData = $(jgrid).jqGrid('getRowData', $(jgrid).jqGrid('getGridParam', 'selrow'));
        KeyValue = Number(rowData[code]);
    }
    return KeyValue;
}
/**获取JqGrid表格列值
jgrid：ID，RowIndx:行ID,code：列字段
**/
function GetJqGridValue(jgrid, RowIndx, code) {
    var rowData = $(jgrid).jqGrid('getRowData', RowIndx);
    return rowData[code];
}

//公共调用方法
function Merger(gridName, CellName) {
    //得到显示到界面的id集合
    var mya = $("#" + gridName + "").getDataIDs();
    //当前显示多少条
    var length = mya.length;
    for (var i = 0; i < length; i++) {
        //从上到下获取一条信息
        var before = $("#" + gridName + "").jqGrid('getRowData', mya[i]);
        //定义合并行数
        var rowSpanTaxCount = 1;
        for (j = i + 1; j <= length; j++) {
            //和上边的信息对比 如果值一样就合并行数+1 然后设置rowspan 让当前单元格隐藏
            var end = $("#" + gridName + "").jqGrid('getRowData', mya[j]);
            if (before[CellName] == end[CellName]) {
                rowSpanTaxCount++;
                $("#" + gridName + "").setCell(mya[j], CellName, '', { display: 'none' });
            } else {
                rowSpanTaxCount = 1;
                break;
            }
            $("#" + CellName + "" + mya[i] + "").attr("rowspan", rowSpanTaxCount);
        }
    }
}
/**grid表格扩展end**/

function isTimeString(str) {
    // 示例：YYYY-MM-DD
    const regex = /^\d{4}-\d{2}-\d{2}$/;
    // 示例：yyyy-MM-dd HH:mm:ss
    // const regex = /^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$/;
    return regex.test(str);
}

/**
格式化时间显示方式、用法:format="yyyy-MM-dd HH:mm:ss";
*/
formatDate = function (v, format) {
    if (!v) return "";
    var d = v;
    if (typeof v === 'string') {
        if (v.indexOf("/Date(") > -1)
            d = new Date(parseInt(v.replace("/Date(", "").replace(")/", ""), 10));
        else
            d = new Date(Date.parse(v.replace(/-/g, "/").replace("T", " ").split(".")[0]));//.split(".")[0] 用来处理出现毫秒的情况，截取掉.xxx，否则会出错
    }
    var o = {
        "M+": d.getMonth() + 1,  //month
        "d+": d.getDate(),       //day
        "h+": d.getHours(),      //hour
        "m+": d.getMinutes(),    //minute
        "s+": d.getSeconds(),    //second
        "q+": Math.floor((d.getMonth() + 3) / 3),  //quarter
        "S": d.getMilliseconds() //millisecond
    };
    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (d.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
};
/**
当前时间
*/
function CurrentTime() {
    var date = new Date();
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hour = date.getHours();
    var minute = date.getMinutes();
    var second = date.getSeconds();
    return year + '-' + month + '-' + day + ' ' + hour + ':' + minute;
}
/*
自动获取页面控件值
*/
function GetWebControls(element, isBlank) {
    var reVal = "";
    $(element).find('input,select,textarea').each(function (r) {
        var id = $(this).attr('id');
        var value = $(this).val();
        var type = $(this).attr('type');
        switch (type) {
            case "checkbox":
                if ($(this).attr("checked")) {
                    reVal += '"' + id + '"' + ':' + '"1",'
                } else {
                    reVal += '"' + id + '"' + ':' + '"0",'
                }
                break;
            case "radio":
                if ($(this).attr("checked")) {
                    reVal += '"' + $(this).attr("name") + '"' + ':' + '"' + $(this).val().trim() + '",'
                }
                break;

            default:
                if (value == "" && ((isBlank == undefined || isBlank == "" || isBlank == 'null' || isBlank == 'undefined'))) {
                    value = "&nbsp;";
                }
                reVal += '"' + id + '"' + ':' + '"' + $.trim(valueReplaceSave(value)) + '",'
                break;
        }
    });
    reVal = reVal.substr(0, reVal.length - 1);
    return JSON.parse('{' + reVal + '}');
}
function GetWebControlsforCustemar(element, isBlank) {
    var reVal = "";
    $(element).find('input,select,textarea').each(function (r) {
        var id = $(this).attr('id');
        var value = $(this).val();
        var type = $(this).attr('type');
        switch (type) {
            case "checkbox":
                if ($(this).attr("checked")) {
                    reVal += '"' + id + '"' + ':' + '"1",'
                } else {
                    reVal += '"' + id + '"' + ':' + '"0",'
                }
                break;
            case "radio":
                if ($(this).attr("checked")) {
                    reVal += '"' + $(this).attr("name") + '"' + ':' + '"' + $(this).val().trim() + '",'
                }
                break;

            default:
                if (value == "" && ((isBlank == undefined || isBlank == "" || isBlank == 'null' || isBlank == 'undefined'))) {
                    value = "&nbsp;";
                }
                reVal += '"' + id + '"' + ':' + '"' + valueReplaceSave(value) + '",'
                break;
        }
    });
    reVal = reVal.substr(0, reVal.length - 1);
    return JSON.parse('{' + reVal + '}');
}

function valueReplaceSave(v) {
    //v = v.toString().replace(new RegExp('(["\"])', 'g'), "\\\"");
    //v = "               毛料         光\板 \nA3(A)          5.60/KG     8.00/KG（厚20~60MM）\n45#(A)         6.00KG      8.30/KG（厚20~60MM）\nCR12(R)       15.00/KG     19.00/KG（厚20~60MM)\nCR12MOV（W）               28.00\n718           21.0/KG      25.0/KG（厚20~90MM）\n718H          23.0/KG      27.0/KG（厚20~90MM）\nSKD11\\D2(S)   40.0/KG      43.0/KG\nD2 (W)        26.00/Kg     30.00/kg\nSKD61(E)      33.0/KG      39.0/KG(黄石)\nSKD61(F)      48.0/KG      52.0/KG(上五钢)\n\n弹簧钢：      22.00/KG     35.00/KG\nDC53(DCS):    65           65\n来料研磨:250一平方,收低收费15.00/件\n备注：\n（1）：精板单价=光板单价+1元/KG\n\n（3）：精板不足5.00KG/件另收精板加工费15.0/件";
    if (!!v) {
        v = v.replace(/	/g, " ");
        v = v.replace(/"/g, "“");
        v = v.replace(/'/g, "“");
        v = v.replace(/\n/g, "@@");
        //v = v.replace(/</g, "(");
        //v = v.replace(/>/g, ")");
        v = v.replace(/\\/g, "/");
        v = v.replace(/,/g, "，");
        //v = v.replace(/:/g, "：");
    }
    return v;
}
function valueReplaceShow(v) {
    //v = v.toString().replace(new RegExp('(["\"])', 'g'), "\\\"");
    //v = "               毛料         光\板 \nA3(A)          5.60/KG     8.00/KG（厚20~60MM）\n45#(A)         6.00KG      8.30/KG（厚20~60MM）\nCR12(R)       15.00/KG     19.00/KG（厚20~60MM)\nCR12MOV（W）               28.00\n718           21.0/KG      25.0/KG（厚20~90MM）\n718H          23.0/KG      27.0/KG（厚20~90MM）\nSKD11\\D2(S)   40.0/KG      43.0/KG\nD2 (W)        26.00/Kg     30.00/kg\nSKD61(E)      33.0/KG      39.0/KG(黄石)\nSKD61(F)      48.0/KG      52.0/KG(上五钢)\n\n弹簧钢：      22.00/KG     35.00/KG\nDC53(DCS):    65           65\n来料研磨:250一平方,收低收费15.00/件\n备注：\n（1）：精板单价=光板单价+1元/KG\n\n（3）：精板不足5.00KG/件另收精板加工费15.0/件";
    if (!!v) {
        v = v.replace(/	/g, " ");
        v = v.replace(/"/g, "“");
        v = v.replace(/'/g, "“");
        //v = v.replace(/</g, "(");
        //v = v.replace(/>/g, ")");
        v = v.replace(/\\/g, "/");
        v = v.replace(/@@/g, "\n");
        v = v.replace(/,/g, "，");
        //v = v.replace(/:/g, "：");
    }
    return v;
}


/*
自动给控件赋值
*/
function SetWebControls(data) {
    for (var key in data) {
        var id = $('#' + key);
        var value = $.trim(data[key]).replace("&nbsp;", "");
        var type = id.attr('type');
        switch (type) {
            case "checkbox":
                if (value == 1) {
                    id.attr("checked", 'checked');
                } else {
                    id.removeAttr("checked");
                }
                $('input').customInput();
                break;
            
            case "text":
                id.val(valueReplaceShow(value));
                break;
            default:
                if ($("input[name='" + key + "']").length > 0) {
                    $("input[name='" + key + "'][value=" + value + "]").prop("checked", true);
                }

                if (id[0] && id[0].tagName.toUpperCase() == "LABEL") {
                    id.html(valueReplaceShow(value));
                } 
                else {
                    id.val(valueReplaceShow(value));
                }

                //if (id.attr('class')) {
                //    if (id.attr('class').indexOf("select") >= 0) {
                //        id.val(value);
                //        break;
                //    }
                //}
                
                break;
        }
    }
}
/*
自动给控件赋值、对Lable
*/
function SetWebLable(data) {
    for (var key in data) {
        var id = $('#' + key);
        var value = $.trim(data[key]).replace("&nbsp;", "");
        id.text(value);
    }
}
/**
* 获取搜索条件：返回JSON
* var parameter = GetParameterJson("搜索区域table的ID");
*/
function GetParameterJson(tableId) {
    var parameter = "";
    $("#" + tableId + " tr").find('input,select').each(function () {
        var id = $(this).attr('id');
        var value = $(this).val();
        var type = $(this).attr('type');
        if (type == "checkbox") {
            if ($(this).attr("checked")) {
                parameter += '"' + id + '"' + ':' + '"1",'
            } else {
                parameter += '"' + id + '"' + ':' + '"",'
            }
        } else {
            parameter += '"' + id + '"' + ':' + '"' + $.trim(value) + '",'
        }
    });
    parameter = parameter.substr(0, parameter.length - 1);
    return '{' + parameter + '}';
}
/**
* 获取动态table：键、值，返回JSON
* var GetTableData = GetTableDataJson("table的ID");
*/
function GetTableDataJson(tableId) {
    var item_Key_Value = "";
    var index = 1;
    var trjson = "";
    if ($(tableId + " tbody tr").length > 0) {
        $(tableId + " tbody tr").each(function () {
            var tdjson = "";
            $(this).find('td').find('input,select,textarea').each(function () {
                var pk_id = $(this).attr('id');
                var pk_value = "";
                if ($("#" + pk_id).attr('type') == "checkbox") {
                    if ($("#" + pk_id).attr("checked")) {
                        pk_value = "1";
                    } else {
                        pk_value = "0";
                    }
                } else {
                    pk_value = $("#" + pk_id).val();
                }
                var array = new Array();
                array = pk_id.split("➩"); //字符分割
                tdjson += '"' + array[0] + '"' + ':' + '"' + $.trim(pk_value) + '",'
            })
            tdjson = tdjson.substr(0, tdjson.length - 1);
            trjson += '{' + tdjson + '},';
        });
    } else {
        $(tableId + " tr").each(function () {
            var tdjson = "";
            $(this).find('td').find('input,select,textarea').each(function () {
                var pk_id = $(this).attr('id');
                var pk_value = "";
                if ($("#" + pk_id).attr('type') == "checkbox") {
                    if ($("#" + pk_id).attr("checked")) {
                        pk_value = "1";
                    } else {
                        pk_value = "0";
                    }
                } else {
                    pk_value = $("#" + pk_id).val();
                }
                var array = new Array();
                array = pk_id.split("➩"); //字符分割
                tdjson += '"' + array[0] + '"' + ':' + '"' + $.trim(pk_value) + '",'
            })
            tdjson = tdjson.substr(0, tdjson.length - 1);
            trjson += '{' + tdjson + '},';
        });
    }
    trjson = trjson.substr(0, trjson.length - 1);
    if (trjson == '{}') {
        trjson = "";
    }
    return '[' + trjson + ']';
}
/**
获取选中复选框值
值：1,2,3,4
**/
function CheckboxValue() {
    var reVal = '';
    $('[type = checkbox]').each(function () {
        if ($(this).attr("checked")) {
            reVal += $(this).val() + ",";
        }
    });
    reVal = reVal.substr(0, reVal.length - 1);
    return reVal;
}
/**
文本框只允许输入数字
**/
function IsNumber(obj) {
    $("#" + obj).bind("contextmenu", function () {
        return false;
    });
    $("#" + obj).css('ime-mode', 'disabled');
    $("#" + obj).keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });
}
/**
只能输入数字和小数点
**/
function IsMoney(obj) {

    console.log(obj);
    $("#" + obj).bind("contextmenu", function () {
        return false;
    });
    $("#" + obj).css('ime-mode', 'disabled');
    $("#" + obj).bind("keydown", function (e) {
        var key = window.event ? e.keyCode : e.which;
        if (isFullStop(key)) {
            return $(this).val().indexOf('.') < 0;
        }
        return (isSpecialKey(key)) || ((isNumber(key) && !e.shiftKey)) || ((isSmallKeyBoard(key) && !e.shiftKey));
    });
    function isNumber(key) {
        return key >= 48 && key <= 57
    }
    function isSpecialKey(key) {
        return key == 8 || key == 46 || (key >= 37 && key <= 40) || key == 35 || key == 36 || key == 9 || key == 13
    }
    function isSmallKeyBoard(key) {
        return key >= 96 && key <= 105
    }
    function isFullStop(key) {
        return key == 190 || key == 110;
    }
}
function IsMoney2(obj) {
    var $myDiv = $(obj);
    $myDiv.bind("contextmenu", function () {
        return false;
    });
    $myDiv.css('ime-mode', 'disabled');
    $myDiv.bind("keydown", function (e) {
        var key = window.event ? e.keyCode : e.which;
        if (isFullStop(key)) {
            return $(this).val().indexOf('.') < 0;
        }
        return (isSpecialKey(key)) || ((isNumber(key) && !e.shiftKey)) || ((isSmallKeyBoard(key) && !e.shiftKey));
    });
    function isNumber(key) {
        return key >= 48 && key <= 57
    }
    function isSpecialKey(key) {
        return key == 8 || key == 46 || (key >= 37 && key <= 40) || key == 35 || key == 36 || key == 9 || key == 13
    }
    function isSmallKeyBoard(key) {
        return key >= 96 && key <= 105
    }
    function isFullStop(key) {
        return key == 190 || key == 110;
    }
}
/**
* 金额格式(保留2位小数)后格式化成金额形式
*/
function FormatCurrency(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
        num = num.substring(0, num.length - (4 * i + 3)) + '' +
                num.substring(num.length - (4 * i + 3));
    return (((sign) ? '' : '-') + num + '.' + cents);
}
//保留两位小数    
//功能：将浮点数四舍五入，取小数点后2位   
function ToDecimal(x) {
    var f = parseFloat(x);
    if (isNaN(f)) {
        return 0;
    }
    f = Math.round(x * 100) / 100;
    return f;
}
/**
查找数组中是否存在某个值
arr:数组
val:对象值
**/
function ArrayExists(arr, val) {
    for (var i = 0; i < arr.length; i++) {
        if (arr[i] == val)
            return true;
    }
    return false;
}
/*弹出对话框begin========================================*/
/*关闭对话框*/
function closeDialog() {
    var api = frameElement.api, W = api.opener;
    api.close();
    return true;
}
/*
弹出对话框（带：确认按钮、取消按钮）
*/
function openDialog(url, _id, _title, _width, _height, callBack) {
    Loading(true);
    top.$.dialog({
        id: _id,
        width: _width,
        height: _height,
        max: false,
        lock: true,
        title: unescape(_title),
        resize: false,
        extendDrag: true,
        content: 'url:' + RootPath() + url,
        ok: function () {
            callBack(_id);
            return false;
        },
        cancel: true
    });
}
function openDialogAudit(url, _id, _title, _width, _height, _callBack1, _callBack2) {
    Loading(true);
    top.$.dialog({
        id: _id,
        width: _width,
        height: _height,
        max: false,
        lock: true,
        title: unescape(_title),
        content: 'url:' + RootPath() + url,
        button: [
            {
                name: '审核通过',
                callback: function () {
                    _callBack1(_id);
                    return false;
                }
            },
            {
                name: '审核拒绝',
                callback: function () {
                    _callBack2(_id);
                    return false;
                }
            },
            {
                name: '取 消'
            }
        ]
    });
}
function openDialogFName(url, _id, _title, _width, _height,name1,name2, _callBack1, _callBack2) {
    Loading(true);
    top.$.dialog({
        id: _id,
        width: _width,
        height: _height,
        max: false,
        lock: true,
        title: unescape(_title),
        content: 'url:' + RootPath() + url,
        button: [
            {
                name: name1,
                callback: function () {
                    _callBack1(_id);
                    return false;
                }
            },
            {
                name: name2,
                callback: function () {
                    _callBack2(_id);
                    return false;
                }
            },
            {
                name: '取 消'
            }
        ]
    });
}

function CurentTime() {
    var now = new Date();

    var year = now.getFullYear();       //年
    var month = now.getMonth() + 1;     //月
    var day = now.getDate();            //日

    var hh = now.getHours();            //时
    var mm = now.getMinutes();          //分

    var clock = year + "-";

    if (month < 10)
        clock += "0";

    clock += month + "-";

    if (day < 10)
        clock += "0";

    clock += day + " ";

    if (hh < 10)
        clock += "0";

    clock += hh + ":";
    if (mm < 10) clock += '0';
    clock += mm;
    return (clock);
}

/*
最大化弹出对话框（带：确认按钮、取消按钮）
*/
function FullopenDialog(url, _id, _title, callBack) {
    Loading(true);
    top.$.dialog({
        id: _id,
        lock: true,
        title: _title,
        max: false,
        min: false,
        width: top.$(window).width() - 40,
        height: top.$('body').height() - 100,
        content: 'url:' + RootPath() + url,
        ok: function () {
            callBack(_id);
            return false;
        },
        cancel: true
    })
}
/*
弹出对话框（没按钮）
*/
function Dialog(url, _id, _title, _width, _height) {
    Loading(true);
    top.$.dialog({
        id: _id,
        width: _width,
        height: _height,
        max: false,
        lock: true,
        title: _title,
        content: 'url:' + RootPath() + url
    });
}
/*
最大化弹出对话框（没按钮）
*/
function FullDialog(url, _id, _title) {
    Loading(true);
    top.$.dialog({
        id: _id,
        lock: true,
        title: _title,
        max: false,
        min: false,
        width: top.$(window).width() - 40,
        height: top.$('body').height() - 100,
        content: 'url:' + RootPath() + url
    })
}
/*
弹出查询
*/
function QueryDialog(url, _id, _title, _width, _height, _callBack) {
    Loading(true);
    top.$.dialog({
        id: _id,
        width: _width,
        height: _height,
        max: false,
        lock: true,
        title: _title,
        content: 'url:' + RootPath() + url,
        button: [
            {
                name: '查 询',
                callback: function () {
                    _callBack(_id);
                    return false;
                }
            },
            {
                name: '取 消'
            }
        ]
    });
}

/*
弹出对话框
*/
function MessageDialog(_html, _id, _title, _width, _height, _callBack) {
    top.$.dialog({
        id: _id,
        width: _width,
        height: _height,
        max: false,
        min: false,
        title: _title,
        content: _html,
        ok: function () {
            var data = eval("(" + top.GetParameterJson(_id) + ")");;
            _callBack(data);
            return false;
        },
        cancel: true
    });
}
/*
弹出打印网页
*/
function PrintDialog(url, _id, _title, _width, _height, _callBack) {
    Loading(true);
    top.$.dialog({
        id: _id,
        width: _width,
        height: _height,
        max: false,
        min: false,
        lock: true,
        title: _title,
        content: 'url:' + RootPath() + url,
        button: [
            {
                name: '打 印',
                callback: function () {
                    _callBack(_id);
                    return false;
                }
            },
            {
                name: '取 消'
            }
        ]
    });
}

/*打开网页 window.open
/*url:          表示请求路径
/*windowname:   定义页名称
/*width:        宽度
/*height:       高度
---------------------------------------------------*/
function OpenWindow(url, title, w, h) {
    var width = w;
    var height = h;
    var left = ($(window).width() - width) / 2;
    var top = ($(window).height() - height) / 2;
    window.open(RootPath() + url, title, 'height=' + height + ', width=' + width + ', top=' + top + ', left=' + left + ', toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, status=no, titlebar=yes, alwaysRaised=yes');
}

/**
短暂提示
msg: 显示消息
time：停留时间
type：类型 >1：成功，<1：失败，其他：警告
**/
function tipDialog(msg, time, type) {
    var msg = "<div class='ui_alert_tip'>" + msg + "</div>"
    if (type >= 1) {
        top.$.dialog.tips(msg, time, 'succ.png');
    } else if (type == -1) {
        top.$.dialog.tips(msg, time, 'fail.png');
    } else if (type == 0) {
        top.$.dialog.tips(msg, time, 'fail.png');
    } else {
        top.$.dialog.tips(msg, time, 'i.png');
    }
}
function tipDialog1(msg, time, type) {
    var msg = "<div class='ui_alert_tip'>" + msg + "</div>"
    if (type >= 1) {
        $.dialog.tips(msg, time, 'succ.png');
    } else if (type == -1) {
        $.dialog.tips(msg, time, 'fail.png');
    } else if (type == 0) {
        $.dialog.tips(msg, time, 'fail.png');
    } else {
        $.dialog.tips(msg, time, 'i.png');
    }
}
/*
警告消息
msg: 显示消息
type：类型 >1：成功，<1：失败，其他：警告
*/
function alertDialog(msg, type) {
    var msg = "<div class='ui_alert'>" + msg + "</div>"
    var icon = "";
    if (type >= 1) {
        icon = "succ.png";
    } else if (type == -1) {
        icon = "fail.png";
    } else {
        icon = "i.png";
    }
    top.$.dialog({
        id: "alertDialog",
        icon: icon,
        content: msg,
        title: "提示",
        ok: function () {
            return true;
        }
    });
}
/*
确认对话框
*/
function confirmDialog(_title, msg, callBack) {
    var msg = "<div class='ui_alert'>" + msg + "</div>"
    top.$.dialog({
        id: "confirmDialog",
        lock: true,
        icon: "hits.png",
        content: msg,
        title: _title,
        ok: function () {
            callBack(true)
            return true;
        },
        cancel: function () {
            callBack(false)
            return true;
        }
    });
}
function confirmDialog1(_title, msg, callBack) {
    var msg = "<div class='ui_alert'>" + msg + "</div>"
    $.dialog({
        id: "confirmDialog",
        lock: true,
        icon: "hits.png",
        content: msg,
        title: _title,
        ok: function () {
            callBack(true)
            return true;
        },
        cancel: function () {
            callBack(false)
            return true;
        }
    });
    return false;
}
/*弹出对话框end========================================*/
/***
 * 自动关闭弹出内容提示
timeOut : 4000,				//提示层显示的时间
msg : "恭喜你!你已成功操作此插件，谢谢使用！",			//显示的消息
type : "success"//提示类型（1、success 2、error 3、warning）
***/
function TipMsg(msg, timeOut, type) {
    $(".tip_container").remove();
    var bid = parseInt(Math.random() * 100000);
    $("body").prepend('<div id="tip_container' + bid + '" class="container tip_container"><div id="tip' + bid + '" class="mtip"><span id="tsc' + bid + '"></span></div></div>');
    var $this = $(this);
    var $tip_container = $("#tip_container" + bid);
    var $tip = $("#tip" + bid);
    var $tipSpan = $("#tsc" + bid);
    //先清楚定时器
    clearTimeout(window.timer);
    //主体元素绑定事件
    $tip.attr("class", type).addClass("mtip");
    $tipSpan.html(msg);
    $tip_container.slideDown(300);
    //提示层隐藏定时器
    window.timer = setTimeout(function () {
        $tip_container.slideUp(300);
        $(".tip_container").remove();
    }, timeOut);
    //鼠标移到提示层时清除定时器
    $tip_container.live("mouseover", function () {
        clearTimeout(window.timer);
    });
    //鼠标移出提示层时启动定时器
    $tip_container.live("mouseout", function () {
        window.timer = setTimeout(function () {
            $tip_container.slideUp(300);
            $(".tip_container").remove();
        }, timeOut);
    });
    $("#tip_container" + bid).css("left", ($(window).width() - $("#tip_container" + bid).width()) / 2);
}


/*
验证是否为空
*/
function IsNullOrEmpty(str) {
    var isOK = true;
    if (str == undefined || str == "" || str == 'null') {
        isOK = false;
    }
    return isOK;
}
function IsDelData(id) {
    var isOK = true;
    if (id == undefined || id == "" || id == 'null' || id == 'undefined') {
        isOK = false;
        tipDialog('您没有选中任何项,请您选中后再操作。', 4, 'warning');
    }
    return isOK;
}
function IsChecked(id) {
    var isOK = true;
    if (id == undefined || id == "" || id == 'null' || id == 'undefined') {
        isOK = false;
        tipDialog('您没有选中任何项,请您选中后再操作。', 4, 'warning');
    } else if (id.split(",").length > 1) {
        isOK = false;
        tipDialog('很抱歉,一次只能选择一条记录。', 4, 'warning');
    }
    return isOK;
}

/*鼠标右击菜单begin========================================*/
var getOffset = {
    top: function (obj) {
        return obj.offsetTop + (obj.offsetParent ? arguments.callee(obj.offsetParent) : 0)
    },
    left: function (obj) {
        return obj.offsetLeft + (obj.offsetParent ? arguments.callee(obj.offsetParent) : 0)
    }
};
function LoadrightMenu(element) {
    var oMenu = $('.rightMenu');
    $(document).click(function () {
        oMenu.hide();
    });
    $(document).mousedown(function (e) {
        if (3 == e.which) {
            oMenu.hide();
        }
    })
    var aUl = oMenu.find("ul");
    var aLi = oMenu.find("li");
    var showTimer = hideTimer = null;
    var i = 0;
    var maxWidth = maxHeight = 0;
    var aDoc = [document.documentElement.offsetWidth, document.documentElement.offsetHeight];
    oMenu.hide();
    for (i = 0; i < aLi.length; i++) {
        //为含有子菜单的li加上箭头
        aLi[i].getElementsByTagName("ul")[0] && (aLi[i].className = "sub");
        //鼠标移入
        aLi[i].onmouseover = function () {
            var oThis = this;
            var oUl = oThis.getElementsByTagName("ul");
            //鼠标移入样式
            oThis.className += " active";
            //显示子菜单
            if (oUl[0]) {
                clearTimeout(hideTimer);
                showTimer = setTimeout(function () {
                    for (i = 0; i < oThis.parentNode.children.length; i++) {
                        oThis.parentNode.children[i].getElementsByTagName("ul")[0] &&
						(oThis.parentNode.children[i].getElementsByTagName("ul")[0].style.display = "none");
                    }
                    oUl[0].style.display = "block";
                    oUl[0].style.top = oThis.offsetTop + "px";
                    oUl[0].style.left = oThis.offsetWidth + "px";

                    //最大显示范围					
                    maxWidth = aDoc[0] - oUl[0].offsetWidth;
                    maxHeight = aDoc[1] - oUl[0].offsetHeight;

                    //防止溢出
                    maxWidth < getOffset.left(oUl[0]) && (oUl[0].style.left = -oUl[0].clientWidth + "px");
                    maxHeight < getOffset.top(oUl[0]) && (oUl[0].style.top = -oUl[0].clientHeight + oThis.offsetTop + oThis.clientHeight + "px")
                }, 300);
            }
        };
        //鼠标移出	
        aLi[i].onmouseout = function () {
            var oThis = this;
            var oUl = oThis.getElementsByTagName("ul");
            //鼠标移出样式
            oThis.className = oThis.className.replace(/\s?active/, "");

            clearTimeout(showTimer);
            hideTimer = setTimeout(function () {
                for (i = 0; i < oThis.parentNode.children.length; i++) {
                    oThis.parentNode.children[i].getElementsByTagName("ul")[0] &&
					(oThis.parentNode.children[i].getElementsByTagName("ul")[0].style.display = "none");
                }
            }, 300);
        };
    }
    //自定义右键菜单
    $(element).bind("contextmenu", function () {
        var event = event || window.event;
        oMenu.show();
        oMenu.css('top', event.clientY + "px");
        oMenu.css('left', event.clientX + "px");
        //最大显示范围
        maxWidth = aDoc[0] - oMenu.width();
        maxHeight = aDoc[1] - oMenu.height();
        //防止菜单溢出
        if (oMenu.offset().top > maxHeight) {
            oMenu.css('top', maxHeight + "px");
        }
        if (oMenu.offset().left > maxWidth) {
            oMenu.css('left', maxWidth + "px");
        }
        return false;
    }).bind("click", function () {
        oMenu.hide();
    });
}
/*鼠标右击菜单end========================================*/
/*自定义复选框css  begin========================================*/
$(document).ready(function () {
    $('.form').customInput();
});
jQuery.fn.customInput = function () {
    return $('input').each(function () {
        if ($(this).is('[type=checkbox],[type=radio]')) {
            var input = $(this);
            if (input.attr('class') != 'checkbox') {
                // 使用输入的ID得到相关的标签
                var label = $('label[for=' + input.attr('id') + ']');
                // 包裹在一个div输入+标签
                input.add(label).wrapAll('<div class="custom-' + input.attr('type') + '"></div>');
                //绑定自定义事件，触发它，绑定点击，焦点，模糊事件				
                input.bind('updateState', function () {
                    input.is(':checked') ? label.addClass('checked') : label.removeClass('checked checkedHover checkedFocus');
                })
                .trigger('updateState')
                .click(function () {
                    input.is(':checked') ? input.attr("checked", 'checked') : input.removeAttr("checked");
                    $('input[name=' + $(this).attr('name') + ']').trigger('updateState');
                })
                .focus(function () {
                    label.addClass('focus');
                    if (input.is(':checked')) { $(this).addClass('checkedFocus'); }
                })
                .blur(function () { label.removeClass('focus checkedFocus'); });
            }
        }
    });
};
/*自定义复选框css  end========================================*/

//自动补全表格
var IndetableRow_autocomplete = 0;
var scrollTopheight = 0;
function autocomplete(Objkey, width, height, data, callBack,Nclear) {
    if ($('#' + Objkey).attr('readonly') == 'readonly') {
        return false;
    }
    if ($('#' + Objkey).attr('disabled') == 'disabled') {
        return false;
    }
    IndetableRow_autocomplete = 0;
    scrollTopheight = 0;
    var X = $("#" + Objkey).offset().top;
    var Y = $("#" + Objkey).offset().left - 1;
    $("#div_gridshow").html("");
    if ($("#div_gridshow").attr("id") == undefined) {
        $('body').append('<div id="div_gridshow" style="overflow: auto;z-index: 1000;border: 1px solid #A8A8A8;width:' + width + ';height:' + height + ';position: absolute; background-color: #fff; display: none;"></div>');
    } else {
        $("#div_gridshow").height(height);
        $("#div_gridshow").width(width);
    }
    var sbhtml = '<table class="grid" style="width: 100%;">';
    if (data != "") {
        sbhtml += '<tbody>' + data + '</tbody>';
    } else {
        sbhtml += '<tbody><tr id="tr_error"><td style="color:red;text-align:center;width:' + width + ';">没有找到您要的相关数据！</td></tr></tbody>';
    }
    sbhtml += '</table>';
    $("#div_gridshow").html(sbhtml);
    $("#div_gridshow").css("left", Y).css("top", X + $('#' + Objkey).height()).show();
    $("#div_gridshow .grid td").css("border-left", "none").css("padding-left", "2px");
    if (data != "") {
        $("#div_gridshow").find('tbody tr').each(function (r) {
            if (r == 0) {
                $(this).addClass('selected');
            }
        });
    }
    $("#div_gridshow").find('tbody tr').click(function () {
        var parameter = "";
        $(this).find('td').each(function (i) {
            parameter += '"' + $(this).attr('id') + '"' + ':' + '"' + $.trim($(this).text()) + '",'
        });
        if ($('#' + Objkey).attr('readonly') == 'readonly') {
            return false;
        }
        if ($('#' + Objkey).attr('disabled') == 'disabled') {
            return false;
        }
        callBack(JSON.parse('{' + parameter.substr(0, parameter.length - 1) + '}'));
        $("#div_gridshow").hide();
    });
    $("#div_gridshow").find('tbody tr').hover(function () {
        $(this).addClass("selected");
    }, function () {
        $(this).removeClass("selected");
    });
    //任意键关闭
    document.onclick = function (e) {
        var e = e ? e : window.event;
        var tar = e.srcElement || e.target;
        if (tar.id != 'div_gridshow') {
            if ($(tar).attr("id") == 'div_gridshow' || $(tar).attr("id") == Objkey) {
                if (!$("#div_gridshow").is(":hidden")) {
                    $("#div_gridshow").show();
                }
            } else {
                if (!$("#div_gridshow").is(":hidden")) {
                    var count = $("#div_gridshow").find('tbody').find('#tr_error').length;
                    if (count == 1) {
                        if (!Nclear) {
                            $('#' + Objkey).val('');
                        }
                        var parameter = "";
                        //callBack(JSON.parse('{' + parameter.substr(0, parameter.length - 1) + '}'));
                    } else {
                        if ($('#' + Objkey).val()) {
                            var parameter = "";
                            $("#div_gridshow").find('tbody tr:eq(0)').find('td').each(function (i) {
                                parameter += '"' + $(this).attr('id') + '"' + ':' + '"' + $.trim($(this).text()) + '",'
                            });
                            callBack(JSON.parse('{' + parameter.substr(0, parameter.length - 1) + '}'));
                        }
                    }
                    $("#div_gridshow").hide();
                }
            }
        }
    }
}
//方向键上,方向键下,回车键
function autocompletekeydown(Objkey, callBack) {
    $("#" + Objkey).keydown(function (e) {
        switch (e.keyCode) {
            case 38: // 方向键上
                if (IndetableRow_autocomplete > 0) {
                    IndetableRow_autocomplete--
                    $("#div_gridshow").find('tbody tr').removeClass('selected');
                    $("#div_gridshow").find('tbody tr').each(function (r) {
                        if (r == IndetableRow_autocomplete) {
                            scrollTopheight -= 22;
                            $("#div_gridshow").scrollTop(scrollTopheight);
                            $(this).addClass('selected');
                        }
                    });
                }
                break;
            case 40: // 方向键下
                var tindex = $("#div_gridshow").find('tbody tr').length - 1;
                if (IndetableRow_autocomplete < tindex) {
                    IndetableRow_autocomplete++;
                    $("#div_gridshow").find('tbody tr').removeClass('selected');
                    $("#div_gridshow").find('tbody tr').each(function (r) {
                        if (r == IndetableRow_autocomplete) {
                            scrollTopheight += 22;
                            $("#div_gridshow").scrollTop(scrollTopheight);
                            $(this).addClass('selected');
                        }
                    });
                }
                break;
            case 13:  //回车键
                var parameter = "";
                $("#div_gridshow").find('tbody tr').each(function (r) {
                    if (r == IndetableRow_autocomplete) {
                        $(this).find('td').each(function (i) {
                            parameter += '"' + $(this).attr('id') + '"' + ':' + '"' + $.trim($(this).text()) + '",'
                        });
                    }
                });
                if ($('#' + Objkey).attr('readonly') == 'readonly') {
                    return false;
                }
                if ($('#' + Objkey).attr('disabled') == 'disabled') {
                    return false;
                }
                callBack(JSON.parse('{' + parameter.substr(0, parameter.length - 1) + '}'));
                $("#div_gridshow").hide();
                break;
            default:
                break;
        }
    })
}
/**
树下拉框
Objkey:          ID
width：          宽度
height：         高度
data：           数据
**/
function comboBoxTree(Objkey, height) {
    top.$(".ui_state_highlight").focus();
    var width = $("#" + Objkey).width();
    $("#" + Objkey).css('ime-mode', 'disabled');
    $("#" + Objkey).bind("contextmenu", function () { return false; });
    $("#" + Objkey).keypress(function (e) { return false; });
    if ($('#' + Objkey).attr('readonly') == 'readonly') { return false; }
    if ($('#' + Objkey).attr('disabled') == 'disabled') { return false; }
    var X = $("#" + Objkey).offset().top - 1;
    var Y = $("#" + Objkey).offset().left - 1;
    var comboBoxTree = "comboBoxTree" + Objkey;
    if ($("#" + comboBoxTree).attr("id") == undefined) {
        $('body').append('<div id="' + comboBoxTree + '" style="overflow: auto;border: 1px solid #ccc;border-top:none;width:' + width + 'px;height:' + height + ';position: absolute; background-color: #fff; display: none;"></div>');
    }
    $("#" + comboBoxTree).css("left", Y).css("top", X + $("#" + Objkey).height() + 2).css("z-index", "99").slideDown(100);
    //任意键关闭
    document.onclick = function (e) {
        var e = e ? e : window.event;
        var tar = e.srcElement || e.target;
        if (tar.id != '' + comboBoxTree + '') {
            if ($(tar).hasClass("bbit-tree-ec-icon")) {
                return false;
            }
            if ($(tar).attr("id") == Objkey) {
                return false;
            } else {
                $("#" + comboBoxTree).slideUp(100);
            }
        }
    }
}
function SelectTree(Objkey, height) {
    top.$(".ui_state_highlight").focus();
    var width = $("#" + Objkey).width();
    $("#" + Objkey).css('ime-mode', 'disabled');
    $("#" + Objkey).bind("contextmenu", function () { return false; });
    $("#" + Objkey).keypress(function (e) { return false; });
    if ($('#' + Objkey).attr('readonly') == 'readonly') { return false; }
    if ($('#' + Objkey).attr('disabled') == 'disabled') { return false; }
    var X = $("#" + Objkey).offset().top;
    var Y = $("#" + Objkey).offset().left;
    var comboBoxTree = "comboBoxTree" + Objkey;
    if ($("#" + comboBoxTree).attr("id") == undefined) {
        $('body').append('<div id="' + comboBoxTree + '" style="overflow: auto;border: 1px solid #ccc;border-top:none;width:' + width + 'px;height:' + height + ';position: absolute; background-color: #fff; display: none;"></div>');
    }
    $("#" + comboBoxTree).css("left", Y).css("top", X + $("#" + Objkey).height() + 2).css("z-index", "99").slideDown(100);
    //任意键关闭
    document.onclick = function (e) {
        var e = e ? e : window.event;
        var tar = e.srcElement || e.target;
        if (tar.id != '' + comboBoxTree + '') {
            if ($(tar).hasClass("bbit-tree-ec-icon")) {
                return false;
            }
            if ($(tar).attr("id") == Objkey) {
                return false;
            } else {
                $("#" + comboBoxTree).slideUp(100);
            }
        }
    }
}
/*删除数据
/*url:        表示请求路径
/*parm：      条件参数
--------------------------------------------------*/
function delConfirm(url, parm, msg) {
    confirmDialog("提示", msg, function (r) {
        if (r) {
            Loading(true, "正在删除数据...");
            window.setTimeout(function () {
                AjaxJson(url, parm, function (data) {
                    tipDialog(data.Message, 3, data.Code);
                    if (data.Code > 0) {
                        windowload();
                    }
                });
            }, 200);
        }
    });
}
function delConfig(url, parm, count) {
    if (count == undefined) {
        count = 1;
    }
    confirmDialog("提示", "注：您确定要删除 " + count + " 笔记录？", function (r) {
        if (r) {
            Loading(true, "正在删除数据...");
            window.setTimeout(function () {
                AjaxJson(url, parm, function (data) {
                    tipDialog(data.Message, 3, data.Code);
                    if (data.Code > 0) {
                        windowload();
                    }
                });
            }, 200);
        }
    });
}
/*公共导出Excel，将表格数据取出来。在导出Excel
obj:表格对象ID
fileName：导出Excel文件名
*/
function GetDeriveExcel(obj, fileName) {
    var column = $(obj).pqGrid("option", "colModel");//获取表格表头
    var data = GetPqGridSelectedRowValue("#grid_paging");
    if (data == "") {
        data = $(obj).pqGrid("option", "dataModel").data;//获取表格数据
    }
    confirmDialog("提示", "注：您确定要导出 " + data.length + " 笔记录？", function (r) {
        if (r) {
            var postData = {
                JsonColumn: JSON.stringify(column),
                JsonData: JSON.stringify(data),
                FileName: fileName
            }
            //var JsonData = {
            //    JsonColumn: JSON.stringify(columnModel),
            //    JsonData: JSON.stringify(dataModel),
            //    JsonFooter: JSON.stringify(footerData),
            //    FileName: fileName,
            //    SelectedField: SelectedField
            //}
            getAjax("/Utility/SetDeriveExcel", JsonData, function (data) { })

            var parm = '/Utility/GetDeriveExcel';
            location.href = parm;
        }
    });
}
/*绑定数据字典模糊搜索
ControlId:控件ID
Code:分类编码
Memo:默认显示
*/


/*绑定数据字典下拉框
ControlId:控件ID
Code:分类编码
Memo:默认显示
*/
function BindDropItem(ControlId, Code, Memo) {
    $(ControlId).html("");
    if (IsNullOrEmpty(Memo)) {
        $(ControlId).append("<option value=''>" + Memo + "</option>");
    }
    getAjax('/CommonModule/DataDictionary/BinDingItemsJson', { Code: Code }, function (data) {
        var itemjson = eval("(" + data + ")");
        $.each(itemjson, function (i) {
            $(ControlId).append($("<option></option>").val(itemjson[i].Code).html(itemjson[i].FullName));
        });
        Loading(false);
    });
}
function BindDropItemT2(ControlId,ControlId1, Code, Memo) {
    getAjax('/CommonModule/DataDictionary/BinDingItemsJson', { Code: Code}, function (data) {
        var itemjson = eval("(" + data + ")");
        $.each(itemjson, function (i) {
            $(ControlId).append($("<option></option>").val(itemjson[i].Code).html(itemjson[i].FullName));
        });
        $(ControlId).change(function () {
            getAjax('/CommonModule/DataDictionary/BinDingItemsJson1', { Code: Code, FullName: $(ControlId).val() }, function (data1) {
                $(ControlId1).empty();
                var itemjson1 = eval("(" + data1 + ")");
                $.each(itemjson1, function (i) {
                    $(ControlId1).append($("<option></option>").val(itemjson1[i].FullName).html(itemjson1[i].FullName));
                });
            });
        });
        Loading(false);
    });
}
function BindDropItemT2index(ControlId, ControlId1, Code, Memo) {
    getAjax('/CommonModule/DataDictionary/BinDingItemsJson', { Code: Code }, function (data) {
        var itemjson = eval("(" + data + ")");
        $(ControlId).empty()
        $(ControlId).append($("<option></option>").val("").html("全部"));
        $.each(itemjson, function (i) {
            $(ControlId).append($("<option></option>").val(itemjson[i].Code).html(itemjson[i].FullName));
        });
        $(ControlId).change(function () {
            getAjax('/CommonModule/DataDictionary/BinDingItemsJson1', { Code: Code, FullName: $(ControlId).val() }, function (data1) {
                var itemjson1 = eval("(" + data1 + ")");
                $(ControlId1).empty()
                $(ControlId1).append($("<option></option>").val("").html("全部"));
                $.each(itemjson1, function (i) {
                    $(ControlId1).append($("<option></option>").val(itemjson1[i].FullName).html(itemjson1[i].FullName));
                });
            });
        });
        Loading(false);
    });
}

function BindDropItemName(ControlId, Code, Memo) {
    //$(ControlId).html("");
    //if (IsNullOrEmpty(Memo)) {
    //    $(ControlId).append("<option value=''>" + Memo + "</option>");
    //}
    getAjax('/CommonModule/DataDictionary/BinDingItemsJson', { Code: Code }, function (data) {
        var itemjson = eval("(" + data + ")");
        $.each(itemjson, function (i) {
            $(ControlId).append($("<option></option>").val(itemjson[i].FullName).html(itemjson[i].FullName));
        });
        Loading(false);
    });
}
/*绑定下拉框
ControlId:控件ID
Memo:默认显示
*/
function JsonBindDrop(ControlId, Memo, DataJson) {
    $(ControlId).html("");
    if (IsNullOrEmpty(Memo)) {
        $(ControlId).append("<option value=''>" + Memo + "</option>");
    }
    var DataJson = eval("(" + DataJson + ")");
    $.each(DataJson, function (i) {
        $(ControlId).append($("<option></option>").val(DataJson[i].Code).html(DataJson[i].FullName));
    });
}
/*
验证对象值不能重复 
id:字段ID、
tablename：表名、
keyfield：主键Id、
Msg：提示消息
*/
function FieldExist(id, tablename, keyfield, Msg) {
    var $ObjFiel = $("#" + id);
    var postData = {
        tablename: tablename,
        fieldname: id,
        fieldvalue: $ObjFiel.val(),
        keyfield: GetQuery('KeyValue'),
        keyvalue: keyfield
    }
    getAjax('/Utility/FieldExist', postData, function (data) {
        if (data.toLocaleLowerCase() == 'false') {
            tipCss("#" + id, Msg + "已存在,请重新输入");
        }
        Loading(false);
    });
}
/*公共导出Excel，将表格数据取出来。在导出Excel
obj:表格对象ID
fileName：导出Excel文件名
*/
function SetDeriveExcel(obj, fileName) {
    var columnModel = $(obj).jqGrid('getGridParam', 'colModel');
    var dataModel = $(obj).jqGrid("getRowData");
    var footerData = $(obj).jqGrid("footerData");
    var SelectedField = "";
    for (var i = 0; i < columnModel.length; i++) {
        if (!columnModel[i].hidden) {
            SelectedField += columnModel[i].name + ","
        }
    }
    var JsonData = {
        JsonColumn: JSON.stringify(columnModel),
        JsonData: JSON.stringify(dataModel),
        JsonFooter: JSON.stringify(footerData),
        FileName: fileName,
        SelectedField: SelectedField
    }
    getAjax("/Utility/SetDeriveExcel", JsonData, function (data) { })
}
function SetDeriveSelectedField(SelectedField) {
    var JsonData = {
        SelectedField: SelectedField
    }
    getAjax("/Utility/SetDeriveExcel", JsonData, function (data) { })
}
/*
表格视图列名
*/
function columnModelData(jgrid) {
    AjaxJson("/Utility/LoadViewColumn", {}, function (data) {
        $.each(data, function (i) {
            $(jgrid).hideCol(data[i].FieldName);
        });
    });
}
//加载权限按钮
function PartialButton() {
    var JsonData = "";
    var toolbar_html = "";
    var toolbar_Children_Count = 0;
    AjaxJson("/Utility/LoadButton", {}, function (Data) {
        JsonData = Data;
        $.each(Data, function (i) {
            if (Data[i].ParentId == '0' && Data[i].Category == '1') {
                var toolbar_Children_List = Toolbar_Children(Data[i].ButtonId, Data)
                if (toolbar_Children_Count > 0) {
                    toolbar_html += "<a id=\"" + Data[i].Code + "\" class=\"tools_btn dropdown\">";
                    toolbar_html += "<div style=\"float: left;\"><div class=\"icon\"><img src=\"../../Content/Images/Icon16/" + Data[i].Icon + "\" /></div><div class=\"text\">" + Data[i].FullName + "</div></div>";
                    toolbar_html += "<div class=\"dropdown-icon\"><img src=\"../../Content/Images/dropdown-icon.png\" /></div>";
                    toolbar_html += "<div class=\"dropdown-data\"><i></i><span></span>";
                    toolbar_html += "    <ul>";
                    toolbar_html += toolbar_Children_List;
                    toolbar_html += "    </ul>";
                    toolbar_html += "</div>";
                    toolbar_html += "</a>";
                } else {
                    var title = "title=\"" + Data[i].Remark + "\"";
                    if (Data[i].Remark == "") { title = ""; }
                    toolbar_html += "<a id=\"" + Data[i].Code + "\" " + title + " onclick=\"" + Data[i].JsEvent + "\" class=\"tools_btn\"><span><b style=\"background: url('../../Content/Images/Icon16/" + Data[i].Icon + "') 50% 4px no-repeat;\">" + Data[i].FullName + "</b></span></a>";
                }
                if (Data[i].Split == '1') {
                    toolbar_html += "<div class=\"tools_separator\"></div>";
                }
            }
        });
    });
    $('.tools_bar .PartialButton').html(toolbar_html);
    function Toolbar_Children(ButtonId, Data) {
        var _toolbar_Children = "";
        toolbar_Children_Count = 0;
        $.each(Data, function (i) {
            if (Data[i].ParentId == ButtonId) {
                var title = "title=\"" + Data[i].Remark + "\"";
                if (Data[i].Remark == "") { title = ""; }
                _toolbar_Children += "<li id=\"" + Data[i].Code + "\" " + title + " onclick=\"" + Data[i].JsEvent + "\"><img src=\"../../Content/Images/Icon16/" + Data[i].Icon + "\" />" + Data[i].FullName + "</li>";
                toolbar_Children_Count++;
            }
        });
        return _toolbar_Children;
    }
    $(".tools_btn").hover(function () {
        $(this).addClass("tools_btn_hover");
    }, function () {
        $(this).removeClass("tools_btn_hover");
    });
    $(".tools_bar .dropdown").hover(function () {
        var left = $(this).offset().left - ($(this).find('.dropdown-data').width() / 2) + ($(this).width() / 2 + 9);
        $(this).find('.dropdown-data').show().css('top', ($(this).offset().top + 50)).css('left', left);
        $(this).find('.dropdown-icon').addClass('dropdown-icon-hover');
        $(this).addClass('dropdown-selected');
    }, function () {
        if (!$(this).hasClass("_click")) {
            $(this).removeClass('dropdown-selected');
            $(this).find('.dropdown-data').hide();
            $(this).find('.dropdown-icon').removeClass('dropdown-icon-hover');
        }
    });
    $('.tools_bar .dropdown').toggle(function () {
        $(this).addClass('_click');
        var left = $(this).offset().left - ($(this).find('.dropdown-data').width() / 2) + ($(this).width() / 2 + 9);
        $(this).find('.dropdown-data').show().css('top', ($(this).offset().top + 50)).css('left', left);
        $(this).find('.dropdown-icon').addClass('dropdown-icon-hover');
        $(this).addClass('dropdown-selected');
    }, function () {
        $(this).removeClass('dropdown-selected');
        $(this).find('.dropdown-data').hide();
        $(this).find('.dropdown-icon').removeClass('dropdown-icon-hover');
        $('.dropdown').removeClass('_click');
    });
    $(".dropdown-data li").click(function () {
        $('.dropdown').removeClass('dropdown-selected');
        $('.dropdown').find('.dropdown-data').hide();
        $('.dropdown').find('.dropdown-icon').removeClass('dropdown-icon-hover');
        $('.dropdown').removeClass('_click');
    });
    //右击菜单
    var right_toolbar_Count = 0;
    var right_toolbar_html = "";
    right_toolbar_html += '<div class="rightMenu"><ul>';
    $.each(JsonData, function (i) {
        if (JsonData[i].ParentId == '0' && JsonData[i].Category == '2') {
            var title = "title=\"" + JsonData[i].Remark + "\"";
            if (JsonData[i].Remark == "") { title = ""; }
            var righttoolbar_Children_List = right_toolbar_Children(JsonData[i].ButtonId, JsonData)
            if (right_toolbar_Count > 0) {
                right_toolbar_html += "<li id=\"right_" + JsonData[i].Code + "\" " + title + " onclick=\"" + JsonData[i].JsEvent + "\" ><img src=\"../../Content/Images/Icon16/" + JsonData[i].Icon + "\" />" + JsonData[i].FullName + righttoolbar_Children_List + "</li>";
            } else {
                right_toolbar_html += "<li id=\"right_" + JsonData[i].Code + "\" " + title + " onclick=\"" + JsonData[i].JsEvent + "\" ><img src=\"../../Content/Images/Icon16/" + JsonData[i].Icon + "\" />" + JsonData[i].FullName + "</li>";
            }
            if (JsonData[i].Split == '1') {
                right_toolbar_html += "<div class=\"m-split\"></div>";
            }
        }
    });
    right_toolbar_html += '</ul></div>';
    function right_toolbar_Children(ButtonId, JsonData) {
        var _right_toolbar = "<ul>";
        right_toolbar_Count = 0;
        $.each(JsonData, function (i) {
            if (JsonData[i].ParentId == ButtonId) {
                var title = "title=\"" + JsonData[i].Remark + "\"";
                if (JsonData[i].Remark == "") { title = ""; }
                _right_toolbar += "<li id=\"right_" + JsonData[i].Code + "\" " + title + " onclick=\"" + JsonData[i].JsEvent + "\" ><img src=\"../../Content/Images/Icon16/" + JsonData[i].Icon + "\" />" + JsonData[i].FullName + "</li>";
                if (JsonData[i].Split == '1') {
                    _right_toolbar += "<div class=\"m-split\"></div>";
                }
                right_toolbar_Count++;
            }
        });
        return _right_toolbar + "</ul>";
    }
    $("body").append(right_toolbar_html);
    if ($('.rightMenu').find('li').length > 0) {
        LoadrightMenu("#grid_List");
    } else {
        $('.rightMenu').remove();
    }
}
//=================动态菜单tab标签========================
function AddTabMenu(tabid, url, name, img, Isclose, IsReplace, IsVisitorModule) {
    $('#overlay_startmenu').hide(); $('#start_menu_panel').hide(); $('#start_menu_panel .nicescroll-rails').show(); $('.nicescroll-rails').hide();
    if (url == "" || url == "#" || url == RootPath()) {
        url = RootPath() + "/Error/Error404";
    }
    var tabs_container = top.$("#tabs_container");
    var ContentPannel = top.$("#ContentPannel");
    if (IsReplace == 'true' || IsReplace == true) {
        top.RemoveDiv(tabid);
    }
    if (top.document.getElementById("tabs_" + tabid) == null) { //如果当前tabid存在直接显示已经打开的tab
        Loading(true);
        if (!IsVisitorModule) {
            VisitorModule(tabid, name);
        }
        if (tabs_container.find('li').length >= 8) {
            top.$(".tab-nav").find("[win_close=true]").each(function (i) {
                if (i == 0) {
                    tabs_container.find("#tabs_" + $(this).attr('id').substr(5)).remove();
                    ContentPannel.find("#tabs_iframe_" + $(this).attr('id').substr(5)).remove();
                }
            });
        }
        tabs_container.find('li').removeClass('selected');
        ContentPannel.find('iframe').hide();
        if (Isclose != 'false') { //判断是否带关闭tab
            tabs_container.append("<li id=\"tabs_" + tabid + "\" class='selected' win_close='true'><span title='" + name + "' onclick=\"AddTabMenu('" + tabid + "','" + url + "','" + name + "','true')\"><a><img src='../Content/Images/Icon16/" + img + "' width='16' height='16'>" + name + "</a></span><a class=\"win_close\" title=\"关闭当前窗口\" onclick=\"RemoveDiv('" + tabid + "')\"></a></li>");
        } else {
            tabs_container.append("<li id=\"tabs_" + tabid + "\" class=\"selected\" onclick=\"AddTabMenu('" + tabid + "','" + url + "','" + name + "','false')\"><a><img src='../Content/Images/Icon16/" + img + "' width='16' height='16'>" + name + "</a></li>");
        }
        ContentPannel.append("<iframe id=\"tabs_iframe_" + tabid + "\" name=\"tabs_iframe_" + tabid + "\" height=\"100%\" width=\"100%\" src=\"" + url + "\" frameBorder=\"0\"></iframe>");
    } else {
        if (!IsVisitorModule) {
            VisitorModule(tabid, name);
        }
        tabs_container.find('li').removeClass('selected');
        ContentPannel.find('iframe').hide();
        tabs_container.find('#tabs_' + tabid).addClass('selected');
        top.document.getElementById("tabs_iframe_" + tabid).style.display = 'block';
        if (top.frames[tabiframeId()].$("#btnSearch")) {
            top.frames[tabiframeId()].$("#btnSearch").trigger("click");
        }
        
    }
    $('iframe#' + tabiframeId()).load(function () {
        Loading(false);
    });
    LoadrightMenu(".tab-nav li");
}
//关闭当前tab
function ThisCloseTab() {
    var tabs_container = top.$("#tabs_container");
    top.RemoveDiv(tabs_container.find('.selected').attr('id').substr(5));
}
//全部关闭tab
function AllcloseTab() {
    top.$(".tab-nav").find("[win_close=true]").each(function () {
        RemoveDiv($(this).attr('id').substr(5))
    });
}
//关闭除当前之外的tab
function othercloseTab() {
    var tabs_container = top.$("#tabs_container");
    var id = tabs_container.find('.selected').attr('id').substr(5);
    top.$(".tab-nav").find("[win_close=true]").each(function () {
        if ($(this).attr('id').substr(5) != id) {
            RemoveDiv($(this).attr('id').substr(5))
        }
    });
}
//关闭事件
function RemoveDiv(obj) {
    Loading(false);
    var tabs_container = top.$("#tabs_container");
    var ContentPannel = top.$("#ContentPannel");
    var ModuleId = tabs_container.find('.selected').attr('id').substr(5);//原来ID
    var ModuleName = tabs_container.find('.selected').find('span').attr('title');//原来名称
    SetLeave(ModuleId, ModuleName);
    tabs_container.find("#tabs_" + obj).remove();
    ContentPannel.find("#tabs_iframe_" + obj).remove();
    var tablist = tabs_container.find('li');
    var pannellist = ContentPannel.find('iframe');
    if (tablist.length > 0) {
        tablist[tablist.length - 1].className = 'selected';
        pannellist[tablist.length - 1].style.display = 'block';
        var id = tabs_container.find('.selected').attr('id').substr(5);
        VisitorModule(id);
    }
}
//访问模块，写入系统菜单Id
function VisitorModule(ModuleId, ModuleName) {
    top.$("#ModuleId").val(ModuleId);
    getAjax("/Home/SetModuleId", { ModuleId: ModuleId, ModuleName: ModuleName }, function (data) { })
}
//离开模块
function SetLeave(ModuleId, ModuleName) {
    getAjax("/Home/SetLeave", { ModuleId: ModuleId, ModuleName: ModuleName }, function (data) { })
}
//=================动态菜单tab标签========================
//js获取网站根路径(站点及虚拟目录)
function RootPath() {
    var strFullPath = window.document.location.href;
    var strPath = window.document.location.pathname;
    var pos = strFullPath.indexOf(strPath);
    var prePath = strFullPath.substring(0, pos);
    var postPath = strPath.substring(0, strPath.substr(1).indexOf('/') + 1);
    //return (prePath + postPath);如果发布IIS，有虚假目录用用这句
    return (prePath);
}

/*JQuery 限制文本框只能输入数字*/
function onlynum1(object) {
    $(object).keyup(function () {
        $(this).val($(this).val().replace(/\D|^0/g, ''));
    }).bind("paste", function () {  //CTR+V事件处理    
        $(this).val($(this).val().replace(/\D|^0/g, ''));
    }).css("ime-mode", "disabled"); //CSS设置输入法不可用    
}
/*JQuery 限制文本框只能输入数字和小数点*/
function onlynum2(object) {
    $(object).keyup(function () {
        $(this).val($(this).val().replace(/[^0-9.]/g, ''));
    }).bind("paste", function () {  //CTR+V事件处理    
        $(this).val($(this).val().replace(/[^0-9.]/g, ''));
    }).css("ime-mode", "disabled"); //CSS设置输入法不可用 
}

function minutesBetweenTimes(time1, time2) {
    const date1 = new Date(time1);
    const date2 = new Date(time2);
    const diffInMs = Math.abs(date2 - date1); // 计算两个日期之间的差值（毫秒）
    return Math.round(diffInMs / (1000 * 60)); // 将毫秒转换为分钟
}
function hourBetweenTimes(time1, time2) {
    const date1 = new Date(time1);
    const date2 = new Date(time2);
    const diffInMs = Math.abs(date2 - date1); // 计算两个日期之间的差值（毫秒）
    return Math.round(diffInMs / (1000 * 60 * 60)); // 将毫秒转换为分钟
}
function MinuteBetweenTimes(time1, time2) {
    const date1 = new Date(time1);
    const date2 = new Date(time2);
    const diffInMs = Math.abs(date2 - date1); // 计算两个日期之间的差值（毫秒）
    return Math.round(diffInMs / (1000 * 60)); // 将毫秒转换为分钟
}

function formatDate(date, format) {
    if (!date) return;
    if (!format) format = "yyyy-MM-dd";
    switch (typeof date) {
        case "string":
            date = new Date(date.replace(/-/, "/"));
            break;
        case "number":
            date = new Date(date);
            break;
    }
    if (!date instanceof Date) return;
    var dict = {
        "yyyy": date.getFullYear(),
        "M": date.getMonth() + 1,
        "d": date.getDate(),
        "H": date.getHours(),
        "m": date.getMinutes(),
        "s": date.getSeconds(),
        "MM": ("" + (date.getMonth() + 101)).substr(1),
        "dd": ("" + (date.getDate() + 100)).substr(1),
        "HH": ("" + (date.getHours() + 100)).substr(1),
        "mm": ("" + (date.getMinutes() + 100)).substr(1),
        "ss": ("" + (date.getSeconds() + 100)).substr(1)
    };
    return format.replace(/(yyyy|MM?|dd?|HH?|ss?|mm?)/g, function () {
        return dict[arguments[0]];
    });
}

function CancelChina(title) {
    if (!title) {
        return "";
    }
    if (title == "&nbsp;") {
        return "";
    }

    title = title.replace(/，/g, ',');
    title = title.replace(/油钢/g, "@@1");
    title = title.replace(/轴承钢/g, "@@2");
    title = title.replace(/弹簧钢/g, "@@3");
    title = title.replace(/吊孔/g, "@@4");
    title = title.replace(/割圆/g, "@@5");
    var reg = /[\u4E00-\u9FA5]/g;
    var result = title.replace(reg, '');
    result = result.replace(')', '');
    result = result.replace('(', '');
    result = result.replace('）', '');
    result = result.replace('（', '');
    result = result.replace('<', '');
    result = result.replace('>', '');
    result = result.replace(/@@1/g, "油钢");
    result = result.replace(/@@2/g, "轴承钢");
    result = result.replace(/@@3/g, "弹簧钢");
    result = result.replace(/@@4/g, "吊孔");
    result = result.replace(/@@5/g, "割圆");
    return result;
}
function CancelChinaall(title) {
    if (!title) {
        return "";
    }

    var reg = /[\u4E00-\u9FA5]/g;
    var result = title.replace(reg, '');
    title = title.replace(/，/g, ',');
    result = result.replace(')', '');
    result = result.replace('(', '');
    result = result.replace('）', '');
    result = result.replace('（', '');
    result = result.replace('<', '');
    result = result.replace('>', '');

    return result;
}
String.prototype.lTrim = function (s) {
    s = (s ? s : "\\s");                            //没有传入参数的，默认去空格
    s = ("(" + s + ")");
    var reg_lTrim = new RegExp("^" + s + "*", "g");     //拼正则
    return this.replace(reg_lTrim, "");
};



String.prototype.rTrim = function (s) {
    s = (s ? s : "\\s");
    s = ("(" + s + ")");
    var reg_rTrim = new RegExp(s + "*$", "g");
    return this.replace(reg_rTrim, "");
};



String.prototype.trim = function (s) {
    s = (s ? s : "\\s");
    s = ("(" + s + ")");
    var reg_trim = new RegExp("(^" + s + "*)|(" + s + "*$)", "g");
    return this.replace(reg_trim, "");
};


function ReturnEmpty(obj) {
    if (!!obj) {
        return obj.replace("&nbsp;", "");
    }
    else {
        return "";
    }

}

function DateDiff(sDate1, sDate2) {    //sDate1和sDate2是2006-12-18格式  
    var aDate, oDate1, oDate2, iDays
    aDate = sDate1.split("-")
    oDate1 = new Date(aDate[1] + '-' + aDate[2] + '-' + aDate[0]) //转换为12-18-2006格式  
    aDate = sDate2.split("-")
    oDate2 = new Date(aDate[1] + '-' + aDate[2] + '-' + aDate[0])
    iDays = parseInt(Math.abs(oDate1 - oDate2) / 1000 / 60 / 60 / 24) //把相差的毫秒数转换为天数  
    return iDays
}

function DateCompare(sDate1, sDate2) {    //sDate1和sDate2是2006-12-18格式  
    const date1 = new Date(sDate1);
    const date2 = new Date(sDate2);

    if (date1.getTime() > date2.getTime()) {
        return true;
    } else if (date1.getTime() < date2.getTime()) {
        return false;
    } else {
        return false;
    }
}

function getNowYMD() {
    var date = new Date();
    var seperator1 = "-";
    var seperator2 = ":";
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = year + seperator1 + month + seperator1 + strDate
    return currentdate;
}
function getNowTime() {
    var date = new Date();
    var seperator1 = "-";
    var seperator2 = ":";
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = year + seperator1 + month + seperator1 + strDate
            + " " + date.getHours() + seperator2 + date.getMinutes()
            + seperator2 + date.getSeconds();
    return currentdate;
}

function compare(value1, value2) {
    if (value1 < value2) {
        return 1;
    } else if (value1 > value2) {
        return -1;
    } else {
        return 0;
    }
}


function BindDropDepartmentName(ControlId) {
    getAjax('/CommonModule/Department/ListJson', { CompanyId: "" }, function (data) {
        var itemjson = eval("(" + data + ")");
        $.each(itemjson, function (i) {
            $(ControlId).append($("<option></option>").val(itemjson[i].fullname).html(itemjson[i].fullname));
        });
    });
}

function round(num, decimals = 2) {
    // 处理空值
    if (num === null || num === undefined || num === '') {
        return 0;
    }

    // 转换为数字
    const n = Number(num);
    const d = Math.max(0, Math.floor(Number(decimals)));

    // 处理 NaN
    if (isNaN(n)) {
        return NaN;
    }

    // 处理整数情况(提升性能)
    if (Number.isInteger(n) && d === 0) {
        return n;
    }

    return Number(Math.round(n + 'e' + d) + 'e-' + d);
}


function preciseCalculate(num1, num2, operator, decimals = 2) {
    // 1. 参数验证
    if (num1 === null || num1 === undefined || num1 === '' ||
        num2 === null || num2 === undefined || num2 === '') {
        return NaN;
    }

    const a = Number(num1);
    const b = Number(num2);
    const d = Math.min(20, Math.max(0, Math.floor(Number(decimals) || 2)));

    if (!Number.isFinite(a) || !Number.isFinite(b)) {
        return NaN;
    }

    const factor = Math.pow(10, d);
    let result;

    switch (operator) {
        case '+':
        case 'add': {
            // 加法:转换为整数运算
            const factor1 = Math.pow(10, Math.max(getDecimals(a), getDecimals(b)));
            result = (a * factor1 + b * factor1) / factor1;
            break;
        }
        case '-':
        case 'subtract': {
            // 减法:转换为整数运算
            const factor1 = Math.pow(10, Math.max(getDecimals(a), getDecimals(b)));
            result = (a * factor1 - b * factor1) / factor1;
            break;
        }
        case '*':
        case 'multiply':
            // 乘法
            result = a * b;
            break;
        case '/':
        case 'divide':
            // 除法
            if (b === 0) return NaN;
            result = a / b;
            break;
        default:
            return NaN;
    }

    // 修正精度并四舍五入
    const corrected = Number(result.toPrecision(15));
    return Math.round(corrected * factor) / factor;
}

// 辅助函数:获取小数位数
function getDecimals(num) {
    const str = num.toString();
    const idx = str.indexOf('.');
    return idx === -1 ? 0 : str.length - idx - 1;
}