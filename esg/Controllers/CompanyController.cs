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
            
            using(MySqlConnection conn=new MySqlConnection(connString))
            {
                conn.Open();
                using(MySqlCommand cmd=new MySqlCommand())
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
            if(status == 1)
            {
                return Ok(new
                {
                    Status = status,
                    ComId = comid
                });
            }
            else
            {
                return Ok(new
                {
                    status = status,
                });
            }
            
        }
        [HttpPost]
        public JsonResult getSubCompany([FromBody]int com_id)
        {
            List<SubCompany> com = new List<SubCompany>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    string sql = "select * from company where parent=@cid";
                    cmd.Connection = conn;
                    cmd.CommandText = sql;

                    cmd.Parameters.Add(new MySqlParameter("@cid", com_id));
                    
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SubCompany c = new SubCompany();
                        c.Name = reader.GetString("name");
                        c.Com_id = reader.GetInt32("com_id");
                        com.Add(c);
                    }
                    reader.Close();
                }
                conn.Close();
            }
            return Json(com);
        }
        [HttpDelete]
        public int deleteCompany([FromBody]int CompanyId)
        {
            int error_code = 1;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    //删除本公司
                    string sql = "delete from company where com_id = @com_id";
                    cmd.Connection = conn;    
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new MySqlParameter("@com_id", CompanyId));

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        error_code = 0;
                    }
                }
                conn.Close();
            }
            return error_code;
        }
    }

}
