using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using esg.Models;
using MySql.Data.MySqlClient;
using System.Data.Odbc;

namespace esg.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataInputUserController : ControllerBase
    {
        string connString = "server=rm-bp1o13lfefc6t7z98io.mysql.rds.aliyuncs.com;database=esg_information_database;uid=super_esg;pwd=esg123456";

        //录入指标信息(定性)
        [HttpPost]
        public int InputDataF([FromBody] DataQualitative data)
        {
            int error_code = 1;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    string sql = "insert into data_qualitative(esg_id,report_id,report_year,data) values(@EId,@RId,@Year,@Data)";
                    cmd.Connection = conn;
                    cmd.CommandText = sql;

                    cmd.Parameters.Add(new MySqlParameter("@EId", data.EsgId));
                    cmd.Parameters.Add(new MySqlParameter("@RId", data.ReportId));
                    cmd.Parameters.Add(new MySqlParameter("@Year", System.DateTime.Now.Year));
                    cmd.Parameters.Add(new MySqlParameter("@Data", data.Data));

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        error_code = 0;
                    }
                }
                conn.Close();
            }
            return error_code;
        }

        //录入指标信息(定量)
        [HttpPost]
        public int InputDataT([FromBody] DataQuantitative data)
        {
            int error_code = 1;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    double temp = data.Data;
                    if (data.Type == 1||data.Type==11||data.Type==12)//定量输入
                    {
                        if (data.Type == 11 || data.Type == 12)//定量计算
                        {
                            string operation = "";
                            string _sql = "select * from indicate";
                            cmd.Connection = conn;
                            cmd.CommandText = _sql;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            //获取source，计算data
                            while (reader.Read())
                            {
                                if (reader.GetInt32(1) == 5)
                                {
                                    if (reader.GetString(5) == data.EsgId)
                                    {
                                        operation = reader.GetString(9);//获取计算公式
                                        reader.Close();
                                        break;
                                    }
                                }
                            }
                            data.Data = solve(operation);

                        }
                        string sql = "insert into data_quantitative(esg_id,report_id,report_year,report_month,data) values(@EId,@RId,@Year,@Month,@Data)";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;

                        cmd.Parameters.Add(new MySqlParameter("@EId", data.EsgId));
                        cmd.Parameters.Add(new MySqlParameter("@RId", data.ReportId));
                        cmd.Parameters.Add(new MySqlParameter("@Year", System.DateTime.Now.Year));
                        cmd.Parameters.Add(new MySqlParameter("@Month", System.DateTime.Now.Month));
                        cmd.Parameters.Add(new MySqlParameter("@Data", data.Data));

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error_code = 0;
                        }
                    }
                    else
                    {
                        //错误type，返回-1
                        error_code = -1;
                    }
                }
                conn.Close();
            }
            return error_code;
        }

        double solve(string operation)
        {
            double sum = 0;
            int op = 0;
            string temp = "";
            for (int i=0;i<operation.Length;++i)
            {
                if(operation[i]!='+'&&operation[i]!='*'&&operation[i]!='/')
                {
                    temp += operation[i];
                }
                else
                {
                    double t = getValue(temp);
                    temp = "";
                    switch (op)
                    {
                        case 0: sum = t; break;
                        case 1: sum += t; break;
                        case 2: sum *= t; break;
                        case 3: sum /= t; break;
                    }
                    switch (operation[i])
                    {
                        case '+': op = 1; break;
                        case '*': op = 2; break;
                        case '/': op = 3; break;
                    }
                }
                if(i==operation.Length-1)
                {
                    double t = getValue(temp);
                    switch (op)
                    {
                        case 0: sum = t; break;
                        case 1: sum += t; break;
                        case 2: sum *= t; break;
                        case 3: sum /= t; break;
                    }
                }
            }
            return sum;
        }

        double getValue(string esg_id)
        {
            double num = 0;
            int flag = 1;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    //在三个表中查找数据
                    if (flag == 1)
                    {
                        string sql = "select * from data_quantitative";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader.GetString(0) == esg_id)
                            {
                                flag = 0;
                                num = reader.GetDouble(4);
                            }
                        }
                        reader.Close();
                    }

                    if(flag==1)
                    {
                        string sql = "select * from energy_ratio";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader.GetString(0) == esg_id)
                            {
                                flag = 0;
                                num = reader.GetDouble(2);
                            }
                        }
                        reader.Close();
                    }

                    if(flag==1)
                    {
                        string sql = "select * from greenhouse_gas_ratio";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader.GetString(0) == esg_id)
                            {
                                flag = 0;
                                num = reader.GetDouble(4);
                            }
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }
            return num;
        }
    }
}
