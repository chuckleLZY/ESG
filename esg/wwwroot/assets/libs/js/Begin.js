$(document).ready(function(){
    var s = location.search.split('?')[1].split('&');
    var id = s[0].split('=')[1];
    var level = s[1].split('=')[1];
    var cname = s[2].split('=')[1];
    var username = s[3].split('=')[1];
    $("#id").html(username+"(ID:"+id+")");
    $("#cname").html(decodeURI(decodeURI(cname)) +"公司");
    if(level==1)
    {
        $("#level").html("您的身份：一级管理员");
    }
    else if (level==2)
    {
        $("#level").html("您的身份：二级管理员");
    }
    else if(level==3)
    {
        $("#level").html("您的身份：数据录入员");
    }
    $("#createcompany").attr("href","CreateCompany.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    $("#managecompany").attr("href","ManageCompany.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    $("#createperson").attr("href","CreateCustomer.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    $("#manageperson").attr("href","ManageUser.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    $("#createreport").attr("href","CreateReport.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    $("#viewreport").attr("href","ViewReport.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    $("#createreport").attr("href","CreateReport.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    $("#viewreport").attr("href","ViewReport.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    $("#datanotcheck").attr("href","ViewNotCheck.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    $("#datahascheck").attr("href","ViewHasCheck.html?"+'id='+id+'&level='+level+'&cname='+cname+'&username='+username);
    //$("#datanotcheck").attr("href","ViewNotCheck.html?"+'id='+id+'&level='+level+'&cname='+encodeURI(encodeURI(cname))+'&username='+username);
    //$("#datahascheck").attr("href","ViewHasCheck.html?"+'id='+id+'&level='+level+'&cname='+encodeURI(encodeURI(cname))+'&username='+username);
    //$("#datanotcheck").attr("href","ViewNotCheck.html?"+'id='+id+'&level='+level+'&cname='+encodeURI(encodeURI(cname))+'&username='+username);
    //$("#datahascheck").attr("href","ViewHasCheck.html?"+'id='+id+'&level='+level+'&cname='+encodeURI(encodeURI(cname))+'&username='+username);
    });