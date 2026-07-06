//
/*检测是否移动设备来访否则跳转*/
//if(!browserRedirect()){
//    location.href = '/Login/Index';
//}
/*页面加载完毕时运行函数
-----------------------------------------------------*/
$(document).ready(function(e) {
    if (!((window.DocumentTouch && document instanceof DocumentTouch) || 'ontouchstart' in window)) {
        $.getScript("../jqmobi/plugins/af.desktopBrowsers.js");
	}
});
/*当窗体大小发生改变时缩放内容元素的大小*/
$(window).resize(function(){
	InitObjectWidth($(".entry iframe"));
	InitObjectWidth($(".entry embed"));
	InitObjectWidth($(".entry video"));
});
/*当窗体加载时缩放内容元素的大小*/
$(window).load(function(){
	InitObjectWidth($(".entry iframe"));
	InitObjectWidth($(".entry embed"));
	InitObjectWidth($(".entry video"));
});

/*全局函数
-----------------------------------------------------*/
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
	        alert('已禁止本次访问：您必须使用微信访问！');
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
//初始化页面无素的大小
function InitObjectWidth(obj){
	var maxWidth=$(".entry").width();
	obj.each(function(){
		if($(this).width()>maxWidth){
			var wh=$(this).width()/$(this).height();
			var newHeight=maxWidth/wh;
			$(this).width(maxWidth);
			$(this).height(newHeight);
		}
	});
}
//写Cookie
function addCookie(objName, objValue, objHours) {
    var str = objName + "=" + escape(objValue);
    if (objHours > 0) {//为0时不设定过期时间，浏览器关闭时cookie自动消失
        var date = new Date();
        var ms = objHours * 3600 * 1000;
        date.setTime(date.getTime() + ms);
        str += "; expires=" + date.toGMTString();
    }
    document.cookie = str;
}

//读Cookie
function getCookie(objName) {//获取指定名称的cookie的值
    var arrStr = document.cookie.split("; ");
    for (var i = 0; i < arrStr.length; i++) {
        var temp = arrStr[i].split("=");
        if (temp[0] == objName) return unescape(temp[1]);
    }
    return "";
}
/*页面通用函数
-----------------------------------------------------*/
/*切换验证码*/
function ToggleCode(obj, codeurl) {
    $(obj).children("img").eq(0).attr("src", codeurl + "?time=" + Math.random());
	return false;
}
/*搜索查询*/
function SiteSearch(send_url, divTgs, channel_name) {
    var strwhere = "";
    if (channel_name !== undefined) {
        strwhere = "&channel_name=" + channel_name
    }
	var str = $.trim($(divTgs).val());
	if (str.length > 0 && str != "输入关健字") {
	    window.location.href = send_url + "?keyword=" + encodeURI($(divTgs).val()) + strwhere;
	}
	return false;
}
/*下载链接*/
function downLink(point, linkurl){
	if(point > 0){
		dialog({
			title:'提示',
			content:"下载需扣除" + point + "个积分<br />重复下载不扣积分，需要继续吗？",
			okValue:'确定',
			ok:function (){
				window.location.href = linkurl;
			},
			cancelValue: '取消',
			cancel: function (){}
		}).showModal();
	}else{
		window.location.href = linkurl;
	}
	return false;
}

//发送验证邮件
function sendEmail(username, sendurl) {
	if(username == ""){
		alert("对不起，用户名不允许为空！");
		return false;
	}
	//提交
	$.ajax({
		url: sendurl,
		type: "POST",
		timeout: 60000,
		data: { "username": username },
		dataType: "json",
		success: function (data, type) {
			if (data.status == 1) {
				var d = dialog({content:data.Message}).show();
				setTimeout(function () {
					d.close().remove();
				}, 2000);
			} else {
				alert(data.Message);
			}
		},
		error: function(XMLHttpRequest, textStatus, errorThrown){
			 alert("状态：" + textStatus + "；出错提示：" + errorThrown);
		}
	});
}

//发送手机短信验证码
var wait = 0; //计算变量
function sendSMS(btnObj, valObj, sendUrl) {
	if($(valObj).val() == ""){
		dialog({title:'提示', content:'对不起，请填写手机号码后再获取！', okValue:'确定', ok:function (){}}).showModal();
		return false;
	}
	//发送AJAX请求
	$.ajax({
		url: sendUrl,
		type: "POST",
		timeout: 60000,
		data: { "mobile": $(valObj).val() },
		dataType: "json",
		beforeSend: function(XMLHttpRequest) {
			$(btnObj).unbind("click").removeAttr("onclick"); //移除按钮事件
		},
		success: function (data, type) {
			if (data.status == 1) {
				wait = data.time * 60; //赋值时间
				time(); //调用计算器
				var d = dialog({content:data.Message}).show();
				setTimeout(function () {
					d.close().remove();
				}, 2000);
			} else {
				$(btnObj).removeClass("gray").text("发送确认码");
				$(btnObj).bind("click", function(){
					sendSMS(btnObj, valObj, sendurl); //重新绑定事件
				});
				dialog({title:'提示', content:data.Message, okValue:'确定', ok:function (){}}).showModal();
			}
		},
		error: function(XMLHttpRequest, textStatus, errorThrown){
			$(btnObj).removeClass("gray").text("发送确认码");
			$(btnObj).bind("click", function(){
				sendSMS(btnObj, valObj, sendurl); //重新绑定事件
			});
			dialog({title:'提示', content:"状态：" + textStatus + "；出错提示：" + errorThrown, okValue:'确定', ok:function (){}}).showModal();
		}
	});
	//倒计时计算器
	function time(){
		if (wait == 0) {
			$(btnObj).removeClass("gray").text("发送确认码");
			$(btnObj).bind("click", function(){
				sendSMS(btnObj, valObj, sendurl); //重新绑定事件
			});
		}else{
			$(btnObj).addClass("gray").text("重新发送(" + wait + ")");
			wait--;
			setTimeout(function() {
				time(btnObj);
			},1000)
		}
	}
}

//单击执行AJAX请求操作
function clickSubmit(sendUrl){
	$.ajax({
		type: "POST",
		url: sendUrl,
		dataType: "json",
		timeout: 20000,
		success: function(data, textStatus) {
			if (data.status == 1){
				var d = dialog({content:data.Message}).show();
				setTimeout(function () {
					d.close().remove();
					location.reload();
				}, 2000);
			} else {
				alert(data.Message);
			}
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			alert("状态：" + textStatus + "；出错提示：" + errorThrown);
		}
	});
}

/*全选取消按钮函数*/
function checkAll(chkobj){
	if($(chkobj).text()=="全选"){
	    $(chkobj).text("取消");
		$(".checkall").prop("checked", true);
	}else{
    	$(chkobj).text("全选");
		$(".checkall").prop("checked", false);
	}
}

/*执行删除操作*/
function ExecDelete(sendUrl, checkValue, urlId){
	var urlObj = $(urlId);
	//检查传输的值
	if (!checkValue) {
		alert("对不起，请选中您要操作的记录！");
        return false;
	}
	dialog({
        title: '提示',
        content: '删除记录后不可恢复，您确定吗？',
        okValue: '确定',
        ok: function () {
			$.ajax({
				type: "POST",
				url: sendUrl,
				dataType: "json",
				data: {
					"checkId":  checkValue
				},
				timeout: 20000,
				success: function(data, textStatus) {
					if (data.status == 1){
						var tipdialog = dialog({content:data.Message}).show();
						setTimeout(function () {
							tipdialog.close().remove();
							if(urlObj){
								location.href = urlObj.val();
							}else{
								location.reload();
							}
						});
					} else {
						alert(data.Message);
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					alert("状态：" + textStatus + "；出错提示：" + errorThrown);
				}
			});
		},
        cancelValue: '取消',
        cancel: function () { }
	}).showModal();
}

/*表单AJAX提交封装(包含验证)*/
function AjaxInitForm(formId, btnId, isDialog, urlId){
	var formObj = $(formId); //表单元素
	var btnObj = $(btnId); //按钮元素
	var urlObj = $(urlId); //隐藏域元素
	
	formObj.Validform({
		tiptype:3,
		callback:function(form){
			$.ajax({
				type:"post",
				url:form.attr("url"),
				data:form.serialize(),
				dataType:"json",
				beforeSend:function(formData, jqForm, options){
					btnObj.prop("disabled", true);
					btnObj.val("请稍候...");
				},
				success:function(data, textStatus){
				    if (data.Code == "1") {
						btnObj.val("提交成功");
						//是否提示，默认不提示
						if(isDialog == 1){
						    var d = dialog({ content: data.Message }).show();
							setTimeout(function () {
								d.close().remove();
								if(data.url){
									location.href = data.url;
								}else if(urlObj.length > 0 && urlObj.val() != ""){
									location.href = urlObj.val();
								}else{
									location.reload();
								}
							}, 2000);
						}else{
							if(data.url){
								location.href = data.url;
							}else if(urlObj){
								location.href = urlObj.val();
							}else{
								location.reload();
							}
						}
					}else{
						alert(data.Message);
						btnObj.prop("disabled", false);
						btnObj.val("重新提交");
					}
				},
				error:function(XMLHttpRequest, textStatus, errorThrown){
					alert("状态：" + textStatus + "；出错提示：" + errorThrown);
					btnObj.prop("disabled", false);
					btnObj.val("重新提交");
				}
			});
			return false;
		}
	});
}

/*AJAX加载评论列表*/
var comment_page_index = 1; //评论页码全局变量
function CommentAjaxList(btnObj,listDiv, pageSize, pageCount, sendUrl){
	//计算总页数
	var pageTotal = Math.ceil(pageCount / pageSize);
	if(comment_page_index > pageTotal){
		return false;
	}
	//发送AJAX请求
	$.ajax({
		type:"post",
		url:sendUrl + "&page_size=" + pageSize + "&page_index=" + comment_page_index,
		dataType:"json",
		beforeSend:function(){
			$(btnObj).prop("disabled", true);
			$(btnObj).val('正在加载...');
		},
		success:function(data){
			var strHtml = '';
			for(var i in data){
				strHtml += '<li>' + 
				'<div class="inner">' +
				'<p>' + unescape(data[i].content) + '</p>' +
				'<div class="meta">' +
				'<span class="blue">' + data[i].user_name + '</span>\n' +
				'<span class="time">' + data[i].add_time + '</span>' +
				'</div>' +
				'</div>';
				if(data[i].is_reply == 1){
					strHtml += '<div class="answer">' +
					'<div class="meta">' +
					'<span class="right time">' + data[i].reply_time + '</span>' +
					'<span class="blue">管理员回复：</span>' +
					'</div>' + 
					'<p>' + unescape(data[i].reply_content) + '</p>' +
					'</div>';
				}
				strHtml += '</li>';
			}
			//添加到列表
			if(comment_page_index==1){
				$(listDiv).html(strHtml);
			}else{
				$(listDiv).append(strHtml);
			}
			comment_page_index++; //当前页码加一
			if(comment_page_index > pageTotal){
				$(btnObj).hide();
			}
		},
		complete:function(){
			$(btnObj).prop("disabled", false);
			$(btnObj).val('加载更多评论');
		}
	});
}

//初始化视频播放器需配合ckplayer.js使用
function initCKPlayer(boxId, videoSrc, playerSrc){
	var flashvars={
        f:videoSrc,
        c:0,
        loaded:'loadedHandler'
    };
    var video=[videoSrc];
    CKobject.embed(playerSrc,boxId,'video_v1','100%','100%',true,flashvars,video);
}

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
function AjaxJsonNoMassge(url, postData, callBack) {
    try {
        $.ajax({
            url: RootPath() + url,
            type: "post",
            data: postData,
            dataType: "json",
            async: true,
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
                id.val(value);
                break;
            default:

                if (id[0] && id[0].tagName.toUpperCase() == "LABEL") {
                    id.html(value);
                }
                else {
                    id.val(value);
                }
                break;
        }
    }
}
function IsNullOrEmpty(str) {
    var isOK = true;
    if (str == undefined || str == "" || str == 'null') {
        isOK = false;
    }
    return isOK;
}
function BindDropItem(ControlId, Code, Memo) {
    $(ControlId).html("");
    if (IsNullOrEmpty(Memo)) {
        $(ControlId).append("<option value=''>" + Memo + "</option>");
    }
    AjaxJsonNoMassge('/CommonModule/DataDictionary/BinDingItemsJson', { Code: Code }, function (data) {
        $.each(data, function (i) {
            $(ControlId).append($("<option></option>").val(data[i].Code).html(data[i].FullName));
        });
    });
}

function BindDropItemName(ControlId, Code, Memo) {
    $(ControlId).html("");
    if (IsNullOrEmpty(Memo)) {
        $(ControlId).append("<option value=''>" + Memo + "</option>");
    }
    AjaxJsonNoMassge('/CommonModule/DataDictionary/BinDingItemsJson', { Code: Code }, function (data) {
        var itemjson = eval("(" + data + ")");
        $.each(itemjson, function (i) {
            $(ControlId).append($("<option></option>").val(itemjson[i].FullName).html(itemjson[i].FullName));
        });
        Loading(false);
    });
}


function BindDropSteelCode(ControlId,  Memo) {
    $(ControlId).html("");
    if (IsNullOrEmpty(Memo)) {
        $(ControlId).append("<option value=''>" + Memo + "</option>");
    }
    AjaxJsonNoMassge('/CommonModule/Steel/BindDropSteelCode', { Code: "" }, function (data) {
        $.each(data, function (i) {
            $(ControlId).append($("<option></option>").val(data[i].SteelCode).html(data[i].SteelCode));
        });
    });
}
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

function IsChecked(id) {
    var isOK = true;
    if (id == undefined || id == "" || id == 'null' || id == 'undefined') {
        isOK = false;
        tipDialog('错误：您没有选中任何项,请您选中后再操作。', -1);
    } else if (id.split(",").length > 1) {
        isOK = false;
        tipDialog('错误：很抱歉,一次只能选择一条记录。', -1);
    }
    return isOK;
}

function tipDialog(msg, type,url) {
   
    if (type >= 1) {
        var d = dialog({ content: msg }).show();
        setTimeout(function () {
            d.close().remove();
            location.href = url;
        }, 2000);
    }
    else {
        dialog({
            title: '提示', content: msg, okValue: '确定', ok: function () {
                if (!!url) {
                    history.back(-1);
                }
            }
        }).showModal();

    }
}


function formatDate(date, format) {
    if (!date) return;
    if (!format) format = "yyyy-MM-dd";
    switch (typeof date) {
        case "string":
            date = new Date(date.replace(/-/, "/").replace('T',' '));
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
                reVal += '"' + id + '"' + ':' + '"' + $.trim(value) + '",'
                break;
        }
    });
    reVal = reVal.substr(0, reVal.length - 1);
    return jQuery.parseJSON('{' + reVal + '}');
}

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
function ChangeDateFormat(cellval)//json时间转换为正常时间    
{
    try {
        var date = new Date(parseInt(cellval.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return date.getFullYear() + "-" + month + "-" + currentDate;
    } catch (e) {
        return "";
    }
}