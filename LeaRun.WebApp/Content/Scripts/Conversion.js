function CountTotalMoney() {

    var _Weight = ToNumber($("#Weight").val());
    var _TheoryWeight = ToNumber($("#TheoryWeight").val());
    var _WorkWeight = ToNumber($("#WorkWeight").val());
    var Qty = ToNumber($("#Qty").val());


    if (_WorkWeight > _TheoryWeight) {
        $("#SmallAmount").val(preciseCalculate(_WorkWeight, $("#SmallCost").val(), '*', 2));//材料费用 金额
    }
    else {
        $("#SmallAmount").val(preciseCalculate($("#TheoryWeight").val(), $("#SmallCost").val(), '*', 2));//材料费用 金额
    }
    if (LType == "GC") {
        if ($("#JJtype").val() == "按公斤") {

            if ($("#Calc").val() == 0 || $("#OrderType").val() == "毛料生产") {
                $("#MaterialAmount").val(preciseCalculate(_Weight , $("#MaterialPrice").val(),'*',2));//材料费用 金额

            }
            else {
                $("#MaterialAmount").val(preciseCalculate(_TheoryWeight, $("#MaterialPrice").val(), '*', 2));//材料费用 金额

            }
            //$("#MaterialAmount").val(($("#Weight").val() * $("#MaterialPrice").val()).toFixed(2));//材料费用 金额

            //$("#HeatAmount").val(FormatCurrency($("#Weight").val() * $("#HeatPrice").val()));//加工费用 金额
        }
        else {
            $("#MaterialAmount").val(preciseCalculate($("#MaterialPrice").val(), $("#Qty").val(), '*', 2));
            //$("#HeatAmount").val(FormatCurrency($("#Qty").val() * $("#HeatPrice").val()));//加工费用 金额
        }
    }
    else {
        if ($("#ProductName").val().indexOf("空心") >= 0 || $("#ProductName").val().indexOf("水套") >= 0 || $("#SteelDescription").val().indexOf("水套") >= 0) {
            $("#MaterialAmount").val(preciseCalculate($("#管状重量").val(), $("#MaterialPrice").val(), '*', 2));//材料费用 金额

        }
        else {
            $("#MaterialAmount").val(preciseCalculate(_Weight , $("#MaterialPrice").val(), '*', 2));//材料费用 金额

        }
    }

    if (LType == "GC") {
        $("#WorkAmount").val(preciseCalculate(Qty, $("#WorkPrice").val(), '*', 2));//加工费用 金额

        // 锯工费
        // $("#JKCost").val(($("#JKCostPer").val() * $("#JKCostQty").val() * Qty).toFixed(2));
        // if ($("#JKCostQty").val() > 0) {
        //    $("#JQ").val("1");
        // }
        // else {
        //    $("#JQ").val("0");
        // }
    }
    else {
        var selejjj = 0;
        var salemcjjj = 0;
        var saleperjjj = 0;

        $('#AccessView .selected a').each(function () {
            salemcjjj = $("[id='" + $(this).attr('id') + "salemc']").val();
            saleperjjj = $("[id='" + $(this).attr('id') + "saleper']").val();
            // if (!!ToNumber(salemcjjj) && !!ToNumber(saleperjjj)) {
            $("[id='" + $(this).attr('id') + "price']").val(preciseCalculate(salemcjjj, saleperjjj, '*', 2));
            selejjj += preciseCalculate(salemcjjj, saleperjjj, '*', 2);
            // }

            //if (!!$("[id='" + $(this).attr('id') + "price']").val()) {
            //    selejjj += ToNumber($("[id='" + $(this).attr('id') + "price']").val());
            //}
        });
        // if (!!selejjj) {
            $("#WorkPrice").val(selejjj)
        $("#WorkAmount").val(preciseCalculate(selejjj, $("#Qty").val(), '*', 2))
        // }
    }
    
    $("#HeatAmount").val(FormatCurrency($("#Weight").val() * $("#HeatPrice").val()));//加工费用 金额

    var a = ToNumber($("#MianCost").val());
    var a1 = ToNumber($("#BianCost").val());
    var a2 = ToNumber($("#DDCost").val());
    var a3 = ToNumber($("#MaterialAmount").val());

    var a4 = ToNumber($("#WorkAmount").val());
    var a5 = ToNumber($("#HeatAmount").val());
    var a6 = ToNumber($("#JKCost").val());
    var a7 = ToNumber($("#SmallAmount").val());


    var _SalesAmount = round(ToNumber($("#MianCost").val()) + ToNumber($("#BianCost").val()) + ToNumber($("#DDCost").val()) + ToNumber($("#MaterialAmount").val()) + ToNumber($("#WorkAmount").val()) + ToNumber($("#HeatAmount").val()) + ToNumber($("#JKCost").val()) + ToNumber($("#SmallAmount").val()));
    $("#SalesAmount").val(_SalesAmount);//销售额
}
function CountTotalMoney1() {
    var _SalesAmount = round(ToNumber($("#DDCost").val()) + ToNumber($("#MaterialAmount").val()) + ToNumber($("#WorkAmount").val()) + ToNumber($("#HeatAmount").val()) + ToNumber($("#JKCost").val()) + ToNumber($("#SmallAmount").val()));
    $("#SalesAmount").val(_SalesAmount);//销售额
}
function value0(obj) {
    if (!$(obj).val()) {
        $(obj).val(0);
    }
}

//不带回车
function Conversion(o) {
    var e = e || window.event || arguments.callee.caller.arguments[0];
    if (e && e.keyCode == 13) {//回车
        return false;
    }
    ConversionkeyCode(o);
}


//无回车
function Conversion() {
    
    if (!$("#SteelCode").val() || !$("#OrderType").val()) {
        return false;
    }

    if ($("#Thick").val() == "" || $("#Weigh").val() == "" || $("#Length").val() == "" ) {
        return false;
    }

    var OrderType = $("#OrderType").val();
    ////理论重量=（厚计价余量厚 * 宽+计价余量宽 * 高+计价余量高）*密度*数量
    var Thick = ToNumber($("#Thick").val());
    var Weigh = ToNumber($("#Weigh").val());
    var Length = ToNumber($("#Length").val());
    var BoreSize = ToNumber($("#BoreSize").val());
    //if (Thick == 0) {
    //    $("#AddThick").val(0);
    //}
    //if (Weigh == 0) {
    //    $("#AddWeigh").val(0);
    //}
    //if (Length == 0) {
    //    $("#AddLength").val(0);
    //}
    var AddThick = ToNumber($("#AddThick").val());
    var AddWeigh = ToNumber($("#AddWeigh").val());
    var AddLength = ToNumber($("#AddLength").val());
    var BoreSizeAdd = ToNumber($("#BoreSizeAdd").val());
    var Qty = ToNumber($("#Qty").val());
    var _Weight = 0;
    var _TheoryWeight = 0;
    var _WorkWeight = 0;

    ////圆钢重量重新计算
    //AjaxJsonNoMassge("/CommonModule/Steel/GetSteelEntityByCode", { SteelCode: $("#SteelCode").val() }, function (steeldata) {
    //    Density = ToNumber(steeldata.Density);
    //});
    //if (Thick != 0 && Length != 0 && Weigh == 0) {
    //    _Weight = (((((Thick + AddThick) / 2) * ((Thick + AddThick) / 2) * 3.1415 * ((Length + AddLength))) * ToNumber(Density)) / 1000000);
    //    if (!!BoreSize) {
    //        _Weight = _Weight - Length * (BoreSize / 2) * (BoreSize / 2) * 3.1415 * ToNumber(Density) / 1000000;
    //    }
    //}
    //else {
    //    _Weight = ((((Thick + AddThick) * (Weigh + AddWeigh) * (Length + AddLength)) * ToNumber(Density)) / 1000000);
    //}
    ////打底重量
    if (temp5 == 0) {
        $("#LimitWeight").val(LimitWeight);
    }
    //if (ToNumber($("#LimitWeight").val()) > 0 && ToNumber($("#LimitWeight").val()) > _Weight) {
    //    _Weight = ToNumber($("#LimitWeight").val());
    //}
    //if ($("#SteelCode").val() == "FY"  ) {
    //    _Weight = 0;
    //}
    //_Weight = (_Weight * Qty);
    //if (!!_Weight) {
    //    _Weight = _Weight.toFixed(2);
    //    $("#Weight").val(_Weight);//理论重量
    //}
    //else {
    //    $("#Weight").val("0");//理论重量
    //}
    //$("#TheoryWeight").val($("#Weight").val());//理论重量

    var GasCuttingSteel = $("#GasCuttingSteel").prop("checked") == true ? 1 : 0;
    var LiftingHole = $("#LiftingHole").prop("checked") == true ? 1 : 0;
    AjaxJsonNoMassge("/CommonModule/SteelPrice/GetSteelPrice", { OrderType: OrderType, CustomerId: GetQuery('CustomerId'), SteelCode: $("#SteelCode").val(), Thick: Thick, Weigh: Weigh, Length: Length, AddThick: AddThick, AddWeigh: AddWeigh, AddLength: AddLength, BoreSize: BoreSize, LimitWeight: ToNumber($("#LimitWeight").val()), Technology: $("#Technology").val(), Qty: Qty, GasCuttingSteel: GasCuttingSteel, LiftingHole: LiftingHole, CustMaterial: CustMaterial, BoreSizeAdd: $("#BoreSizeAdd").val() }, function (data) {
        
        
        if (!!ToNumber(data.Weight)) {
            _Weight = preciseCalculate(data.Weight, Qty, '*', 2);
            $("#Weight").val(round(_Weight));//理论重量
        }
        else {
            $("#Weight").val("0");//理论重量
        }
        if (!!ToNumber(data.TheoryWeight)) {
            _TheoryWeight = preciseCalculate(data.TheoryWeight, Qty, '*', 2);
            $("#TheoryWeight").val(round(_TheoryWeight));//理论重量
        }
        else {
            $("#TheoryWeight").val("0");//理论重量
        }

        if (LType == "GC") {
            $("#Weight").val(round(_Weight - ToNumber(data.内孔重量) * Qty));
            $("#TheoryWeight").val(round(_TheoryWeight - ToNumber(data.内孔重量) * Qty));//理论重量
        }
        


        if (!!ToNumber(data.WorkWeight)) {
            _WorkWeight = preciseCalculate(data.WorkWeight, Qty, '*', 2);
            $("#WorkWeight").val(round(_WorkWeight));//理论重量
        }
        else {
            $("#WorkWeight").val("0");//理论重量
        }


        $("#WorkWeight").val("0");//理论重量




        $("#内孔重量").val(preciseCalculate(ToNumber(data.内孔重量), Qty, '*', 2));
    $("#管状重量").val(preciseCalculate(ToNumber(data.管状重量), Qty, '*', 2));
    $("#两平面面积").val(preciseCalculate(ToNumber(data.两平面面积), Qty, '*', 2));
    $("#四侧面积").val(preciseCalculate(ToNumber(data.四侧面积), Qty, '*', 2));
    $("#六面面积").val(preciseCalculate(ToNumber(data.六面面积), Qty, '*', 2));


        if (temp13 == 0 && (LType == "GC")) {
            if (!!$("#Technology").val()) {
                if (!!data.ProductName) {
                    $("#ProductName").val(data.ProductName + $("#Technology").val());
                }
                else if (!!$("#SteelCode").val()) {
                    $("#ProductName").val($("#SteelCode").val() + $("#Technology").val());
                }
                if ($("#ProductName").val().indexOf("板") >= 0) {
                    var temp = $("#ProductName").val().replace(/板/g, "");
                    $("#ProductName").val(temp + "板");
                }
            }
            else {

                var Mheipi = $("#Mheipi").prop("checked") == true ? 1 : 0;
                if (Mheipi == 1) {
                    $("#ProductName").val($("#SteelCode").val() + "净尺寸黑皮");
                }
                else {
                    $("#ProductName").val($("#SteelCode").val() + $("#OrderType").val().substr(0, 2));

                    if ($("#ProductName").val().indexOf("板") >= 0) {
                        var temp = $("#ProductName").val().replace(/板/g, "");
                        $("#ProductName").val(temp + "板");
                    }
                    if ($("#ProductName").val().indexOf("板") >= 0) {
                        var temp = $("#ProductName").val().replace(/板/g, "");
                        $("#ProductName").val(temp + "板");
                    }
                }


            }
            $("#ProductName").val($("#SteelCode").val());
            if (!Weigh) {
                $("#ProductName").val($("#ProductName").val().replace(/毛料/g, ""));
            }

            $("#ProductName").val($("#ProductName").val().replace(/精精/g, "精"));
            $("#ProductName").val($("#ProductName").val().replace(/光光/g, "光"));
            $("#ProductName").val($("#ProductName").val().replace(/毛毛/g, "毛"));
        }

        if (!KeyValue) {


            if (temp1 == 0 && $("#JJtype").val() == "按公斤") {

                $("#MaterialPrice").val(data.MaterialPrice);
            }
            if (temp2 == 0 && $("#JJtype").val() == "按公斤") {
                $("#WorkPrice").val(data.WorkPrice);
            }
            if (temp3 == 0 && $("#JJtype").val() == "按公斤" && ToNumber($("#HeatPrice").val()) == 0) {
                $("#HeatPrice").val(data.HeatPrice)
            }
            //客来料
            if (CustMaterial == "1" && temp1 == 0) {
                $("#MaterialPrice").val(0);
            }

            if (temp6 == 0 && $("#JJtype").val() == "按公斤") {
                $("#SmallCost").val(data.SmallCost);
            }
            //if (LType == "GC") {
            //    var m = $("input[name=MianType]:checked")[0].id;
            //}
            //if (temp8 == 0) {
            //    if (m == "m1") {
            //        $("#DDCostPer").val(0);
            //    }
            //    if (m == "m2") {
            //        $("#DDCostPer").val(data.DDCostPer / 2);
            //    }
            //    if (m == "m3") {
            //        $("#DDCostPer").val(data.DDCostPer);
            //    }
            //}

            //锯工费
            if (temp12 == 0) {
                $("#JKCostPer").val(data.JKCostPer)
            }


        }

        //$("#AddThick").val(data.AddThick);
        //$("#AddWeigh").val(data.AddWeigh);
        //$("#AddLength").val(data.AddLength);

        //更新折扣率和单价



        $("#DDCost").val(ToNumber($("#DDCostPer").val()) * Qty);

        if (FreeBill == "1") {
            $("#MaterialPrice").val(0);
            $("#WorkPrice").val(0);
            $("#HeatPrice").val(0);
            $("#SmallCost").val(0);
        }

     



        







        $("#WorkAmount").val(FormatCurrency(Qty * $("#WorkPrice").val()));//加工费用 金额


        //AddThick: AddThick, AddWeigh: AddWeigh, AddLength: AddLength, 


        $("#JKCost").val(round(ToNumber($("#JKCostPer").val()) * Qty));

        if (LType == "GC") {
            if (temp10 == 0) {
                //面，边
                var mm = $("input[name=MianCostUnit]:checked")[0].id;
                if (mm == "mm1") {
                    $("#MianCostArea").val(0);
                }
                if (mm == "mm2") {
                    $("#MianCostArea").val((Length + AddLength) * (Weigh + AddWeigh) / 1000000);
                }
                if (mm == "mm3") {
                    $("#MianCostArea").val((Length + AddLength) * (Weigh + AddWeigh) * 2 / 1000000);
                }
            }
            $("#MianCost").val(round(ToNumber($("#MianCostPer").val()) * ToNumber($("#MianCostArea").val()) * Qty));
            if (temp11 == 0) {
                var bb = $("input[name=BianCostUnit]:checked")[0].id;
                if (bb == "bb1") {
                    $("#BianCostLong").val(0);
                }
                if (bb == "bb2") {

                    $("#BianCostLong").val(((Length + AddLength) + (Weigh + AddWeigh)) / 1000);
                }
                if (bb == "bb3") {
                    $("#BianCostLong").val(((Length + AddLength) + (Weigh + AddWeigh)) * 2 / 1000);
                }
            }
            $("#BianCost").val(round(ToNumber($("#BianCostPer").val()) * ToNumber($("#BianCostLong").val()) * Qty));
        }
        
        //var _SalesAmount = (ToNumber($("#MianCost").val()) + ToNumber($("#BianCost").val()) + ToNumber($("#DDCost").val()) + ToNumber($("#MaterialAmount").val()) + ToNumber($("#WorkAmount").val()) + ToNumber($("#HeatAmount").val()) + ToNumber($("#JKCost").val()) + ToNumber($("#SmallAmount").val())).toFixed(2);
        //$("#SalesAmount").val(_SalesAmount);//销售额

        CountTotalMoney();

    });

}


function ConversionAdd() {
    if (!KeyValue) {
        if (!$("#SteelCode").val()) {
            $("#AddLength").val(0);
            $("#AddWeigh").val(0);
            $("#AddThick").val(0);
        }

        if ($("#OrderType").val() != "毛料生产") {
            $("#Mheipi").prop('disabled', true);
        }
        else {
            $("#Mheipi").prop('disabled', false);

        }
        $('.form').customInput();

        if (!$("#SteelCode").val()) {
            return;
        }

        var Thick = ToNumber($("#Thick").val());
        var Weigh = ToNumber($("#Weigh").val());
        var Length = ToNumber($("#Length").val());


        AjaxJsonNoMassge("/CommonModule/SteelPrice/GetAdd", { OrderType: $("#OrderType").val(), CustomerId: GetQuery('CustomerId'), SteelCode: $("#SteelCode").val() }, function (data) {
            if (!!data.AddLength) {
                $("#AddLength").val(data.AddLength);
            }
            if (!!data.AddWeigh) {
                $("#AddWeigh").val(data.AddWeigh);
            }
            if (!!data.AddThick) {
                $("#AddThick").val(data.AddThick);
            }

            if (!!data.Fang) {
                $("#Fang").val(data.Fang);
            }
            if (!!data.Kprocess) {
                $("#Kprocess").val(data.Kprocess);
            }
            
            if (!!data.Remark) {
                $("#Remark").val(data.Remark.replace("&nbsp;", ""));
            }
            if (!!data.RemarkKL) {
                $("#RemarkKL").val(data.RemarkKL.replace("&nbsp;", ""));
            }
            if (!!data.NowStatus) {
                $("#NowStatus").val(data.NowStatus.replace("&nbsp;", ""));
            }


            if (!!data.KSize) {
                $("#KSize").val(data.KSize.replace("&nbsp;", ""));
            }
            if (!!data.ZSize) {
                $("#ZSize").val(data.ZSize.replace("&nbsp;", ""));
            }
            if (!!data.STSize) {
                $("#STSize").val(data.STSize.replace("&nbsp;", ""));
            }
            if (!!data.Hardness) {
                $("#Hardness").val(data.Hardness.replace("&nbsp;", ""));
            }
            if (!!data.BoreSizeAdd) {
                $("#BoreSizeAdd").val(data.BoreSizeAdd.replace("&nbsp;", ""));
            }
            if (!!data.CustomerProduct) {
                $("#CustomerProduct").val(data.CustomerProduct.replace("&nbsp;", ""));
            }
            
        });
    }
}
function changeProduceN() {

    if (temp13 == 0 && (LType == "GC")) {
        if (!!$("#SteelDescription").val()) {
            if ($("#SteelCode").val() == "FY" || $("#OrderType").val().substr(0, 2) == "毛料") {
                if ($("#Technology").val() != "" && $("#SteelCode").val() == "FY") {
                    $("#ProductName").val($("#Technology").val());
                }
                else {
                    $("#ProductName").val($("#SteelDescription").val());
                }
            }
            else {

                $("#ProductName").val($("#SteelDescription").val() + $("#OrderType").val().substr(0, 2));
            }
        }


        if ($("#ProductName").val().indexOf("板") >= 0) {
            var temp = $("#ProductName").val().replace(/板/g, "");
            $("#ProductName").val(temp + "板");
        }
    }
}


//function ConversionForRemark(isYorB) {
//    var e = e || window.event || arguments.callee.caller.arguments[0];
//    if (e && e.keyCode == 13) {//回车
//        return false;
//    }

//    ConversionForRemarkkeyCode(isYorB);
//}
//function ConversionForRemarkkeyCode(isYorB) {
//    if (!$("#OrderType").val()) {
//        return false;
//    }
//    var OrderType = $("#OrderType").val();
//    //if (!!SteelType && !!OrderType) {
//    //    //加工备注
//    //    if (OrderType == "精板加工") {
//    //        $("#Remark").val(JLrequire);

//    //    }
//    //    else if (OrderType == "光板加工") {
//    //        if (SteelType == "软料") {
//    //            $("#Remark").val(RLrequire);
//    //        }
//    //        if (SteelType.indexOf("硬料") >= 0) {
//    //            $("#Remark").val(YLrequire);
//    //        }

//    //    }
//    //    else if (OrderType == "毛料生产") {
//    //        $("#Remark").val(Mrequire);
//    //        //毛料自动取消倒角
//    //        //$("#IsChamfer").prop("checked", false);
//    //        $('input').customInput();
//    //    }

//    //}

//    if ($("#SteelCode").val() == "FY") {
//        $("#Thick").val("0");
//        $("#LimitThick").val("0");
//        LimitThick = 0;
//        LimitThickGB = 0;
//    }
//    var DD = $("#DD").prop("checked") == true ? 1 : 0;
//    AjaxJsonNoMassge("/CommonModule/SteelPrice/GetProductName", { OrderType: OrderType, CustomerId: GetQuery('CustomerId'), SteelCode: $("#SteelCode").val(), DD: DD, CustMaterial: CustMaterial }, function (data) {
//        if ($("#SteelDescription").val().indexOf("锻") >= 0) {
//            $("#AddLength").val(0);
//            $("#AddWeigh").val(0);
//        }
//        else {
//            if (!!data.AddWeigh) {
//                $("#AddWeigh").val(data.AddWeigh);
//            }
//            if (!!data.AddLength) {
//                $("#AddLength").val(data.AddLength);
//            }
//        }

//        if (isYorB == "B" && !!data.Fang) {
//            $("#Fang").val(data.Fang);
//        }

//        if (!!data.Remark) {
//            $("#Remark").val(data.Remark.replace("&nbsp;", ""));
//        }

//        if (!!data.DDCostPer) {
//            $("#DDCostPer").val(data.DDCostPer);
//        }
//        else {
//            $("#DDCostPer").val(0);

//        }

//    });

//}
