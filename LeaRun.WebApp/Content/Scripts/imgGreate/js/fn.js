/* Design by jason.leung, QQ,147430218, 新浪微博,@切片面包 */
/* 酷站代码整理 转载请注明出处 http://www.5icool.org */
function getClass(oParent,sClass)
{
	var value=[];
	var Ele=oParent.getElementsByTagName('*');
	for (var i=0; i<Ele.length; i++)
	{
		if(Ele[i].className==sClass)
		{
			value.push(Ele[i]);
		}
	}
	return value;
}
window.onload=function()
{
	var oSmall=document.getElementById('smallpic');
	var oBig=document.getElementById('bigpic');
	var oSpan=getClass(oSmall,'mark')[0];
	var oImg=oBig.getElementsByTagName('img')[0];
	
	oSmall.onmouseover=function()
	{
		oBig.style.display='block';
		oSpan.style.display='block';
	}
	
	oSmall.onmouseout=function()
	{
		oBig.style.display='none';
		oSpan.style.display='none';
	}
	
	oSmall.onmousemove=function(ev)
	{
		var oEvent=ev||event;
		var X=oEvent.clientX-oSpan.offsetWidth/2;
		var Y=oEvent.clientY-oSpan.offsetHeight/2;
		if (X<0)
		{
			X=0;
		}
		else if (X>oSmall.offsetWidth-oSpan.offsetWidth)
		{
			X=oSmall.offsetWidth-oSpan.offsetWidth;
		}
		if (Y<0)
		{
			Y=0;
		}
		else if (Y>oSmall.offsetHeight-oSpan.offsetHeight)
		{
			Y=oSmall.offsetHeight-oSpan.offsetHeight;
		}
		oSpan.style.left=X+'px';
		oSpan.style.top=Y+'px';
		
		var Xp=X/(oSmall.offsetWidth-oSpan.offsetWidth);
		var Yp=Y/(oSmall.offsetHeight-oSpan.offsetHeight);
		
		oImg.style.left=-Xp*(oImg.offsetWidth-oBig.offsetWidth)+'px';
		oImg.style.top=-Yp*(oImg.offsetHeight-oBig.offsetHeight)+'px';
	}
	
}