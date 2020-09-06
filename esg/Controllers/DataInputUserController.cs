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
        public int InputData([FromBody] InputData data)
        {
            int error_code = 1;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    if (data.Type == 2)
                    {
                        string sql = "insert into data_qualitative(esg_id,report_id,report_year,data) values(@EId,@RId,@Year,@Data)";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;

                        cmd.Parameters.Add(new MySqlParameter("@EId", data.EsgId));
                        cmd.Parameters.Add(new MySqlParameter("@RId", data.ReportId));
                        cmd.Parameters.Add(new MySqlParameter("@Year", data.ReportYear));
                        cmd.Parameters.Add(new MySqlParameter("@Data", data.Data));
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error_code = 0;
                        }
                    }
                    else if (data.Type == 1)//定量输入
                    {
                        string sql = "insert into data_quantitative(esg_id,report_id,report_year,report_month,data) values(@EId,@RId,@Year,@Month,@Data)";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;

                        cmd.Parameters.Add(new MySqlParameter("@EId", data.EsgId));
                        cmd.Parameters.Add(new MySqlParameter("@RId", data.ReportId));
                        cmd.Parameters.Add(new MySqlParameter("@Year", data.ReportYear));
                        cmd.Parameters.Add(new MySqlParameter("@Month", data.ReportMonth));
                        cmd.Parameters.Add(new MySqlParameter("@Data", data.Data));
                        cmd.ExecuteNonQuery();
                        if(data.EsgId== "A1.3-35")
                        {
                            /*更新A1.3-55*/
                            string sql_1 = "insert into data_quantitative(esg_id,report_id,report_year,report_month,data) values('A1.3-55',@RId1,@Year1,@Month1,@Data1)";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_1;
                            cmd.Parameters.Add(new MySqlParameter("@RId1", data.ReportId));
                            cmd.Parameters.Add(new MySqlParameter("@Year1", data.ReportYear));
                            cmd.Parameters.Add(new MySqlParameter("@Month1", data.ReportMonth));
                            cmd.Parameters.Add(new MySqlParameter("@Data1", data.Data));
                            cmd.ExecuteNonQuery();

                            /*更新A1.3-56*/
                            string sql_2 = "select data from data_quantitative where esg_id='A0-4' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_2;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows)//如果A0-4存在
                            {
                                double A0_4 = reader.GetDouble("data");
                                reader.Close();
                                double result = double.Parse(data.Data) / A0_4;
                                string sql_3 = "insert into data_quantitative(esg_id,report_id,report_year,report_month,data) values('A1.3-56',@RId2,@Year2,@Month2,@Data2)";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_3;
                                cmd.Parameters.Add(new MySqlParameter("@RId2", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year2", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month2", data.ReportMonth));
                                cmd.Parameters.Add(new MySqlParameter("@Data2", result));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (data.EsgId == "A0-4")
                        {
                            /*更新A1.3-56*/
                            string sql_1 = "select data from data_quantitative where esg_id='A1.3-55' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_1;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows)//如果A1.3-55存在
                            {
                                double A13_55 = reader.GetDouble("data");
                                reader.Close();
                                double result = A13_55/double.Parse(data.Data);
                                string sql_3 = "insert into data_quantitative(esg_id,report_id,report_year,report_month,data) values('A1.3-56',@RId1,@Year1,@Month1,@Data1)";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_3;
                                cmd.Parameters.Add(new MySqlParameter("@RId1", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year1", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month1", data.ReportMonth));
                                cmd.Parameters.Add(new MySqlParameter("@Data1", result));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (data.EsgId == "A2.1-10.1")
                        {
                            /*更新A2.1-10*/
                            string sql_1 = "select ratio from energy_ratio where esg_id='A2.1-10' and year='" + data.ReportYear + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_1;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows)//如果系数存在
                            {
                                double coefficient = reader.GetDouble("ratio");
                                reader.Close();
                                double result = coefficient*double.Parse(data.Data);
                                string sql_2 = "insert into data_quantitative(esg_id,report_id,report_year,report_month,data) values('A2.1-10',@RId1,@Year1,@Month1,@Data1)";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_2;
                                cmd.Parameters.Add(new MySqlParameter("@RId1", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year1", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month1", data.ReportMonth));
                                cmd.Parameters.Add(new MySqlParameter("@Data1", result));
                                cmd.ExecuteNonQuery();
                            }

                            string sql_3 = "select data from data_quantitative where esg_id='A2.1-10' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_3;
                            MySqlDataReader reader_1 = cmd.ExecuteReader();
                            reader_1.Read();
                            if (reader_1.HasRows)
                            {
                                double da = reader_1.GetDouble("data");
                                reader_1.Close();
                                string sql_4 = "select ratio from greenhouse_gas_ratio where esg_id='A1.2-10' and year='" + data.ReportYear + "'";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_4;
                                MySqlDataReader reader_2 = cmd.ExecuteReader();
                                reader_2.Read();
                                if(reader_2.HasRows)
                                {
                                    double coefficient2 = reader_2.GetDouble("ratio");
                                    reader_2.Close();
                                    double result = da * coefficient2;
                                    string sql_5 = "insert into data_quantitative(esg_id,report_id,report_year,report_month,data) values('A1.2-10',@RId2,@Year2,@Month2,@Data2)";
                                    cmd.Connection = conn;
                                    cmd.CommandText = sql_5;
                                    cmd.Parameters.Add(new MySqlParameter("@RId2", data.ReportId));
                                    cmd.Parameters.Add(new MySqlParameter("@Year2", data.ReportYear));
                                    cmd.Parameters.Add(new MySqlParameter("@Month2", data.ReportMonth));
                                    cmd.Parameters.Add(new MySqlParameter("@Data2", result));
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else//error type
                    {
                        error_code = -1;
                        conn.Close();
                        return error_code;
                    }
                    
                }
                conn.Close();
            }
            return error_code;
        }

        [HttpPost]///////////////////////////////////////////////
        public int UpdataData([FromBody] InputData data)
        {
            int error_code = 1;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    if (data.Type == 2)
                    {
                        string sql = "update data_qualitative set data = @Data where esg_id = @EId and report_id = @RId and report_year =@Year";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;

                        cmd.Parameters.Add(new MySqlParameter("@RId", data.ReportId));
                        cmd.Parameters.Add(new MySqlParameter("@Year", data.ReportYear));
                        cmd.Parameters.Add(new MySqlParameter("@Data", data.Data));
                        cmd.Parameters.Add(new MySqlParameter("@EId", data.EsgId));
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error_code = 0;
                        }
                    }
                    else if (data.Type == 1)//定量输入
                    {
                        string sql = "update data_quantitative set data = @Data where esg_id = @EId and report_id = @RId and report_year =@Year and report_month=@Month";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;

                        cmd.Parameters.Add(new MySqlParameter("@RId", data.ReportId));
                        cmd.Parameters.Add(new MySqlParameter("@Year", data.ReportYear));
                        cmd.Parameters.Add(new MySqlParameter("@Month", data.ReportMonth));
                        cmd.Parameters.Add(new MySqlParameter("@Data", data.Data));
                        cmd.Parameters.Add(new MySqlParameter("@EId", data.EsgId));
                        cmd.ExecuteNonQuery();
                        if (data.EsgId == "A1.3-35")
                        {
                            /*更新A1.3-55*/
                            string sql_1 = "update data_quantitative set data = @Data1 where esg_id = 'A1.3-55' and report_id = @RId1 and report_year =@Year1 and report_month=@Month1";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_1;
                            cmd.Parameters.Add(new MySqlParameter("@RId1", data.ReportId));
                            cmd.Parameters.Add(new MySqlParameter("@Year1", data.ReportYear));
                            cmd.Parameters.Add(new MySqlParameter("@Month1", data.ReportMonth));
                            cmd.Parameters.Add(new MySqlParameter("@Data1", data.Data));
                            cmd.ExecuteNonQuery();

                            /*更新A1.3-56*/
                            string sql_2 = "select data from data_quantitative where esg_id='A0-4' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_2;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows)//如果A0-4存在
                            {
                                double A0_4 = reader.GetDouble("data");
                                reader.Close();
                                double result = double.Parse(data.Data) / A0_4;
                                string sql_3 = "update data_quantitative set data = @Data2 where esg_id = 'A1.3-56' and report_id = @RId2 and report_year =@Year2 and report_month=@Month2";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_3;
                                cmd.Parameters.Add(new MySqlParameter("@RId2", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year2", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month2", data.ReportMonth));
                                cmd.Parameters.Add(new MySqlParameter("@Data2", result));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (data.EsgId == "A0-4")
                        {
                            /*更新A1.3-56*/
                            string sql_1 = "select data from data_quantitative where esg_id='A1.3-55' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_1;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows)//如果A1.3-55存在
                            {
                                double A13_55 = reader.GetDouble("data");
                                reader.Close();
                                double result = A13_55 / double.Parse(data.Data);
                                string sql_3 = "update data_quantitative set data = @Data1 where esg_id = 'A1.3-56' and report_id = @RId1 and report_year =@Year1 and report_month=@Month1";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_3;
                                cmd.Parameters.Add(new MySqlParameter("@RId1", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year1", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month1", data.ReportMonth));
                                cmd.Parameters.Add(new MySqlParameter("@Data1", result));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (data.EsgId == "A2.1-10.1")
                        {
                            /*更新A2.1-10*/
                            string sql_1 = "select ratio from energy_ratio where esg_id='A2.1-10' and year='" + data.ReportYear + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_1;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows)//如果系数存在
                            {
                                double coefficient = reader.GetDouble("ratio");
                                reader.Close();
                                double result = coefficient * double.Parse(data.Data);
                                string sql_2 = "update data_quantitative set data = @Data1 where esg_id = 'A2.1-10' and report_id = @RId1 and report_year =@Year1 and report_month=@Month1";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_2;
                                cmd.Parameters.Add(new MySqlParameter("@RId1", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year1", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month1", data.ReportMonth));
                                cmd.Parameters.Add(new MySqlParameter("@Data1", result));
                                cmd.ExecuteNonQuery();
                            }

                            string sql_3 = "select data from data_quantitative where esg_id='A2.1-10' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_3;
                            MySqlDataReader reader_1 = cmd.ExecuteReader();
                            reader_1.Read();
                            if (reader_1.HasRows)
                            {
                                double da = reader_1.GetDouble("data");
                                reader_1.Close();
                                string sql_4 = "select ratio from greenhouse_gas_ratio where esg_id='A1.2-10' and year='" + data.ReportYear + "'";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_4;
                                MySqlDataReader reader_2 = cmd.ExecuteReader();
                                reader_2.Read();
                                if (reader_2.HasRows)
                                {
                                    double coefficient2 = reader_2.GetDouble("ratio");
                                    reader_2.Close();
                                    double result = da * coefficient2;
                                    string sql_5 = "update data_quantitative set data = @Data2 where esg_id = 'A1.2-10' and report_id = @RId2 and report_year =@Year2 and report_month=@Month2";
                                    cmd.Connection = conn;
                                    cmd.CommandText = sql_5;
                                    cmd.Parameters.Add(new MySqlParameter("@RId2", data.ReportId));
                                    cmd.Parameters.Add(new MySqlParameter("@Year2", data.ReportYear));
                                    cmd.Parameters.Add(new MySqlParameter("@Month2", data.ReportMonth));
                                    cmd.Parameters.Add(new MySqlParameter("@Data2", result));
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else//error type
                    {
                        error_code = -1;
                        conn.Close();
                        return error_code;
                    }

                }
                conn.Close();
            }
            return error_code;
        }

        [HttpPost]///////////////////////////////////////////////
        public int DeleteData([FromBody] InputData data)
        {
            int error_code = 1;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    if (data.Type == 2)
                    {
                        string sql = "delete from data_qualitative where esg_id = @EId and report_id = @RId and report_year =@Year";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;

                        cmd.Parameters.Add(new MySqlParameter("@RId", data.ReportId));
                        cmd.Parameters.Add(new MySqlParameter("@Year", data.ReportYear));
                        cmd.Parameters.Add(new MySqlParameter("@EId", data.EsgId));
                        cmd.ExecuteNonQuery();
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error_code = 0;
                        }
                    }
                    else if (data.Type == 1)//定量输入
                    {
                        string sql = "delete from data_quantitative where esg_id = @EId and report_id = @RId and report_year =@Year and report_month=@Month";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;

                        cmd.Parameters.Add(new MySqlParameter("@RId", data.ReportId));
                        cmd.Parameters.Add(new MySqlParameter("@Year", data.ReportYear));
                        cmd.Parameters.Add(new MySqlParameter("@Month", data.ReportMonth));
                        cmd.Parameters.Add(new MySqlParameter("@EId", data.EsgId));
                        cmd.ExecuteNonQuery();
                        if (data.EsgId == "A1.3-35")
                        {
                            /*更新A1.3-55*/
                            string sql_1 = "delete from data_quantitative where esg_id = 'A1.3-55' and report_id = @RId1 and report_year =@Year1 and report_month=@Month1";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_1;
                            cmd.Parameters.Add(new MySqlParameter("@RId1", data.ReportId));
                            cmd.Parameters.Add(new MySqlParameter("@Year1", data.ReportYear));
                            cmd.Parameters.Add(new MySqlParameter("@Month1", data.ReportMonth));
                            cmd.ExecuteNonQuery();

                            /*更新A1.3-56*/
                            string sql_2 = "select data from data_quantitative where esg_id='A1.3-56' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_2;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows)
                            {
                       
                                reader.Close();
                                string sql_3 = "delete from data_quantitative where esg_id = 'A1.3-56' and report_id = @RId2 and report_year =@Year2 and report_month=@Month2";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_3;
                                cmd.Parameters.Add(new MySqlParameter("@RId2", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year2", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month2", data.ReportMonth));
                               
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (data.EsgId == "A0-4")
                        {
                            /*更新A1.3-56*/
                            string sql_1 = "select data from data_quantitative where esg_id='A1.3-56' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_1;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows)//如果A1.3-55存在
                            {
                                reader.Close();
                                string sql_3 = "delete from data_quantitative where esg_id = 'A1.3-56' and report_id = @RId1 and report_year =@Year1 and report_month=@Month1";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_3;
                                cmd.Parameters.Add(new MySqlParameter("@RId1", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year1", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month1", data.ReportMonth));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (data.EsgId == "A2.1-10.1")
                        {
                            /*更新A2.1-10*/
                            string sql_1 = "select data from data_quantitative where esg_id='A2.1-10' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_1;
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows)
                            {
                                reader.Close();
                                string sql_2 = "delete from data_quantitative where esg_id = 'A2.1-10' and report_id = @RId1 and report_year =@Year1 and report_month=@Month1";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_2;
                                cmd.Parameters.Add(new MySqlParameter("@RId1", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year1", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month1", data.ReportMonth));
                                cmd.ExecuteNonQuery();
                            }

                            string sql_3 = "select data from data_quantitative where esg_id='A1.2-10' and report_id='" + data.ReportId + "' and report_year='" + data.ReportYear + "' and report_month='" + data.ReportMonth + "'";
                            cmd.Connection = conn;
                            cmd.CommandText = sql_3;
                            MySqlDataReader reader_1 = cmd.ExecuteReader();
                            reader_1.Read();
                            if (reader_1.HasRows)
                            {
                                reader_1.Close();
                                string sql_5 = "delete from data_quantitative where esg_id = 'A1.2-10' and report_id = @RId2 and report_year =@Year2 and report_month=@Month2";
                                cmd.Connection = conn;
                                cmd.CommandText = sql_5;
                                cmd.Parameters.Add(new MySqlParameter("@RId2", data.ReportId));
                                cmd.Parameters.Add(new MySqlParameter("@Year2", data.ReportYear));
                                cmd.Parameters.Add(new MySqlParameter("@Month2", data.ReportMonth));
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else//error type
                    {
                        error_code = -1;
                        conn.Close();
                        return error_code;
                    }
                }
                conn.Close();
            }
            return error_code;
        }


        //数据录入员提交审核
        [HttpPut]
        public int Submit(int report_id)
        {
            int error_code = 1;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    string sql = "update report set status=10 where report_id=@reportId";
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new MySqlParameter("@reportId", report_id));
                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        error_code = 0;
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
