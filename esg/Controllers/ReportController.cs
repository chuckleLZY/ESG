using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using esg.Models;

namespace esg.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        string connString = "server=rm-bp1o13lfefc6t7z98io.mysql.rds.aliyuncs.com;database=esg_information_database;uid=super_esg;pwd=esg123456";

        //新建报表
        [HttpPost]
        public int AddReport(Report rep)
        {
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                //插入数据库失败后会报错崩溃，所以用代码判断边界条件


                /*//判断是否存在员工ID
                string sql = "select * from user where user_id='" + rep.UserId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();*/

                //判断是否存在公司ID
                string sql = "select * from company where com_id='" + rep.CompanyId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                /*//判断员工是否是管理员、员工与公司是否对应
                sql = "select com_id,level from user where user_id='" + rep.UserId + "'";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                while (reader.Read())//按行执行查询，每次循环查询一行
                {
                    //此时reader会获取一行的内容
                    int com_id = reader.GetInt32("com_id");
                    int level = reader.GetInt32("level");
                    if (com_id != rep.CompanyId || level != 1)//level为1表示管理员
                    {
                        reader.Close();
                        return 0;
                    }
                }
                reader.Close();

                //判断公司与等级是否对应
                sql = "select * from company where com_id='" + rep.CompanyId + "' and level='" + rep.Level + "'";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();*/

                //插入数据
                sql = "insert into report(com_id,report_name,status,report_year) values (@com_id,@report_name,@status,@report_year)";
                cmd.CommandText = sql;//更新一下sql语句
                cmd.Parameters.Add(new MySqlParameter("@com_id", rep.CompanyId));
                cmd.Parameters.Add(new MySqlParameter("@report_name", rep.ReportName));
                cmd.Parameters.Add(new MySqlParameter("@status", rep.Status));
                cmd.Parameters.Add(new MySqlParameter("@report_year", rep.Report_Year));
                int err=cmd.ExecuteNonQuery();
                //int report_id = (int)cmd.LastInsertedId;

                con.Close();
                if (err > 0)
                    return 1;
                else
                    return 0;
            }
        }

        //删除报表
        [HttpDelete]
        //   api/Report/Delete
        public int DeleteReport(Report rep)
        {
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                //插入数据库失败后会报错崩溃，所以用代码判断边界条件

                /*//判断是否存在员工ID
                string sql = "select * from user where user_id='" + rep.UserId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();*/

                //判断是否存在报表ID
                string sql = "select * from report where report_id='" + rep.ReportId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                /*//判断员工是否是管理员
                sql = "select level from user where user_id='" + rep.UserId + "' and level=1";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                //判断员工公司与报表公司是否一致
                sql = "select* from user join report using(com_id) where user_id='" + rep.UserId + "' and report_id='" + rep.ReportId + "'";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();*/

                //删除数据
                sql = "delete from data_qualitative where report_id='" + rep.ReportId + "'";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                sql = "delete from data_quantitative where report_id='" + rep.ReportId + "'";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                //删除报表
                sql = "delete from report where report_id='" + rep.ReportId + "'";
                cmd.CommandText = sql;
                int err=cmd.ExecuteNonQuery();

                con.Close();
                if (err > 0)
                    return 1;
                else
                    return 0;
            }
        }

        //审核通过
        [HttpPost]
        //   api/Report/ReportPass
        public int ReportPass(Report rep)
        {
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                // 判断是否存在报表ID
                string sql = "select * from report where report_id='" + rep.ReportId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return -1;
                }
                reader.Close();
                if(rep.Status==10)
                {
                    rep.Status = 11;
                }
                else if(rep.Status==20)
                {
                    rep.Status = 21;
                }
                sql = "update report set status='" + rep.Status + "'where report_id='" + rep.ReportId + "'";
                cmd = new MySqlCommand(sql, con);
                int err=cmd.ExecuteNonQuery();

                con.Close();
                if (err > 0)
                    return rep.Status;
                else
                    return -1;
            }
        }

        //审核退回
        [HttpPost]
        //   api/Report/Return
        public int ReportReturn(Report rep)
        {
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                // 判断是否存在报表ID
                string sql = "select * from report where report_id='" + rep.ReportId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();
                if (rep.Status == 10)
                {
                    rep.Status = 1;
                }
                else if (rep.Status == 20)
                {
                    rep.Status = 22;
                }
                sql = "update report set status='" + rep.Status + "'where report_id='" + rep.ReportId + "'";
                cmd = new MySqlCommand(sql, con);
                int err = cmd.ExecuteNonQuery();

                con.Close();
                if (err > 0)
                    return rep.Status;
                else
                    return 0;
            }
        }

        //提交审核数据
        [HttpPost]
        //   api/Report/HandIn
        public int ReportHandIn(Report rep)
        {
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                // 判断是否存在报表ID
                string sql = "select * from report where report_id='" + rep.ReportId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();
                
                sql = "update report set status= 20 where report_id='" + rep.ReportId + "'";
                cmd = new MySqlCommand(sql, con);
                int err = cmd.ExecuteNonQuery();

                con.Close();
                if (err > 0)
                    return 20;
                else
                    return 0;
            }
        }

        //查看四级指标
        [HttpPost]
        //   api/Report/GetIndicate4
        public List<string> GetIndicate4(Indicate ind)
        {
            List<string> indicatename4 = new List<string>();
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                /*// 判断是否存在员工ID
                string sql = "select * from user where user_id='" + ind.UserId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return indicatename4;
                }
                reader.Close();*/

                string sql = "select name from indicate where parent=(select id from indicate where name='" + ind.IndicateName3 + "' and parent=(select id from indicate where name='" + ind.IndicateName2 + "' and parent=(select id from indicate where name='" + ind.IndicateName1 + "')))";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                string a;
                while (reader.Read())
                {
                    a = reader.GetString("name");
                    indicatename4.Add(a);
                }
                reader.Close();

                con.Close();
            }
            return indicatename4;
        }

        //查看五级指标
        [HttpPost]
        //   api/Report/GetIndicate5
        public List<Indicate5> GetIndicate5(Indicate ind)
        {
            List<Indicate5> indicate5 = new List<Indicate5>();
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                /*// 判断是否存在员工ID
                string sql = "select * from user where user_id='" + ind.UserId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return indicatename4;
                }
                reader.Close();*/

                string sql = "select name,esg_id,unit,type,frequency from indicate where parent=(select id from indicate where name='" + ind.IndicateName4 + "' and parent=(select id from indicate where name='" + ind.IndicateName3 + "' and parent=(select id from indicate where name='" + ind.IndicateName2 + "' and parent=(select id from indicate where name='" + ind.IndicateName1 + "'))))";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                Indicate5 a = new Indicate5();
                while (reader.Read())
                {
                    a = new Indicate5();
                    a.Name = reader.GetString("name");
                    a.ESG_Id = reader.GetString("esg_id");
                    a.Unit = reader.GetString("unit");
                    a.Type = reader.GetInt32("type");
                    a.Frequency = reader.GetInt32("frequency");
                    indicate5.Add(a);
                }
                reader.Close();

                con.Close();
            }
            return indicate5;
        }

        //查看数据
        [HttpPost]
        //   api/Report/GetData
        public List<Data> GetData([FromBody]DataDetails dataDetail)
        {
            int Report_Year = dataDetail.Report_Year;
            int ReportId = dataDetail.ReportId;
            List<Data> datas = new List<Data>();
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                /*// 判断是否存在员工ID
                string sql = "select * from user where user_id='" + ind.UserId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return datas;
                }
                reader.Close();*/

                // 判断是否存在报表ID
                string sql = "select * from report where report_id='" + ReportId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return datas;
                }
                reader.Close();
                Data data = new Data();
                int frequency;
                for (int i=0;i< dataDetail.dataDetails.Count;i++)
                {
                    if(dataDetail.dataDetails[i].Type>10)
                    {
                        if(dataDetail.dataDetails[i].ESG_Id== "A1.2-10")
                        {
                            //A2.1 - 10 * A1.2 - 10
                            double A21_10, A12_10;
                            sql = "select ratio from greenhouse_gas_ratio where esg_id='A1.2-10' and year='" + Report_Year + "'";
                            cmd = new MySqlCommand(sql, con);
                            reader = cmd.ExecuteReader();
                            reader.Read();
                            if (!reader.HasRows)
                            {
                                reader.Close();
                                data = new Data();
                                data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                data.Report_Month = 0;
                                data.TData = -1;
                                datas.Add(data);
                                continue;
                            }
                            A12_10 = reader.GetDouble("ratio");
                            reader.Close();
                            for (int j=1;j<13;j++)
                            {
                                sql = "select data from data_quantitative where esg_id='A2.1-10' and report_id='" + ReportId + "' and report_year='" + Report_Year + "' and report_month='" + j + "'";
                                cmd = new MySqlCommand(sql, con);
                                reader = cmd.ExecuteReader();
                                if (!reader.HasRows)
                                {
                                    reader.Close();
                                    data = new Data();
                                    data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                    data.Report_Month = j;
                                    data.TData = -1;
                                    datas.Add(data);
                                    continue;
                                }
                                else
                                {
                                    reader.Read();
                                    A21_10 = reader.GetDouble("data");
                                    reader.Close();

                                    data = new Data();
                                    data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                    data.Report_Month = j;
                                    data.TData = A21_10 * A12_10;
                                    datas.Add(data);
                                }
                            }
                        }
                        else if(dataDetail.dataDetails[i].ESG_Id == "A1.3-55")
                        {
                            //A1.3-35
                            double A13_35;
                            for (int j = 1; j < 13; j++)
                            {
                                sql = "select data from data_quantitative where esg_id='A1.3-35' and report_id='" + ReportId + "' and report_year='" + Report_Year + "' and report_month='" + j + "'";
                                cmd = new MySqlCommand(sql, con);
                                reader = cmd.ExecuteReader();
                                reader.Read();
                                if (!reader.HasRows)
                                {
                                    reader.Close();
                                    data = new Data();
                                    data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                    data.Report_Month = j;
                                    data.TData = -1;
                                    datas.Add(data);
                                    continue;
                                }
                                A13_35 = reader.GetDouble("data");
                                reader.Close();

                                data = new Data();
                                data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                data.Report_Month = j;
                                data.TData = A13_35;
                                datas.Add(data);
                            }
                        }
                        else if (dataDetail.dataDetails[i].ESG_Id == "A1.3-56")
                        {
                            //A1.3-55/A0-4
                            double A13_55,A0_4;
                            for (int j = 1; j < 13; j++)
                            {
                                sql = "select data from data_quantitative where esg_id='A1.3-55' and report_id='" + ReportId + "' and report_year='" + Report_Year + "' and report_month='" + j + "'";
                                cmd = new MySqlCommand(sql, con);
                                reader = cmd.ExecuteReader();
                                reader.Read();
                                if (!reader.HasRows)
                                {
                                    reader.Close();
                                    data = new Data();
                                    data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                    data.Report_Month = j;
                                    data.TData = -1;
                                    datas.Add(data);
                                    continue;
                                }
                                A13_55 = reader.GetDouble("data");
                                reader.Close();

                                sql = "select data from data_quantitative where esg_id='A0-4' and report_id='" + ReportId + "' and report_year='" + Report_Year + "' and report_month='" + j + "'";
                                cmd = new MySqlCommand(sql, con);
                                reader = cmd.ExecuteReader();
                                reader.Read();
                                if (!reader.HasRows)
                                {
                                    reader.Close();
                                    data = new Data();
                                    data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                    data.Report_Month = j;
                                    data.TData = -1;
                                    datas.Add(data);
                                    continue;
                                }
                                A0_4 = reader.GetDouble("data");
                                reader.Close();

                                data = new Data();
                                data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                data.Report_Month = j;
                                data.TData = A13_55/A0_4;
                                datas.Add(data);
                            }
                        }

                        else if (dataDetail.dataDetails[i].ESG_Id == "A2.1-10")
                        {
                            //A2.1-10.1*能源系数A2.1-10
                            double A21_101, A21_10;
                            sql = "select ratio from energy_ratio where esg_id='A2.1-10' and year='" + Report_Year + "'";
                            cmd = new MySqlCommand(sql, con);
                            reader = cmd.ExecuteReader();
                            reader.Read();
                            if (!reader.HasRows)
                            {
                                reader.Close();
                                data = new Data();
                                data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                data.Report_Month = 0;
                                data.TData = -1;
                                datas.Add(data);
                                continue;
                            }
                            A21_10 = reader.GetDouble("ratio");
                            reader.Close();
                            for (int j = 1; j < 13; j++)
                            {
                                sql = "select data from data_quantitative where esg_id='A2.1-10.1' and report_id='" + ReportId + "' and report_year='" + Report_Year + "' and report_month='" + j + "'";
                                cmd = new MySqlCommand(sql, con);
                                reader = cmd.ExecuteReader();
                                reader.Read();
                                if (!reader.HasRows)
                                {
                                    reader.Close();
                                    data = new Data();
                                    data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                    data.Report_Month = j;
                                    data.TData = -1;
                                    datas.Add(data);
                                    continue;
                                }
                                A21_101 = reader.GetDouble("data");

                                data = new Data();
                                data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                data.Report_Month = j;
                                data.TData = A21_101 * A21_10;
                                datas.Add(data);
                                reader.Close();
                            }
                        }
                    }
                    else
                    {
                        if(dataDetail.dataDetails[i].Type == 2)//定性数据
                        {
                            sql = "select data from data_qualitative where esg_id='"+ dataDetail.dataDetails[i].ESG_Id+"' and report_id='" + ReportId + "' and report_year='" + Report_Year + "'";
                            cmd = new MySqlCommand(sql, con);
                            reader = cmd.ExecuteReader();
                            reader.Read();
                            if (!reader.HasRows)
                            {
                                reader.Close();
                                data = new Data();
                                data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                data.Report_Month = 0;
                                data.TData = -1;
                                datas.Add(data);
                                continue;
                            }
                            data = new Data();
                            data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                            data.TData = reader.GetString("data");
                            datas.Add(data);
                            reader.Close();
                        }
                        else if(dataDetail.dataDetails[i].Type == 1)//定量数据
                        {
                            sql = "select frequency from indicate where esg_id='" + dataDetail.dataDetails[i].ESG_Id + "'";
                            cmd = new MySqlCommand(sql, con);
                            reader = cmd.ExecuteReader();
                            reader.Read();
                            if (!reader.HasRows)
                            {
                                reader.Close();
                                data = new Data();
                                data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                data.Report_Month = 0;
                                data.TData = -1;
                                datas.Add(data);
                                continue;
                            }
                            frequency = reader.GetInt32("frequency");
                            reader.Close();
                            if (frequency==0)//频率为年
                            {
                                sql = "select data from data_quantitative where esg_id='" + dataDetail.dataDetails[i].ESG_Id + "' and report_id='" + ReportId + "' and report_year='" + Report_Year + "'";
                                cmd = new MySqlCommand(sql, con);
                                reader = cmd.ExecuteReader();
                                reader.Read();
                                if (!reader.HasRows)
                                {
                                    reader.Close();
                                    data = new Data();
                                    data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                    data.Report_Month = 0;
                                    data.TData = -1;
                                    datas.Add(data);
                                    continue;
                                }
                                data = new Data();
                                data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                data.TData = reader.GetDouble("data");
                                datas.Add(data);
                                reader.Close();
                            }
                            else//频率为月
                            {
                                for (int j = 1; j < 13; j++)
                                {
                                    sql = "select data from data_quantitative where esg_id='" + dataDetail.dataDetails[i].ESG_Id + "' and report_id='" + ReportId + "' and report_year='" + Report_Year + "' and report_month='" + j + "'";
                                    cmd = new MySqlCommand(sql, con);
                                    reader = cmd.ExecuteReader();
                                    reader.Read();
                                    if (!reader.HasRows)
                                    {
                                        reader.Close();
                                        data = new Data();
                                        data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                        data.Report_Month = j;
                                        data.TData = -1;
                                        datas.Add(data);
                                        continue;
                                    }
                                    data = new Data();
                                    data.ESG_Id = dataDetail.dataDetails[i].ESG_Id;
                                    data.Report_Month = j;
                                    data.TData = reader.GetDouble("data");
                                    datas.Add(data);
                                    reader.Close();
                                }
                            }
                        }
                    }
                }
                con.Close();
                return datas;
            }
        }
        
        //数据录入员提交审核
        [HttpPost]
        //   api/Report/DataSubmitReport
        public int DataSubmitReport([FromBody]Report rep)
        {
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                // 判断是否存在报表ID
                string sql = "select * from report where report_id=" + rep.ReportId;
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();
                sql = "update report set status=10 where report_id=" + rep.ReportId;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                con.Close();
            }
            return 1;
        }


        //查看本公司报表(针对录入员)
        [HttpPost]
        public List<Report> Data_CompanyReport([FromBody]int CompanyId)
        {
            List<Report> reports = new List<Report>();
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();
                string sql = "select report_id,report_name,status,report_year from report where com_id=" + CompanyId;
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Report report = new Report();
                    report.ReportId = reader.GetInt32("report_id");
                    report.ReportName = reader.GetString("report_name");
                    report.Status = reader.GetInt32("status");
                    report.Report_Year = reader.GetInt32("report_year");
                    reports.Add(report);
                }
                reader.Close();
            }
            return reports;
        }

        //查看本公司报表(针对管理员)
        [HttpPost]
        public List<Report> Admin_CompanyReport([FromBody]int CompanyId)
        {
            List<Report> reports = new List<Report>();
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                //找本公司的报表
                string sql = "select report_id,report_name,status,report_year from report where com_id=" + CompanyId;
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Report report = new Report();
                    report.ReportId = reader.GetInt32("report_id");
                    report.ReportName = reader.GetString("report_name");
                    report.Status = reader.GetInt32("status");
                    report.Report_Year = reader.GetInt32("report_year");
                    reports.Add(report);
                }
                reader.Close();

                //找子公司的报表
                sql = "select report_id,report_name,status,report_year from report natural join company " +
                    "where parent=" + CompanyId;
                cmd.CommandText = sql;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Report report = new Report();
                    report.ReportId = reader.GetInt32("report_id");
                    report.ReportName = reader.GetString("report_name");
                    report.Status = reader.GetInt32("status");
                    report.Report_Year = reader.GetInt32("report_year");
                    reports.Add(report);
                }
            }
            return reports;
        }
    }
}
