using HridhayConnect_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;

namespace HridhayConnect_API.Infra
{
    public interface IRepositoryBase<T>
    {
        public dynamic ExecuteStoredProcedure(string query, SqlParameter[] parameters = null);

        PagedResult<T> ExecuteWithPagination(
            string query,
            SqlParameter[] parameters = null,
            int start = 0,
            int length = 10,
            string sortColumn = "",
            string sortColumnDir = "asc",
            string searchValue = "");

        //dropdwon
        List<T> ExecuteForDropdown(string query, SqlParameter[] parameters = null);

        //getbyid
        T? ExecuteSingle(string query, SqlParameter[] parameters = null);

        public (bool IsSuccess, string Message, long Id, List<string> Extra) ExecuteStoredProcedurenew(string query, List<SqlParameter> parameters, bool returnParameter = false);

        DataTable ExecuteStoredProcedureDataTable(string query, List<SqlParameter> parameters = null);

        DataSet ExecuteStoredProcedureDataSet(string query, List<SqlParameter> parameters = null);
    }
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private static string _connectionString;
        public RepositoryBase(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DataConnection");
        }

        public dynamic ExecuteStoredProcedure(string query, SqlParameter[] parameters = null)
        {
            try
            {
                dynamic exo = new System.Dynamic.ExpandoObject();

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = query;

                    if (parameters != null && parameters.Count() > 0)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    cmd.CommandTimeout = 120;
                    cmd.ExecuteNonQuery();
                    cmd.CommandTimeout = 120;

                    SqlParameterCollection paramCollection = cmd.Parameters;

                    for (int i = 0; i < paramCollection.Count; i++)
                    {
                        if (paramCollection[i].Direction == ParameterDirection.Output)
                            ((IDictionary<String, Object>)exo).Add(paramCollection[i].ParameterName.Replace("@", ""), paramCollection[i].Value);
                    }
                }

                if (((IDictionary<String, Object>)exo) != null && ((IDictionary<String, Object>)exo).Count == 1 && ((IDictionary<String, Object>)exo).ContainsKey("response"))
                {
                    return Convert.ToString(((IDictionary<String, Object>)exo).FirstOrDefault().Value);
                }

                return exo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public PagedResult<T> ExecuteWithPagination(
            string query,
            SqlParameter[] parameters = null,
            int start = 0,
            int length = 10,
            string sortColumn = "",
            string sortColumnDir = "asc",
            string searchValue = "")
        {
            try
            {
                List<T> list = new List<T>();

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Any())
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            var dt = new DataTable();
                            dt.Load(reader);

                            string json = JsonConvert.SerializeObject(dt);
                            list = JsonConvert.DeserializeObject<List<T>>(json);
                        }
                    }
                }

                var recordsTotal = list.Count;

                if (!string.IsNullOrEmpty(searchValue))
                    list = list.Where(x => JsonConvert.SerializeObject(x)
                        .ToLower().Contains(searchValue.ToLower())).ToList();

                if (!string.IsNullOrEmpty(sortColumn))
                {
                    list = sortColumnDir.ToLower() == "asc"
     ? list.OrderBy(x =>
     {
         var prop = x.GetType().GetProperty(sortColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
         var val = prop?.GetValue(x, null);
         return val is string str ? str.ToLower() : val;
     }).ToList()
     : list.OrderByDescending(x =>
     {
         var prop = x.GetType().GetProperty(sortColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
         var val = prop?.GetValue(x, null);
         return val is string str ? str.ToLower() : val;
     }).ToList();
                }

                //var pagedData = list.Skip(start).Take(length).ToList();
                var pagedData = list.ToList();

                return new PagedResult<T>
                {
                  //  StartIndex = start,
                  //  Length = length,
                    RecordsFiltered = list.Count,
                    RecordsTotal = recordsTotal,
                    Data = pagedData
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //  New method for Dropdowns 
        public List<T> ExecuteForDropdown(string query, SqlParameter[] parameters = null)
        {
            List<T> list = new List<T>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Any())
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);

                        string json = JsonConvert.SerializeObject(dt);
                        list = JsonConvert.DeserializeObject<List<T>>(json);
                    }
                }
            }

            return list;
        }

        //get by id

        public T? ExecuteSingle(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (parameters != null && parameters.Any())
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            var dt = new DataTable();
                            dt.Load(reader);

                            if (dt.Rows.Count == 0)
                                return null;

                            string json = JsonConvert.SerializeObject(dt);
                            return JsonConvert.DeserializeObject<List<T>>(json)?.FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public  (bool IsSuccess, string Message, long Id, List<string> Extra) ExecuteStoredProcedurenew(string query, List<SqlParameter> parameters, bool returnParameter = false)
        {
            var response = string.Empty;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = query;
                        //cmd.DeriveParameters();

                        if (parameters != null && parameters.Count > 0)
                            cmd.Parameters.AddRange(parameters.ToArray());

                        if (returnParameter)
                            cmd.Parameters.Add(new SqlParameter("@response", SqlDbType.VarChar, 2000) { Direction = ParameterDirection.Output });

                        cmd.CommandTimeout = 86400;
                        cmd.ExecuteNonQuery();

                        //RETURN VALUE
                        //response = cmd.Parameters["P_Response"].Value.ToString();

                        response = "S|Success";

                        if (cmd.Parameters.Contains("@response"))
                        {
                            response = cmd.Parameters["@response"].Value.ToString();
                        }

                        con.Close();
                        cmd.Parameters.Clear();
                        cmd.Dispose();

                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        cmd.Parameters.Clear();
                        cmd.Dispose();

                        throw ex;
                    }
                }
            }

            if (!string.IsNullOrEmpty(response) && response.Contains("|"))
            {
                var parts = response.Split('|');

                string msgType = parts.Length > 0 ? parts[0] : "";
                string message = parts.Length > 1 ? parts[1] : "";

                long id = 0;
                if (parts.Length > 2) long.TryParse(parts[2], out id);

                List<string> extra = new List<string>();

                if (parts.Length > 3) for (int i = 3; i < parts.Length; i++) extra.Add(parts[i]);

                return (msgType == "S", message, id, extra);
            }

            return (false, "Invalid response format.", 0, new List<string>());
        }

        public DataTable ExecuteStoredProcedureDataTable(string query, List<SqlParameter> parameters = null)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (SqlParameter param in parameters)
                            {
                                if (param.Direction == ParameterDirection.Input)
                                    param.IsNullable = true;

                                cmd.Parameters.Add(param);
                            }
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public DataSet ExecuteStoredProcedureDataSet(string query, List<SqlParameter> parameters = null)
        {
            var ds = new DataSet();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open(); // ✅ IMPORTANT

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (parameters?.Count > 0)
                            cmd.Parameters.AddRange(parameters.ToArray());

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw; // ✅ preserve stack trace
            }

            return ds;
        }

    }
}
