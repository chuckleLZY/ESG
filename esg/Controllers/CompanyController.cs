using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using esg.Models;
using System.Reflection.Metadata;
using Org.BouncyCastle.Utilities;

namespace esg.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyController : Controller
    {
        string connString = "server=rm-bp1o13lfefc6t7z98io.mysql.rds.aliyuncs.com;database=esg_information_database;uid=super_esg;pwd=esg123456";
        [HttpPost]
        public ActionResult<string> createCompany(Company com)
        {
            int status = 1;
            int comid = 0;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    string sql = "insert into company(level,name,parent) values(@level,@name,@parent)";
                    cmd.Connection = conn;
                    cmd.CommandText = sql;

                    cmd.Parameters.Add(new MySqlParameter("@level", com.Level));
                    cmd.Parameters.Add(new MySqlParameter("@name", com.Name));
                    cmd.Parameters.Add(new MySqlParameter("@parent", com.Parent));

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        status = 0;
                    }

                    comid = (int)cmd.LastInsertedId;
                }
                conn.Close();
            }
            if (status == 1)
            {
                return Ok(new
                {
                    Status = true,
                    ComId = comid
                });
            }
            else
            {
                return Ok(new
                {
                    status = false,
                });
            }

        }
        [HttpPost]
        public ActionResult<string> getSubCompany(int com_id)
        {
            //0无母公司，1无子公司，2正常
            int status = 0;
            int num = 0;
            SubCompany info1 = new SubCompany();
            List<SubCompany> info2 = new List<SubCompany>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    string sql = "select com_id,level,name,parent from company where com_id=@cid";
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    //查找母公司信息
                    sql = "select com_id,level,name,parent from company where com_id=@cid";
                    cmd.Parameters.Add(new MySqlParameter("@cid", com_id));

                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SubCompany c = new SubCompany();
                        c.Com_id = reader.GetInt32("com_id");
                        c.Name = reader.GetString("name");
                        c.Level = reader.GetInt32("level");
                        info1 = c;
                        status = 1;
                    }
                    reader.Close();

                    //查找子公司信息
                    sql = "select com_id,level,name,parent from company where parent=@comid";
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new MySqlParameter("@comid", com_id));
                    
                    MySqlDataReader reader1 = cmd.ExecuteReader();
                    while (reader1.Read())
                    {
                        SubCompany c = new SubCompany();
                        c.Com_id= reader1.GetInt32("com_id");
                        c.Name = reader1.GetString("name");
                        c.Level = reader1.GetInt32("level");
                        info2.Add(c);
                        status = 2;
                        num++;
                    }
                    reader1.Close();
                }
                conn.Close();
            }

            return Ok(new
            {
                status = status,
                parentCompany = info1,
                subCompanyNum=num,
                subCompany = info2
            });

        }

        [HttpPost]
        public ActionResult<string>GetAllParentCompany()
        {
            int num = 0;
            List<int> info = new List<int>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    string sql = "select com_id from company where level=1";
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    //查找母公司id

                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int c = reader.GetInt32("com_id");
                        info.Add(c);
                        num++;
                    }
                    reader.Close();
                }
                conn.Close();
            }
            return Ok(new
            {
                cnt = num,
                parent = info
            }) ;
        }

        [HttpDelete]
        public int deleteCompany(int com_id)
        {
            int error_code = 1;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    int type = 0;
                    cmd.Connection = conn;
                    string sql = "select * from company where com_id=@com_id";
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new MySqlParameter("@com_id", com_id));

                    MySqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        //获取公司级别，母公司需删除所有子公司
                        type = reader.GetInt32(1);
                    }
                    reader.Close();
                    //删除本公司
                    {
                        sql = "delete from company where com_id = @com_id1";
                        cmd.CommandText = sql;
                        cmd.Parameters.Add(new MySqlParameter("@com_id1", com_id));

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error_code = 0;
                        }
                    }
                    //级联删除
                    if(type==1)
                    {
                        sql = "select * from company where parent = @parent";
                        cmd.CommandText = sql;
                        cmd.Parameters.Add(new MySqlParameter("@parent", com_id));
                        MySqlDataReader reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            if(deleteCompany(reader1.GetInt32(0))!=1)
                            {
                                error_code = 0;
                            }
                        }
                        reader1.Close();
                    }
                }
                conn.Close();
            }
            return error_code;
        }
    }

}
