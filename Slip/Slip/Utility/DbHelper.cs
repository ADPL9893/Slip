using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Slip.Utility
{
    public static class DbHelper
    {
        private static string ConnectionString
        {
            get
            {
                return ConfigurationManager
                       .ConnectionStrings["StockDetailConnectionString"]
                       .ConnectionString;
            }
        }

        public static DataSet ExecuteDataSet(string storedProcedure, params SqlParameter[] parameters)
        {
            return ExecuteDataSet(storedProcedure, 30, parameters);
        }

        // Overload for procs that need longer than the default 30s (e.g. bulk XML inserts).
        public static DataSet ExecuteDataSet(string storedProcedure, int commandTimeoutSeconds, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(storedProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = commandTimeoutSeconds;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                con.Open();
                DataSet ds = new DataSet();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(ds);
                }
                return ds;
            }
        }

        public static List<Dictionary<string, object>> ExecuteReaderAsList(string storedProcedure, params SqlParameter[] parameters)
        {
            var list = new List<Dictionary<string, object>>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(storedProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            row[rdr.GetName(i)] = rdr.IsDBNull(i) ? null : rdr.GetValue(i);
                        }
                        list.Add(row);
                    }
                }
            }

            return list;
        }

        public static object ExecuteScalar(string storedProcedure, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(storedProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                con.Open();
                return cmd.ExecuteScalar();
            }
        }

        // Plain fire-and-forget call: no @MESSAGE output, just the affected row count.
        public static int ExecuteNonQuery(string storedProcedure, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(storedProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // Matches the project-wide convention: input parameters plus a single
        // @MESSAGE VARCHAR(1000) output parameter carrying the result message.
        public static string ExecuteNonQueryWithMessage(string storedProcedure, params SqlParameter[] inputParameters)
        {
            return ExecuteNonQueryWithMessage(storedProcedure, 30, inputParameters);
        }

        // Overload for procs that need longer than the default 30s (e.g. bulk XML inserts).
        public static string ExecuteNonQueryWithMessage(string storedProcedure, int commandTimeoutSeconds, params SqlParameter[] inputParameters)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(storedProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = commandTimeoutSeconds;
                if (inputParameters != null && inputParameters.Length > 0)
                {
                    cmd.Parameters.AddRange(inputParameters);
                }

                var messageParam = new SqlParameter("@MESSAGE", SqlDbType.VarChar, 1000)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(messageParam);

                con.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToString(messageParam.Value);
            }
        }
    }
}
