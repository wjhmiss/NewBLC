/*
演示脚本
*/


(function(){
		  
	//判断IE6
	var isIE6 = !+'\v1' && ([/MSIE (\d)\.0/i.exec(navigator.userAgent)][0][1] == 6);
	
	//document.getElementById简化函数
	var $ = function (id){
		return 'string' == typeof id ? document.getElementById(id) : id;
	};
	
	//页面就绪，允许你绑定一个在DOM文档载入完成后执行的函数
	var domReady = !+'\v1' ? function(f){(function(){
			try{
				document.documentElement.doScroll('left');
			} catch (error){
				setTimeout(arguments.callee, 0);
				return;
			};
			f();
		})();
	} : function(f){
		document.addEventListener('DOMContentLoaded', f, false);
	};
	
	//在页面就绪后绑定事件
	domReady(function(){
		//artDialog({content:'欢迎使用 "artDialog" 对话框组件！'});		
		
		
		//--------------------------ardDialog演示脚本开始------------------------------//
		$('btn1').onclick = function(){
			artDialog({title:'图片查看', content:'<img width="817" height="479" src="http://www.hunanyishi.cn/images/main.jpg" />', fixed:true});
			return false;
		};
		
		$('btn2').onclick = function(){
			artDialog({title:'google', url:'http://code.google.com/p/artdialog/', width:640, height:320});//url参数其实就是iframe
			return false;
		};
		
		$('btn3').onclick = function(){
			artDialog({title:'功夫兔', content:'<object width="420" height="363"><param name="movie" value="http://www.tudou.com/v/bXwe7XgTxuA"></param><param name="allowFullScreen" value="true"></param><param name="allowscriptaccess" value="always"></param><param name="wmode" value="opaque"></param><embed src="http://www.tudou.com/v/bXwe7XgTxuA" type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="true" wmode="opaque" width="420" height="363"></embed></object>', fixed:true});
			return false;
		};
		
		$('btn4').onclick = function(){
			artDialog({content:'你人品稳定么？', fixed:true, yesText:'我很稳定', style:'confirm'}, function(){alert('你是好人');}, function(){alert('你是坏人');});//style参数可以填写多个，用空格隔开。具体有什么请看皮肤css文件
			return false;
		};
		
		$('btn5').onclick = function(){
			artDialog({content:'你是坏人', style:'alert', lock:true}, function(){});//lock参数就是锁屏，它会中断用户操作，用于显示非常重要的消息。慎用
			return false;
		};
		
		$('btn6').onclick = function(e){
			//获取对象的坐标，让对话框在按钮附近弹出
			var e = e || window.event,
			_x,
			_y;
			if(e.pageX || e.pageY){
				_x = e.pageX;
				_y = e.pageY;
			} else {
				_x = e.clientX + document.body.clientLeft;
				_y = e.clientY + Math.max(document.body.scrollTop, document.documentElement.scrollTop);
			};
			
			var _this = this;
			artDialog({id:'menu_4834783',content:'<input style="width:200px;" id="M_dfd" type="text" value="hello world!" />',x:_x, y:_y, style:'noTitle noBorder'}, function(){ _this.innerHTML = $('M_dfd').value; });//使用id参数，可以防止点击弹出多个对话框
			return false;
		};
		
		$('btn7').onclick = function(){
			artDialog({content:'您收到 <strong>2</strong> 条消息', x:'right', y:'bottom', time:'100', fixed:true});//x和y坐标可以使用关键字，当然也可以使用数字
			return false;
		};
		//--------------------------ardDialog演示脚本结束------------------------------//
		




		//皮肤切换(不支持IE6，因为png皮肤的缘故)
		if (!isIE6) {
			function mySkin(s){
				$('artDialogSkin').href = s;
			};
			$('skin_0').click();
			$('skin_0').onclick = function(){
				mySkin('skin/aero/aero.css');
			};
			$('skin_1').onclick = function(){
				mySkin('skin/chrome/chrome.css');
			};
			$('skin_2').onclick = function(){
				mySkin('skin/facebook/facebook.css');
			};
			$('skin_3').onclick = function(){
				mySkin('skin/mini/mini.css');
			};
			$('skin_4').onclick = function(){
				mySkin('skin/aero/aero.css');
				$('skin_0').click();
				artDialog({content:'自己动手，丰衣足食', lock:true});
			};
			$('showBg').onclick = function(){
				if (!$('test_3544')) {
					document.getElementsByTagName('body')[0].className = 'showBg';
					artDialog({id:'test_3544', content:'<div id="topMenu" style="background:#000; width:200px; height:2em;line-height:2em;text-align:center; filter:alpha(opacity=70); opacity:0.7;">[<a href="#" style="color:#FFF" onclick="showWin();return false">打开新对话框</a>]&nbsp;&nbsp;[<a id="bgCloseBtn" href="#" style="color:#FFF" onclick="bgShow();return false">关闭</a>]</div>', x:'left', y:'top', style:'noSkin', fixed:true});
				} else {
					bgShow();
				};
				return false;
			};
		};
		
		//运行代码[不支持Chrome]
		$('runCodeBtn').onclick = function(){
			var fn = $('runCode').value;
			$('runCodeDiv').innerHTML = '<iframe style="display:none" id="runCodeIframe"></iframe>';
			var fw = $('runCodeIframe').contentWindow;
			var f = fw.document;
			f.contentEditable = true; 
			//f.open();
			f.writeln('<!DOCTYPE><html><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><script type="text/javascript">try{window.parent.'+fn+'}catch(e){alert(e.description);};</script><body></body></html>');
			//f.close();
			return false;
		};
		
		
		var mailName = 'admin' + '@' + 'plane' + 'art' + '.cn';
		$('myMail').innerHTML = mailName;
	});

})();

//显示一个新对话框
function showWin(){
			artDialog({content:'欢迎使用 "artDialog" 对话框组件！', fixed:true, lock:false,style:'succeed'}, function(){artDialog({title:' ', content:'谢谢观赏', lock:true, time:2})});
};

//显示背景
function bgShow(){
	var body = document.getElementsByTagName('body')[0];
	if(body.className != 'showBg') {
		body.className = 'showBg';
		document.getElementById('bgCloseBtn').innerHTML = '关闭背景';
	} else {
		body.className = '';
		document.getElementById('bgCloseBtn').innerHTML = '打开背景';
	};
};