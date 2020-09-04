$(document).ready(function(){
    var s = location.search.split('?')[1].split('&');
    var id = s[0].split('=')[1];
    var level = s[1].split('=')[1];
    var cname = s[2].split('=')[1];
    var username = s[3].split('=')[1];
    var cid=s[4].split('=')[1];
    $("#id").html(username+"(ID:"+id+")");
    $("#cname").html(decodeURI(decodeURI(cname)) +"公司");
    if(level==1)
    {
        $("#level").html("您的身份：一级管理员");
        $("#luru").remove();

    }
    else if (level==2)
    {
        $("#level").html("您的身份：二级管理员");
        $("#luru").remove();
        $("#createcompany").remove();
        $("#managecompany").remove();
    }
    else if(level==3)
    {
        $("#level").html("您的身份：数据录入员");
        $("#xinjian").remove();
        $("#shenhe").remove();
    }
    else if(level==0)
    {
        $("#level").html("您的身份：管理员");
        $("#shenhe").remove();
        $("#luru").remove();
    }
    $("#createcompany").attr("href","CreateCompany.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#managecompany").attr("href","ManageCompany.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#createperson").attr("href","CreateCustomer.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#manageperson").attr("href","ManageUser.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#createreport").attr("href","CreateReport.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#viewreport_luru").attr("href","ViewReport.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#createreport").attr("href","CreateReport.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#viewreport_shenhe").attr("href","ViewReport_2.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#datanotcheck").attr("href","ViewNotCheck.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#datahascheck").attr("href","ViewHasCheck.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#shouye").attr("href","backend_homepage.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    $("#personalinfo").attr("href","PersonalInfo.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username+"&cid="+cid);
    });