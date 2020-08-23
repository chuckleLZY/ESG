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


                //判断是否存在员工ID
                string sql = "select * from user where user_id='" + rep.UserId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                //判断是否存在公司ID
                sql = "select * from company where com_id='" + rep.CompanyId + "'";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                //判断员工是否是管理员、员工与公司是否对应
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
                reader.Close();

                //插入数据
                sql = "insert into report(com_id,report_name,status,level) values (@com_id,@report_name,@status,@lvl)";
                cmd.CommandText = sql;//更新一下sql语句
                cmd.Parameters.Add(new MySqlParameter("@com_id", rep.CompanyId));
                cmd.Parameters.Add(new MySqlParameter("@report_name", rep.ReportName));
                cmd.Parameters.Add(new MySqlParameter("@status", rep.Status));
                cmd.Parameters.Add(new MySqlParameter("@lvl", rep.Level));
                cmd.ExecuteNonQuery();
                int report_id = (int)cmd.LastInsertedId;

                con.Close();
                return report_id;
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

                //判断是否存在员工ID
                string sql = "select * from user where user_id='" + rep.UserId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                //判断是否存在报表ID
                sql = "select * from report where report_id='" + rep.ReportId + "'";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                //判断员工是否是管理员
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
                reader.Close();

                //删除数据
                sql = "delete from report where report_id='" + rep.ReportId + "'";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                con.Close();
                return 1;
            }
        }
        //修改报表状态
        [HttpPut]
        //   api/Report/ReportModifyStatus
        public int ReportModifyStatus(Report rep)
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
                sql = "update report set status='" + rep.Status + "'where report_id='" + rep.ReportId + "'";
                cmd = new MySqlCommand(sql, con);
                cmd.ExecuteNonQuery();

                con.Close();
            }
            return 1;
        }

        //审核退回
        [HttpPut]
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

                // 判断是否存在接收ID
                sql = "select * from user where user_id='" + rep.ReceiveId + "'";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                sql = "update report set status=2,level=(select distinct level from company where com_id=(select distinct com_id from user where user_id='" + rep.ReceiveId + "')) where report_id='" + rep.ReportId + "'";
                cmd = new MySqlCommand(sql, con);
                cmd.ExecuteNonQuery();

                con.Close();
            }
            return 1;
        }

        //提交审核数据
        [HttpPut]
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

                // 判断是否存在接收ID
                sql = "select * from user where user_id='" + rep.ReceiveId + "'";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                sql = "update report set status=0,level=(select distinct level from company where com_id=(select distinct com_id from user where user_id='" + rep.ReceiveId + "')) where report_id='" + rep.ReportId + "'";
                cmd = new MySqlCommand(sql, con);
                cmd.ExecuteNonQuery();

                con.Close();
            }
            return 1;
        }

        //查看一、二、三级指标
        [HttpPost]
        //   api/Report/GetIndicate123
        public List<Indicate123> GetIndicate123(Indicate ind)
        {
            List<Indicate123> indicatename123 = new List<Indicate123>();
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                // 判断是否存在报表ID
                string sql = "select * from report where report_id='" + ind.ReportId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return indicatename123;
                }
                reader.Close();

                sql = "select A.name as nameA,B.name as nameB,C.name as nameC from indicate as A,indicate as B,indicate as C where A.level=1 and B.level=2 and C.level=3 and B.parent=A.id  and C.parent=B.id";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                Indicate123 name = new Indicate123();
                while (reader.Read())
                {
                    name = new Indicate123();
                    name.IndicateName1 = reader.GetString("nameA");
                    name.IndicateName2 = reader.GetString("nameB");
                    name.IndicateName3 = reader.GetString("nameC");
                    indicatename123.Add(name);
                }
                reader.Close();

                con.Close();
            }
            return indicatename123;
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

                // 判断是否存在员工ID
                string sql = "select * from user where user_id='" + ind.UserId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return indicatename4;
                }
                reader.Close();

                sql = "select name from indicate where parent=(select id from indicate where name='" + ind.IndicateName3 + "' and parent=(select id from indicate where name='" + ind.IndicateName2 + "' and parent=(select id from indicate where name='" + ind.IndicateName1 + "')))";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    indicatename4.Add(reader.GetString("name"));
                }
                reader.Close();

                con.Close();
            }
            return indicatename4;
        }

        //查看数据
        [HttpPost]
        //   api/Report/GetData
        public List<Data> GetData([FromForm]Indicate ind)
        {
            List<Data> data = new List<Data>();
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                // 判断是否存在员工ID
                string sql = "select * from user where user_id='" + ind.UserId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return data;
                }
                reader.Close();

                // 判断是否存在报表ID
                sql = "select * from report where report_id='" + ind.ReportId + "'";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return data;
                }
                reader.Close();

                //查看定性指标
                sql = "select name,data,status from (indicate natural join data_qualitative) join report using(report_id) where parent=(select id from indicate where name='" + ind.IndicateName4 + "' and parent=(select id from indicate where name='" + ind.IndicateName3 + "' and parent=(select id from indicate where name='" + ind.IndicateName2 + "' and parent=(select id from indicate where name='" + ind.IndicateName1 + "'))))";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                Data a = new Data();
                while (reader.Read())
                {
                    a = new Data();
                    a.Name = reader.GetString("name");
                    a.TData = reader.GetString("data");
                    a.Status = reader.GetInt32("status");
                    data.Add(a);
                }
                reader.Close();

                //查看定量指标
                sql = "select name,data,status from (indicate natural join data_quantitative) join report using(report_id) where parent=(select id from indicate where name='" + ind.IndicateName4 + "' and parent=(select id from indicate where name='" + ind.IndicateName3 + "' and parent=(select id from indicate where name='" + ind.IndicateName2 + "' and parent=(select id from indicate where name='" + ind.IndicateName1 + "'))))";
                cmd = new MySqlCommand(sql, con);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    a = new Data();
                    a.Name = reader.GetString("name");
                    a.TData = reader.GetDouble("data");
                    a.Status = reader.GetInt32("status");
                    data.Add(a);
                }
                reader.Close();

                con.Close();
            }
            return data;
        }
    }

}
