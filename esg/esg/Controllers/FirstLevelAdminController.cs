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
    [Route("api/[controller]")]
    [ApiController]
    public class FirstLevelAdminController : ControllerBase
    {
        [HttpPost]
        public int AddAdmin(User user)
        {
            string connString = "server=localhost;database=esg;uid=root;pwd=123456";
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                //插入数据库失败后会报错崩溃，所以用代码判断边界条件
                //判断是否存在公司ID
                string sql = "select * from company where com_id='" + user.CompanyId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                //判断用户名和邮箱是否重复
                sql = "select account,email from user where account=@act or email=@eml";
                cmd.CommandText = sql;//更新一下sql语句
                cmd.Parameters.Add(new MySqlParameter("@act", user.UserAccount));
                cmd.Parameters.Add(new MySqlParameter("@eml", user.Email));
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Close();
                    return 0;
                }
                reader.Close();

                sql = "insert into user(com_id,account,password,email,level) values (@com_id,@account,@pwd,@email,@lvl)";
                cmd.CommandText = sql;//更新一下sql语句
                cmd.Parameters.Add(new MySqlParameter("@com_id", user.CompanyId));
                cmd.Parameters.Add(new MySqlParameter("@account", user.UserAccount));
                cmd.Parameters.Add(new MySqlParameter("@pwd", user.UserPassword));
                cmd.Parameters.Add(new MySqlParameter("@email", user.Email));
                cmd.Parameters.Add(new MySqlParameter("@lvl", user.Level));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return 1;
        }
    }
}