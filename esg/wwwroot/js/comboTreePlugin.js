(function ($, window, document, undefined) {
    let comboTreePlugin = 'comboTree',
        //定义一个存储数据的数组，用于下面重复选择判断，删除标签,
        defaults = {
            source: [],
            isMultiple: false,
            cascadeSelect: false,
            selected: [],
            selectedlength: 3
        },
        tempstructerarray = [];

    let ComboTree = function (element, options) {
        this.options = $.extend({}, defaults, options);
        this.element = element;
        this.ulcontainer = null;
        this.oliIdArray = [];
        this.copysource = [];
        this.roleSelect = null;
        this.copydom = null;
        this.comboxinput = null;
        this.comboxinputcontainer = null;
        this.comboxulcontainer = null;
        this.selectul = null;
        this.myplaceholder = null;
        this.noresults = null;
        this.init();
    };

    ComboTree.prototype.init = function () {
        $.extend(true, this.copysource, this.options.source);
        this.initstruct();
        this.initdom();
        this.ulcontainer = $(this.element).find('div[_id=comboxulcontainer]');
        this.comboxinput = $(this.element).find('input[_id=comboxinput]');
        this.roleSelect = $(this.element).find('div[_id=role_select]');
        this.comboxinputcontainer = $(this.element).find('div[_id=comboxinputcontainer]');
        this.comboxulcontainer = $(this.element).find('div[_id=comboxulcontainer]')
        this.copydom = this.selectul.clone();
        this.myplaceholder = $(this.element).find('span[_id=myplaceholder]');
        this.noresults = $(this.element).find('.noresults');
        this.initevent();

    };

    ComboTree.prototype.initstruct = function () {
        for (let i = 0; i < this.copysource.length; i++) {
            this.copysource[i]['class'] = 'first'
        }
    };

    ComboTree.prototype.initevent = function () {
        let _this = this;

        //点击输入框时候
        $(this.element).on('click', function (event) {

            event = event || window.event;

            let closet = $(event.target).closest('.input-keyword-wrap'),icon = null;
            if (($(event.target).hasClass('imitationSelect') || $(event.target).hasClass('fa')) && closet.length > 0) {
                $(_this.element).find('.drop-down-wrap').toggle();
                if (!icon) {
                    icon = $(_this.element).find('.input-keyword-wrap>i');
                }
                if (icon.hasClass("fa-caret-down")) {
                    icon.removeClass("fa-caret-down").addClass("fa-caret-up"); //点击input选择适合，小图标动态切换
                } else {
                    icon.removeClass("fa-caret-up").addClass("fa-caret-down"); //点击input选择适合，小图标动态切换
                }
            }

            let target = $(event.target).hasClass('title-container') ? $(event.target) : $(event.target)
                .parent().hasClass('title-container') ? $(event.target).parent() : null;

            if (target) {
                // 如果已选择的大于设定的数目，并且当前是选择动作 则不执行
                if ((_this.oliIdArray.length >= _this.options.selectedlength) && !target.hasClass('actived_li') && _this.options.isMultiple) {
                    console.log('最大可选条目已设置');
                    return false;
                }

                if (target.attr('role') !== 'parent') {
                    event.target = target;
                    let oliId = target.attr("data-id");
                    if (target.hasClass('actived_li')) {
                        _this.uncheckrow(oliId);
                    } else {
                        if (!_this.options.isMultiple) { //如果是单选，则已选条目大于零，并且所点击的不是 激活状态的，不执行
                            if (_this.oliIdArray.length > 0) {
                                for (let j = 0; j < _this.oliIdArray.length; j++) {
                                    _this.uncheckrow(_this.oliIdArray[j]);
                                }
                            }
                        }
                        _this.checkrow(target, oliId, false); //第三个参数表示点击的是否是checkbox
                    }

                    if (!_this.options.isMultiple) {
                        _this.hideul();
                    }
                }
            }

            if ($(event.target).attr('type') === 'checkbox') {
                if ((_this.oliIdArray.length >= _this.options.selectedlength) && $(event.target).prop(
                        'checked')) {
                    console.log('超出最大条目');
                    return false;
                }

                let _target = $(event.target).closest('.title-container');

                if (_target.hasClass('actived_li')) {
                    _this.uncheckrow($(event.target).attr('data-id'));
                } else {
                    _this.checkrow(_target, $(event.target).attr('data-id'), true);
                }
            }

            //点击x关闭事件处理
            if ($(event.target).attr('class') === 'close') {
                _this.uncheckrow($(event.target).attr('data-id'));
            }

            let containerparent = $(event.target).attr('role') === 'parent' ? $(event.target) : $(event
                .target).parent().attr('role') === 'parent' ? $(event.target).parent() : null;

            if (!containerparent) {
                target = $(event.target).attr('tag') === 'closeitem' ?
                    $(event.target) : $(event.target).parent().attr('tag') === 'closeitem' ?
                    $(event.target).parent() : null;
            }

            if( $(event.target).attr('tag') === 'search' || $(event.target).parent().attr('tag') === 'search' ){
                _this.comboxinput.val('');
                _this.comboxinput.trigger('keyup');
            }

            if (target || containerparent) {
                let _parent = null;
                if (containerparent) {
                    _parent = containerparent
                } else {
                    _parent = target.closest('.title-container');
                }

                _parent.next().toggle();
                target = target.find('i');

                if (target.hasClass('fa-caret-down')) {
                    target.removeClass('fa-caret-down').addClass('fa-caret-right');
                } else {
                    target.removeClass('fa-caret-right').addClass('fa-caret-down');
                }
            }

            if (event.stopPropagation) {
                event.stopPropagation(); // 针对 Mozilla 和 Opera
            } else if (window.event) {
                window.event.cancelBubble = true; // 针对 IE
            }
        });

        this.comboxinput.on('keyup', function (event) {
            event = event || window.event;

            _this.selectul.find('.hide').removeClass('hide');


            // 判断搜索框里是否有内容，如果有则添加删除按钮
            if (event.currentTarget.value != "") {
              $(this).siblings('span').find('i').removeClass('fa-search').addClass('fa-close');
            } else {
              $(this).siblings('span').find('i').removeClass('fa-close').addClass('fa-search');
            }

            let lis = _this.selectul.find('li'),
                targetli = null,
                _tempattr = null;

            lis.each(function (index, item) {
                $(item).attr('matched', '');
            });
            let val = $(event.target).val();

            function getChildren(parent) {
                let lichild = parent.children('li');

                if (lichild.length) {
                    for (let i = 0, _p = lichild, len = _p.length; i < len; i++) {
                        targetli = _p.eq(i), _tempattr = targetli.attr('data-name');

                        if (_tempattr.indexOf(val) >= 0 && _tempattr !== ' ' && _tempattr !== '') {
                            targetli.attr('matched', 'matched');
                        }

                        let subulcontainer = targetli.find('>.tree-sub-body');

                        if (subulcontainer.length > 0) {
                            let _tempul = subulcontainer.find('>ul');
                            getChildren(_tempul);
                        }
                    }
                }
            }
            getChildren(_this.selectul);

            if (val.trim() !== '') {
                let lis1 = _this.selectul.find('li');
                lis1.each(function (index, item) {
                    let _item = $(item);
                    let matched = _item.find('li[matched="matched"]');
                    if (_item.length === 0) {
                        return true; //相当于continue
                    }
                    if (matched.length === 0) {
                        if (_item.attr('matched') === 'matched') { //如果当前元素匹配，则保留当前的删除它后面的所有元素

                            let _matcheditem = _item.find('>div.tree-sub-body');
                            _matcheditem.each(function (index, item) {
                                $(item).addClass('hide');
                            });
                        } else {
                            _item.addClass('hide');
                        }
                    }
                });

                let children = _this.comboxulcontainer.find('li[matched=matched]');

                if (children.length === 0) {
                    _this.selectul.hide();
                    _this.noresults.show();
                } else {
                    _this.noresults.hide();
                    _this.selectul.show();
                }

            } else {
                _this.selectul.show();
                _this.selectul.find('.hide').removeClass('hide');
                _this.noresults.hide();
            }

        });

        //点击任意地方隐藏下拉
        $(document).click(function (event) {
            _this.hideul();
        });
    };

    ComboTree.prototype.initdom = function () {
        $(this.element).append(
            '<div class="input-keyword-wrap">' +
            '<div _id="role_select" class="select-menu-input imitationSelect role_select">'+
            '<span _id="myplaceholder" class="input-tips">请选择指标</span>'+
            '</div>' +
            '<i class="fa fa-caret-down handle-arrow"></i>' +
            '</div>' +
            '<div class="drop-down-wrap">' +
            '<div _id="comboxinputcontainer" class="comboxinputcontainer keyword-search">' +
            '<input _id="comboxinput" placeholder="输入关键词搜索" type="text">' +
            '<span tag="search" class="search-icons"><i class="fa fa-search"></i></span>' +
            '</div>' +
            '<div _id="comboxulcontainer">' +
            '<div _id="noresault" class="noresults">无搜索结果</div>'+
            '<ul class="select-tree-list" _id="selectUl"></ul>' +
            '</div>' +
            '</div>');

        this.selectul = $(this.element).find('ul[_id=selectUl]');
        this.createitem(this.copysource, this.selectul);

    };

    ComboTree.prototype.createitem = function (SampleJSONData, container) {
        for (let j = 0; j < SampleJSONData.length; j++) {

            var oliName = SampleJSONData[j].title;
            var oliId = SampleJSONData[j].id;
            // li容器
            let item = $('<li data-name="' + oliName + '" data-id="' + oliId + '"></li>'),
                divitem = $('<div data-name="' + oliName + '" data-id="' + oliId +'" class="title-container"></div>');
            spanitem = $('<span tag="closeitem" class="handle-left-icons"></span>');
            divitem.append(spanitem);

            //如果是第一层，给他设置一个属性叫firstclass
            if (SampleJSONData[j]['class'] === 'first') {
                divitem.attr('role', 'parent');
            }

            if (SampleJSONData[j]['subs']) {
                spanitem.append('<i class="fa fa-caret-down"></i>');
                divitem.append(spanitem);
                divitem.attr('role', 'parent');
            }

            //放入checkbox  ,这段业务表示如果是第一层，并且在配置中写了，第一层允许选择才加入checkbox
            if (this.options.isMultiple) {
                if (SampleJSONData[j]['class'] === 'first') {
                    if (this.options.isFirstClassSelectable) {
                        appendcheckbox(divitem, oliId);
                    }
                } else {
                    if (!SampleJSONData[j]['subs']) {
                        appendcheckbox(divitem, oliId);
                    }
                }
            }
            // 放入名称
            divitem.append('<span '+'" data-id="' + oliId + 'class="title-group-name">' + oliName + '</span>');
            item.append(divitem);
            container.append(item);

            if (SampleJSONData[j]['subs']) {
                let titlediv = $('<div class="tree-sub-body"></div>');
                item.append(titlediv);
                let subul = $('<ul></ul>');
                titlediv.append(subul);
                this.createitem(SampleJSONData[j]['subs'], subul);
            }
        }


    };

    ComboTree.prototype.checkrow = function (target, oliId, ischeckbox) {
        let _this = this;
        target.addClass("actived_li"); //点击当前的添加   actived_li这个类；
        // 判断当前元素前面是否有checkbox，如果有就选中
        let inputcheckbox = target.find('input');
        if (inputcheckbox.length > 0 && !ischeckbox) {
            inputcheckbox.prop('checked', !inputcheckbox.prop('checked'));
        }
        _this.oliIdArray.push(oliId);
        _this.roleSelect.attr("data-id", _this.oliIdArray); //把当前点击的oliId赋值到显示的input的oliId里面

        if (_this.oliIdArray.length > 0) {
            _this.myplaceholder.hide();
        }

        //向input里面存放的内容，是一个span
        let item = $("<span data-id='" + oliId + "' class='input-keyword-item'></span>"),
            namespan = $("<span>" + target.attr('data-name') + "</span>"),
            checkicon = $("<i class='close' data-id='" + oliId + "' >x</i>");

        item.append(namespan);
        if (this.options.isMultiple) {
            item.append(checkicon);
        } else {
            item.addClass('single-keyword')
        }

        _this.roleSelect.append(item);
        getResult(KKK,target.attr('data-name'));
        result.push(target.attr('data-name'));
        console.log(result);
        var str = JSON.stringify({
            IndicateName1: result[0],
            IndicateName2: result[1],
            IndicateName3: result[2]
        });
        $("#bread1").html(result[0]);
        $("#bread2").html(result[1]);
        $("#bread3").html(result[2]);
        $("#MAIN").html("");
        $.ajax({
            url: "api/Report/GetIndicate4",//修改API
            type: "POST",
            contentType: "application/json",
            data: str,
        success:function(data){//alert(data);
            //此时已经拿到了四级指标
            var level4=data;
            for(var i=0;i<data.length;i++)
            {
                $("#MAIN").append("<h2 class=\"pageheader-title\">"+data[i]+"</h2><div class=\"card\"><div class=\"card-body\"> \
                <div name=\"Five\" class=\"table-responsive\"></div></div></div>");
            }
            for(var i=0;i<data.length;i++)
            {
                //针对于每一个四级指标要发送一个请求
                var str1=JSON.stringify({
                    IndicateName1: $("#bread1").html(),
                    IndicateName2: $("#bread2").html(),
                    IndicateName3: $("#bread3").html(),
                    IndicateName4: data[i]
                });
                $.ajax({
                    url: "api/Report/GetIndicate5",
                    type: "POST",
                    async: false,
                    contentType: "application/json",
                    data: str1,
                success:function(data2){//alert();
                if(rst==0||rst==1)//报表权限：可以更改
                {
                    for(var j=0;j<data2.length;j++)
                    {
                     $("[name='Five']").each(function(index){
                        if(index==i){//选择对应的四级表格插入
                            if(data2[j]["type"]==1)//定量指标
                            {
                               if(data2[j]["frequency"]==1)//按月
                               {
                                $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+"(单位:\
                                "+data2[j]["unit"]+")"+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\
                                <span class=\"badge badge-pill badge-primary\">定量指标-按月填报</span>"+"</h4>\
                                <table class=\"table table-striped table-bordered first\"> \
                                <thead id=\"head\">\
                                <tr>\
                                <th>1月</th><th>2月</th><th>3月</th>\
                                <th>4月</th><th>5月</th><th>6月</th>\
                                </tr>\
                                </thead>\
                                <tbody id=\"body\">\
                                <tr>\
                                <td><input type=\"text\" id=\"1\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"2\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"3\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"4\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"5\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"6\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                </tr>\
                                </tbody>\
                                <thead id=\"head\">\
                                <tr>\
                                <th>7月</th><th>8月</th><th>9月</th>\
                                <th>10月</th><th>11月</th><th>12月</th>\
                                </tr>\
                                </thead>\
                                <tbody id=\"body\">\
                                <tr>\
                                <td><input type=\"text\" id=\"7\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"8\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"9\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"10\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"11\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                <td><input type=\"text\" id=\"12\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></td>\
                                </tr>\
                                </tbody>\
                                </table>");
                                var jsonArr = [ {
                                    "ESG_Id" : data2[j]["esG_Id"],
                                    "Type" : data2[j]["type"],
                                }]
                                var str2 = JSON.stringify({
                                    ReportId: rid,
                                    Report_Year: ryear,
                                    dataDetails: jsonArr
                                });
                                var _Type=data2[j]["type"];
                                var _id=data2[j]["esG_Id"];
                                $.ajax({
                                        url: "api/Report/GetData",
                                        type: "POST",
                                        async: false,
                                        contentType: "application/json",
                                        data: str2,
                                        success:function(data3){
                                            for(var k=0;k<12;k++)
                                            {
                                                if(data3[k]['tData']!=-1)
                                                {
                                                    $("[name='" + _id + "'][id='" + (k+1) + "']").val(data3[k]['tData']);
                                                }
                                            }
                                        }
                                    });
                                $("[name='" + data2[j]["esG_Id"] + "']").each(function(index,E){
                                    $(this).click(function(){
    
                                        if($(this).val()!="")
                                        {
                                            $(".modal-title").html("更改数据");
                                            $("[name='Data']").html("确认更改");
                                            $("[name='Delete']").show();
                                            $(".modal-body").html("<p>原数据"+$(this).val()+"</p>\
                                             更改为:<input type=\"text\" class=\"form-control\">");
                                            //更改数据，发送请求
                                             $("[name='Data']").off("click").click(function(){
                                                $.ajax({
                                                    url: "api/DataInputUser/UpdataData",
                                                    type: "POST",
                                                    contentType: "application/json",
                                                    data: JSON.stringify({
                                                        EsgId: $(E).attr("name"),
                                                        ReportId: rid,
                                                        ReportYear: ryear,
                                                        ReportMonth:$(E).attr("id"),
                                                        Data:$(".modal-body").children(".form-control").val(),
                                                        type:_Type
                                                    }),
                                                    success:function(data3){
                                                        $("[name='Data']").off('click');
                                                        if(data3==1){          
                                                            alert("更改成功！");
                                                            $('.modal').modal('hide');
                                                            $("[name='" + _id + "'][id='" + $(E).attr("id") + "']").val($(".modal-body").children(".form-control").val());
                                                        }
                                                    }
                                                });
                                            });
                                            $("[name='Delete']").off("click").click(function(){
                                                $.ajax({
                                                    url: "api/DataInputUser/DeleteData",
                                                    type: "POST",
                                                    contentType: "application/json",
                                                    data: JSON.stringify({
                                                        EsgId: $(E).attr("name"),
                                                        ReportId: rid,
                                                        ReportYear: ryear,
                                                        ReportMonth:$(E).attr("id"),
                                                        type:_Type
                                                    }),
                                                    success:function(data3){
                                                        $("[name='Delete']").off('click');
                                                        if(data3==1){          
                                                            alert("删除成功！");
                                                            $('.modal').modal('hide');
                                                            $("[name='" + _id + "'][id='" + $(E).attr("id") + "']").val("");
                                                        }
                                                    }
                                                });
                                            });
                                            $(".modal").modal();
                                        }
                                        else
                                        {
                                            $(".modal-title").html("录入数据");
                                            $("[name='Data']").html("确认录入");
                                            $("[name='Delete']").hide();
                                            $(".modal-body").html("录入新数据:<input type=\"text\" class=\"form-control\">");
                                            $(".modal").modal();
                                            $("[name='Data']").off("click").click(function(){
                                                $.ajax({
                                                    url: "api/DataInputUser/InputData",
                                                    type: "POST",
                                                    contentType: "application/json",
                                                    data: JSON.stringify({
                                                        EsgId: $(E).attr("name"),
                                                        ReportId: rid,
                                                        ReportYear: ryear,
                                                        ReportMonth:$(E).attr("id"),
                                                        Data:$(".modal-body").children(".form-control").val(),
                                                        type:_Type
                                                    }),
                                                    success:function(data3){
                                                        $("[name='Data']").off('click');
                                                        if(data3==1){
                                                            alert("录入成功！");
                                                            $('.modal').modal('hide');
                                                            $("[name='" + _id + "'][id='" + $(E).attr("id") + "']").val($(".modal-body").children(".form-control").val());
                                                        }
                                                    }
                                                });
                                            });
                                        }
                                    });
                                });
                               }
                               else if(data2[j]["frequency"]==0)//按年
                                  {
                                   $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                   15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+"(单位:\
                                   "+data2[j]["unit"]+")"+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\
                                   <span class=\"badge badge-pill badge-primary\">定量指标-按年填报</span></h4>\
                                   <table class=\"table table-striped table-bordered first\"> \
                                   <thead id=\"head\">\
                                   <tr>\
                                   <th style=\"font-size:20px\">"+ryear+"年"+"</th><th><input type=\"text\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></th>\
                                   </table>");
                                   var jsonArr = [ {
                                       "ESG_Id" : data2[j]["esG_Id"],
                                       "Type" : data2[j]["type"],
                                   }]
                                   var str2 = JSON.stringify({
                                       ReportId: rid,
                                       Report_Year: ryear,
                                       dataDetails: jsonArr
                                   });
                                   var _Type=data2[j]["type"];
                                   var _id=data2[j]["esG_Id"];
                                   $.ajax({
                                           url: "api/Report/GetData",
                                           type: "POST",
                                           async: false,
                                           contentType: "application/json",
                                           data: str2,
                                           success:function(data3){
                                                if(data3[0]['tData']!=-1)
                                                {
                                                       $("[name='" + _id + "']").val(data3[0]['tData']);
                                                }
                                           }
                                       });
                                   $("[name='" + data2[j]["esG_Id"] + "']").click(function(e){
                                           if($(this).val()!="")
                                           {
                                               $(".modal-title").html("更改数据");
                                               $("[name='Data']").html("确认更改");
                                               $("[name='Delete']").show();
                                               $(".modal-body").html("<p>原数据"+$(this).val()+"</p>\
                                                更改为:<input type=\"text\" class=\"form-control\">");
                                               //更改数据，发送请求
                                                $("[name='Data']").off("click").click(function(){
                                                   $.ajax({
                                                       url: "api/DataInputUser/UpdataData",
                                                       type: "POST",
                                                       contentType: "application/json",
                                                       data: JSON.stringify({
                                                           EsgId: e.currentTarget.name,
                                                           ReportId: rid,
                                                           ReportYear: ryear,
                                                           Data:$(".modal-body").children(".form-control").val(),
                                                           type:_Type
                                                       }),
                                                       success:function(data3){
                                                           $("[name='Data']").off('click');
                                                           if(data3==1){          
                                                               alert("更改成功！");
                                                               $('.modal').modal('hide');
                                                               $("[name='" + _id + "']").val($(".modal-body").children(".form-control").val());
                                                           }
                                                       }
                                                   });
                                               });
                                               $("[name='Delete']").off("click").click(function(){
                                                   $.ajax({
                                                       url: "api/DataInputUser/DeleteData",
                                                       type: "POST",
                                                       contentType: "application/json",
                                                       data: JSON.stringify({
                                                           EsgId: e.currentTarget.name,
                                                           ReportId: rid,
                                                           ReportYear: ryear,
                                                           type:_Type
                                                       }),
                                                       success:function(data3){
                                                           $("[name='Delete']").off('click');
                                                           if(data3==1){          
                                                               alert("删除成功！");
                                                               $('.modal').modal('hide');
                                                               $("[name='" + _id + "']").val("");
                                                           }
                                                       }
                                                   });
                                               });
                                               $(".modal").modal();
                                           }
                                           else
                                           {
                                               $(".modal-title").html("录入数据");
                                               $("[name='Data']").html("确认录入");
                                               $("[name='Delete']").hide();
                                               $(".modal-body").html("录入新数据:<input type=\"text\" class=\"form-control\">");
                                               $(".modal").modal();
                                               $("[name='Data']").off("click").click(function(){
                                                   $.ajax({
                                                       url: "api/DataInputUser/InputData",
                                                       type: "POST",
                                                       contentType: "application/json",
                                                       data: JSON.stringify({
                                                           EsgId: e.currentTarget.name,
                                                           ReportId: rid,
                                                           ReportYear: ryear,
                                                           Data:$(".modal-body").children(".form-control").val(),
                                                           type:_Type
                                                       }),
                                                       success:function(data3){
                                                           $("[name='Data']").off('click');
                                                           if(data3==1){
                                                               alert("录入成功！");
                                                               $('.modal').modal('hide');
                                                               $("[name='" + _id + "']").val($(".modal-body").children(".form-control").val());
                                                           }
                                                       }
                                                   });
                                               });
                                           }
                                   });
                                  }
                            }
                            else if(data2[j]["type"]==2)//定性指标
                            {
                                $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+
                                "&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"badge badge-pill badge-dark\">定性指标</span></h4>\
                                <table class=\"table table-striped table-bordered first\"> \
                                <th><input type=\"text\" name=\""+data2[j]["esG_Id"]+"\" class=\"form-control\"></th>\
                                </table>");
                                var jsonArr = [ {
                                    "ESG_Id" : data2[j]["esG_Id"],
                                    "Type" : data2[j]["type"],
                                }]
                                var str2 = JSON.stringify({
                                    ReportId: rid,
                                    Report_Year: ryear,
                                    dataDetails: jsonArr
                                });
                                var _Type=data2[j]["type"];
                                var _id=data2[j]["esG_Id"];
                                $.ajax({
                                        url: "api/Report/GetData",
                                        type: "POST",
                                        async: false,
                                        contentType: "application/json",
                                        data: str2,
                                        success:function(data3){
                                             if(data3[0]['tData']!=-1)
                                             {
                                                    $("[name='" + _id + "']").val(data3[0]['tData']);
                                             }
                                        }
                                    });
                                $("[name='" + data2[j]["esG_Id"] + "']").click(function(e){
                                        if($(this).val()!="")
                                        {
                                            
                                            $(".modal-title").html("更改数据");
                                            $("[name='Data']").html("确认更改");
                                            $("[name='Delete']").show();
                                            $(".modal-body").html("<p>原数据"+$(this).val()+"</p>\
                                             更改为:<input type=\"text\" class=\"form-control\">");
                                            //更改数据，发送请求
                                             $("[name='Data']").off("click").click(function(){
                                                $.ajax({
                                                    url: "api/DataInputUser/UpdataData",
                                                    type: "POST",
                                                    contentType: "application/json",
                                                    data: JSON.stringify({
                                                        EsgId: e.currentTarget.name,
                                                        ReportId: rid,
                                                        ReportYear: ryear,
                                                        Data:$(".modal-body").children(".form-control").val(),
                                                        type:_Type
                                                    }),
                                                    success:function(data3){
                                                        if(data3==1){          
                                                            alert("更改成功！");
                                                            $('.modal').modal('hide');
                                                            $("[name='" + _id + "']").val($(".modal-body").children(".form-control").val());
                                                        }
                                                    }
                                                });
                                            });
                                            $("[name='Delete']").off("click").click(function(){
                                                $.ajax({
                                                    url: "api/DataInputUser/DeleteData",
                                                    type: "POST",
                                                    contentType: "application/json",
                                                    data: JSON.stringify({
                                                        EsgId: e.currentTarget.name,
                                                        ReportId: rid,
                                                        ReportYear: ryear,
                                                        type:_Type
                                                    }),
                                                    success:function(data3){
                                                        if(data3==1){          
                                                            alert("删除成功！");
                                                            $('.modal').modal('hide');
                                                            $("[name='" + _id + "']").val("");
                                                        }
                                                    }
                                                });
                                            });
                                            $(".modal").modal();
                                        }
                                        else
                                        {
                                            $(".modal-title").html("录入数据");
                                            $("[name='Data']").html("确认录入");
                                            $("[name='Delete']").hide();
                                            $(".modal-body").html("录入新数据:<input type=\"text\" class=\"form-control\">");
                                            $(".modal").modal();
                                            $("[name='Data']").off("click").click(function(){
                                                $.ajax({
                                                    url: "api/DataInputUser/InputData",
                                                    type: "POST",
                                                    contentType: "application/json",
                                                    data: JSON.stringify({
                                                        EsgId: e.currentTarget.name,
                                                        ReportId: rid,
                                                        ReportYear: ryear,
                                                        Data:$(".modal-body").children(".form-control").val(),
                                                        type:_Type
                                                    }),
                                                    success:function(data3){
                                                        if(data3==1){
                                                            alert("录入成功！");
                                                            $('.modal').modal('hide');
                                                            $("[name='" + _id + "']").val($(".modal-body").children(".form-control").val());
                                                        }
                                                    }
                                                });
                                            });
                                        }
                                });
                            }
                            else if(data2[j]["type"]==11||data2[j]["type"]==12)//派生指标不可写
                            {
                                if(data2[j]["frequency"]==0){//按年
                                $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+"(单位:\
                                "+data2[j]["unit"]+")"+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                                +"<span class=\"badge badge-pill badge-warning\">派生指标-按年填报</span></h4>\
                                <table class=\"table table-striped table-bordered first\"> \
                                <thead id=\"head\">\
                                <tr>\
                                <th name=\""+data2[j]["esG_Id"]+"\"></th>\
                                </tr>\
                                </thead>\
                                </table>");
                                var jsonArr = [ {
                                    "ESG_Id" : data2[j]["esG_Id"],
                                    "Type" : data2[j]["type"],
                                }]
                                var str2 = JSON.stringify({
                                    ReportId: rid,
                                    Report_Year: ryear,
                                    dataDetails: jsonArr
                                });
                                var _Type=data2[j]["type"];
                                var _id=data2[j]["esG_Id"];
                                $.ajax({
                                        url: "api/Report/GetData",
                                        type: "POST",
                                        async: false,
                                        contentType: "application/json",
                                        data: str2,
                                        success:function(data3){
                                             if(data3[0]['tData']!=-1)
                                             {
                                                    $("[name='" + _id + "']").val(data3[0]['tData']);
                                             }
                                        }
                                    });
                                }
                                else if(data2[j]["frequency"]==1){//按月
                                    $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                    15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+"(单位:\
                                    "+data2[j]["unit"]+")"+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                                    +"<span class=\"badge badge-pill badge-warning\">派生指标-按月填报</span></h4>\
                                    <table class=\"table table-striped table-bordered first\"> \
                                    <thead id=\"head\">\
                                    <tr>\
                                    <th>1月</th><th>2月</th><th>3月</th>\
                                    <th>4月</th><th>5月</th><th>6月</th>\
                                    </tr>\
                                    </thead>\
                                    <tbody id=\"body\">\
                                    <tr>\
                                    <td id=\"1\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"2\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"3\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"4\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"5\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"6\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    </tr>\
                                    </tbody>\
                                    <thead id=\"head\">\
                                    <tr>\
                                    <th>7月</th><th>8月</th><th>9月</th>\
                                    <th>10月</th><th>11月</th><th>12月</th>\
                                    </tr>\
                                    </thead>\
                                    <tbody id=\"body\">\
                                    <tr>\
                                    <td id=\"7\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"8\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"9\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"10\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"11\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"12\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    </tr>\
                                    </tbody>\
                                    </table>");
                                    var jsonArr = [ {
                                        "ESG_Id" : data2[j]["esG_Id"],
                                        "Type" : data2[j]["type"],
                                    }]
                                    var str2 = JSON.stringify({
                                        ReportId: rid,
                                        Report_Year: ryear,
                                        dataDetails: jsonArr
                                    });
                                    var _Type=data2[j]["type"];
                                    var _id=data2[j]["esG_Id"];
                                    $.ajax({
                                            url: "api/Report/GetData",
                                            type: "POST",
                                            async: false,
                                            contentType: "application/json",
                                            data: str2,
                                            success:function(data3){
                                                for(var k=0;k<data3.length;k++)
                                                {
                                                    if(data3[k]['tData']!=-1)
                                                    {
                                                        $("[name='" + _id + "'][id='" + data3[k]['report_Month'] + "']").val(data3[k]['tData']);
                                                    }
                                                }
                                            }
                                        });
    
                                }
                            }
                        }
                        
    
    
                    });
                   }
                }
                else//报表权限：只读权限
                {
                    for(var j=0;j<data2.length;j++)
                    {
                     $("[name='Five']").each(function(index){
                        if(index==i){//选择对应的四级表格插入
                            if(data2[j]["type"]==1)//定量指标
                            {
                               if(data2[j]["frequency"]==1)//按月
                               {
                                $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+"(单位:\
                                "+data2[j]["unit"]+")"+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\
                                <span class=\"badge badge-pill badge-primary\">定量指标-按月填报</span>"+"</h4>\
                                <table class=\"table table-striped table-bordered first\"> \
                                <thead id=\"head\">\
                                <tr>\
                                <th>1月</th><th>2月</th><th>3月</th>\
                                <th>4月</th><th>5月</th><th>6月</th>\
                                </tr>\
                                </thead>\
                                <tbody id=\"body\">\
                                <tr>\
                                <td id=\"1\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"2\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"3\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"4\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"5\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"6\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                </tr>\
                                </tbody>\
                                <thead id=\"head\">\
                                <tr>\
                                <th>7月</th><th>8月</th><th>9月</th>\
                                <th>10月</th><th>11月</th><th>12月</th>\
                                </tr>\
                                </thead>\
                                <tbody id=\"body\">\
                                <tr>\
                                <td id=\"7\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"8\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"9\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"10\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"11\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                <td id=\"12\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                </tr>\
                                </tbody>\
                                </table>");
                                var jsonArr = [ {
                                    "ESG_Id" : data2[j]["esG_Id"],
                                    "Type" : data2[j]["type"],
                                }]
                                var str2 = JSON.stringify({
                                    ReportId: rid,
                                    Report_Year: ryear,
                                    dataDetails: jsonArr
                                });
                                var _Type=data2[j]["type"];
                                var _id=data2[j]["esG_Id"];
                                $.ajax({
                                        url: "api/Report/GetData",
                                        type: "POST",
                                        async: false,
                                        contentType: "application/json",
                                        data: str2,
                                        success:function(data3){
                                            for(var k=0;k<12;k++)
                                            {
                                                if(data3[k]['tData']!=-1)
                                                {
                                                    $("[name='" + _id + "'][id='" + (k+1) + "']").html(data3[k]['tData']);
                                                }
                                            }
                                        }
                                    });
                               }
                               else if(data2[j]["frequency"]==0)//按年
                                  {
                                   $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                   15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+"(单位:\
                                   "+data2[j]["unit"]+")"+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\
                                   <span class=\"badge badge-pill badge-primary\">定量指标-按年填报</span></h4>\
                                   <table class=\"table table-striped table-bordered first\"> \
                                   <thead id=\"head\">\
                                   <tr>\
                                   <th style=\"font-size:20px\">"+ryear+"年"+"</th><th name=\""+data2[j]["esG_Id"]+"\"></th>\
                                   </table>");
                                   var jsonArr = [ {
                                       "ESG_Id" : data2[j]["esG_Id"],
                                       "Type" : data2[j]["type"],
                                   }]
                                   var str2 = JSON.stringify({
                                       ReportId: rid,
                                       Report_Year: ryear,
                                       dataDetails: jsonArr
                                   });
                                   var _Type=data2[j]["type"];
                                   var _id=data2[j]["esG_Id"];
                                   $.ajax({
                                           url: "api/Report/GetData",
                                           type: "POST",
                                           async: false,
                                           contentType: "application/json",
                                           data: str2,
                                           success:function(data3){
                                                if(data3[0]['tData']!=-1)
                                                {
                                                       $("[name='" + _id + "']").html(data3[0]['tData']);
                                                }
                                           }
                                       });
                                  }
                            }
                            else if(data2[j]["type"]==2)//定性指标
                            {
                                $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+
                                "&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"badge badge-pill badge-dark\">定性指标</span></h4>\
                                <table class=\"table table-striped table-bordered first\"> \
                                <th name=\""+data2[j]["esG_Id"]+"\"></th>\
                                </table>");
                                var jsonArr = [ {
                                    "ESG_Id" : data2[j]["esG_Id"],
                                    "Type" : data2[j]["type"],
                                }]
                                var str2 = JSON.stringify({
                                    ReportId: rid,
                                    Report_Year: ryear,
                                    dataDetails: jsonArr
                                });
                                var _Type=data2[j]["type"];
                                var _id=data2[j]["esG_Id"];
                                $.ajax({
                                        url: "api/Report/GetData",
                                        type: "POST",
                                        async: false,
                                        contentType: "application/json",
                                        data: str2,
                                        success:function(data3){
                                             if(data3[0]['tData']!=-1)
                                             {
                                                    $("[name='" + _id + "']").html(data3[0]['tData']);
                                             }
                                        }
                                    });
                            }
                            else if(data2[j]["type"]==11||data2[j]["type"]==12)//派生指标不可写
                            {
                                if(data2[j]["frequency"]==0){//按年
                                $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+"(单位:\
                                "+data2[j]["unit"]+")"+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                                +"<span class=\"badge badge-pill badge-warning\">派生指标-按年填报</span></h4>\
                                <table class=\"table table-striped table-bordered first\"> \
                                <thead id=\"head\">\
                                <tr>\
                                <th name=\""+data2[j]["esG_Id"]+"\"></th>\
                                </tr>\
                                </thead>\
                                </table>");
                                var jsonArr = [ {
                                    "ESG_Id" : data2[j]["esG_Id"],
                                    "Type" : data2[j]["type"],
                                }]
                                var str2 = JSON.stringify({
                                    ReportId: rid,
                                    Report_Year: ryear,
                                    dataDetails: jsonArr
                                });
                                var _Type=data2[j]["type"];
                                var _id=data2[j]["esG_Id"];
                                $.ajax({
                                        url: "api/Report/GetData",
                                        type: "POST",
                                        async: false,
                                        contentType: "application/json",
                                        data: str2,
                                        success:function(data3){
                                             if(data3[0]['tData']!=-1)
                                             {
                                                    $("[name='" + _id + "']").html(data3[0]['tData']);
                                             }
                                        }
                                    });
                                }
                                else if(data2[j]["frequency"]==1){//按月
                                    $(this).append("<h4 class=\"pageheader-title\" style=\"font-size:\
                                    15px\">"+data2[j]["esG_Id"]+"&nbsp;&nbsp;&nbsp;"+data2[j]["name"]+"(单位:\
                                    "+data2[j]["unit"]+")"+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                                    +"<span class=\"badge badge-pill badge-warning\">派生指标-按月填报</span></h4>\
                                    <table class=\"table table-striped table-bordered first\"> \
                                    <thead id=\"head\">\
                                    <tr>\
                                    <th>1月</th><th>2月</th><th>3月</th>\
                                    <th>4月</th><th>5月</th><th>6月</th>\
                                    </tr>\
                                    </thead>\
                                    <tbody id=\"body\">\
                                    <tr>\
                                    <td id=\"1\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"2\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"3\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"4\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"5\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"6\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    </tr>\
                                    </tbody>\
                                    <thead id=\"head\">\
                                    <tr>\
                                    <th>7月</th><th>8月</th><th>9月</th>\
                                    <th>10月</th><th>11月</th><th>12月</th>\
                                    </tr>\
                                    </thead>\
                                    <tbody id=\"body\">\
                                    <tr>\
                                    <td id=\"7\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"8\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"9\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"10\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"11\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    <td id=\"12\" name=\""+data2[j]["esG_Id"]+"\"></td>\
                                    </tr>\
                                    </tbody>\
                                    </table>");
                                    var jsonArr = [ {
                                        "ESG_Id" : data2[j]["esG_Id"],
                                        "Type" : data2[j]["type"],
                                    }]
                                    var str2 = JSON.stringify({
                                        ReportId: rid,
                                        Report_Year: ryear,
                                        dataDetails: jsonArr
                                    });
                                    var _Type=data2[j]["type"];
                                    var _id=data2[j]["esG_Id"];
                                    $.ajax({
                                            url: "api/Report/GetData",
                                            type: "POST",
                                            async: false,
                                            contentType: "application/json",
                                            data: str2,
                                            success:function(data3){
                                                for(var k=0;k<data3.length;k++)
                                                {
                                                    if(data3[k]['tData']!=-1)
                                                    {
                                                        $("[name='" + _id + "'][id='" + data3[k]['report_Month'] + "']").html(data3[k]['tData']);
                                                    }
                                                }
                                            }
                                        });
    
                                }
                            }
                        }
                    });
                   }
                }
                },
                error:function(xhr){alert("error");}
                 });
            }
        },
        error:function(xhr){alert("error");}
         });

        result.splice(0, result.length);
    };

    ComboTree.prototype.uncheckrow = function (oliId) {
        let _this = this, icon = null;
        let id = null;

        for (let i = 0; i < _this.oliIdArray.length; i++) {
            if (_this.oliIdArray[i] === oliId) { //表示数组里面有这个元素
                id = i; //元素位置
                _this.oliIdArray.splice(i, 1);
                //把当前点击的oliId赋值到显示的input的oliId里面
                _this.roleSelect.attr("data-id", _this.oliIdArray);
                // console.log('删除当前的序号' + oliId + ';' + '剩下数组' + _this.oliIdArray)
            }
        }

        $(_this.element).find('.title-container').each(function (index, item) {
            if ($(item).attr('data-id') === oliId) {
                $(item).removeClass('actived_li');
                let $checkbox = $(item).find('input');
                $checkbox.prop('checked', false);
            }
        });

        if (!icon) {
            icon = $(_this.element).find('.input-keyword-wrap>i');
            icon.removeClass("fa-caret-up").addClass("fa-caret-down"); //点击input选择适合，小图标动态切换
        }

        _this.roleSelect.find('>span').each(function (index, item) {
            if ($(item).attr('data-id') === oliId) {
                item.remove();
            }
        });

        if (_this.oliIdArray.length === 0) {
            _this.myplaceholder.show();
        }
    };

    ComboTree.prototype.hideul = function () {
        // event=event||window.event;
        $(this.element).find('.input-keyword-wrap .fa').removeClass("fa-caret-up").addClass("fa-caret-down"); //当点隐藏ul弹窗时候，把小图标恢复原状
        $(this.element).find('.drop-down-wrap').hide(); //当点击空白处，隐藏ul弹窗
    };

    /**
     * 清空搜索输入框里面的内容
     */
    ComboTree.prototype.clearSearchValue = function() {
        console.log(3333);
    };

    ComboTree.prototype.datas = function () {

        let arr = [];
        $(this.element).find('.input-keyword-item').each(function (index, item) {
            arr.push({
                id:$(item).attr('data-id'),
                val:$(item).find('span').html()
            });
        });
        return arr;
    };

    function appendcheckbox(divitem, oliId) {
        let $checkboxspan = $('<span class="handle-checkbox"></span>'),
            $checkbox = $('<input data-id="' + oliId + '" type="checkbox">');
        $checkboxspan.append($checkbox);
        divitem.append($checkboxspan);
    }

    $.fn[comboTreePlugin] = function (options) {
        var ctArr = [];
        this.each(function () {
            if (!$.data(this, 'plugin_' + comboTreePlugin)) {
                $.data(this, 'plugin_' + comboTreePlugin, new ComboTree(this, options));
                ctArr.push($(this).data()['plugin_' + comboTreePlugin]);
            }
        });

        if (this.length === 1)
            return ctArr[0];
        else
            return ctArr;
    };

})(jQuery, window, document);
