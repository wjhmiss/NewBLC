//====================初始化验证表单====================
$(function(){
	//提交表单
	$("#btnSubmit").bind("click", function() {
		if($("#txtUserName").val()=="" || $("#txtPassword").val()==""){
			$("#msgtips").show();
			$("#msgtips").text("请填写用户名和登录密码！");
			return false;
		}
		$.ajax({
            type: "POST",
            url: $("#loginform").attr("url"),
			dataType: "json",
            data: {
                "Account": $("#txtUserName").val(),
                "Password": escape($.md5($("#txtPassword").val())),
                "PasswordMobile": escape($("#txtPassword").val())
            },
            timeout: 20000,
			beforeSend: function(XMLHttpRequest) {
				$("#btnSubmit").attr("disabled", true);
				$("#msgtips").show();
				$("#msgtips").text("正在登录，请稍候...");
			},
            success: function(data, textStatus) {
                if (data == 3){
                    window.location.href = RootPath() + "/WeiWebSite/WeiLogin/UserCenter";
                } else {
                    if (data == -1) {
                        $("#btnSubmit").attr("disabled", false);
                        $("#msgtips").text("错误：账户不存在");
                        return false;
                    }
                    if (data == "lock") {
                        $("#btnSubmit").attr("disabled", false);
                        $("#msgtips").text("错误：账户不存在");
                        return false;
                    }
                    if (data == "error") {
                        $("#btnSubmit").attr("disabled", false);
                        $("#msgtips").text("错误：密码错误");
                        return false;
                    }
                    $("#btnSubmit").attr("disabled", false);
                    $("#msgtips").text("错误：登录失败");
                }
            },
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				$("#msgtips").text("状态：" + textStatus + "；出错提示：" + errorThrown);
				$("#btnSubmit").attr("disabled", false);
			} 
        });
		return false;
    });
});