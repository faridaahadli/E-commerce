using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.DB
{
    public class DbHandler : IDisposable
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DBpath"].ConnectionString;
        private readonly string _connectionString;
        private SqlConnection _conn;
        private bool _disposed;

        public SqlConnection GetConnection()
        {
            return _conn;
        }

        public DbHandler(): this(connectionString)
        {
            
        }
        public DbHandler(string connectionString)
        {
            _connectionString = connectionString;
            Connect();
        }

        private void Connect()
        {
            _conn = new SqlConnection(_connectionString);
            _conn.Open();
        }

        public void OpenConnection()
        {
            if (_conn == null)
                _conn = new SqlConnection(_connectionString);

            if (_conn.State != ConnectionState.Open)
                _conn.Open();
        }

        public void CloseConnection()
        {
            if (_conn != null && _conn.State == ConnectionState.Open)
                _conn.Close();
        }

        public bool IsConAlive()
        {
            try
            {
                return ((_conn == null) || (_conn.State == ConnectionState.Closed) ||
                        (_conn.State == ConnectionState.Broken));
            }
            catch
            {
                return false;
            }
        }

        private SqlCommand GetCommand(string sqlQuery) => GetCommand(sqlQuery, null);

        private SqlCommand GetCommand(string sqlQuery, SqlParameter[] parameters)
        {
            var cmd = new SqlCommand(sqlQuery, _conn) { CommandTimeout = 3600 };
            if (parameters == null) return cmd;

            foreach (var parameter in parameters)
                cmd.Parameters.Add(parameter);
            return cmd;
        }

        public SqlDataReader ExecuteSql(string sqlQuery) => ExecuteSql(sqlQuery, null);

        public SqlDataReader ExecuteSql(string sqlQuery, SqlParameter[] parameters)
        {
            var cmd = GetCommand(sqlQuery, parameters);
            return cmd.ExecuteReader(CommandBehavior.KeyInfo);
        }

        public SqlDataReader ExecuteStoredProcedureWithDataReader(string storeProcedureName, SqlParameter[] sqlParameters)
        {
            var cmd = GetCommand(storeProcedureName, sqlParameters);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public static SqlParameter SetParameter(string parameterName, SqlDbType parameterType, int size,
            ParameterDirection direction, object value)
        {
            var param = new SqlParameter(parameterName, parameterType, size)
            {
                Direction = direction,
                Value = value
            };
            return param;
        }

        public void ExecuteStoredProcedure(string storeProcedureName) =>
            ExecuteStoredProcedure(storeProcedureName, null);

        public void ExecuteStoredProcedure(string storeProcedureName, SqlParameter[] parameters)
        {
            var cmd = GetCommand(storeProcedureName, parameters);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteScalar();
        }

        public object ExecStoredProcWithReturnIntValue(string storeProcedureName) =>
            ExecuteStoredProcedureWithReturnValue(storeProcedureName, null);

        public int ExecStoredProcWithReturnIntValue(string storeProcedureName, SqlParameter[] parameters,
            Action<SqlInfoMessageEventArgs> infoMsgCallback)
        {
            void InfoDel(object sender, SqlInfoMessageEventArgs e)
            {
                infoMsgCallback(e);
            }

            _conn.FireInfoMessageEventOnUserErrors = true;
            _conn.InfoMessage += InfoDel;

            int procResult = ExecStoredProcWithReturnIntValue(storeProcedureName, parameters);
            _conn.InfoMessage -= InfoDel;
            _conn.FireInfoMessageEventOnUserErrors = false;

            return procResult;
        }



        public int ExecStoredProcWithReturnIntValue(string storeProcedureName, SqlParameter[] parameters)
        {
            var cmd = GetCommand(storeProcedureName, parameters);
            cmd.CommandType = CommandType.StoredProcedure;

            var returnParameter = cmd.Parameters.Add("@RET_VAL", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;

            cmd.ExecuteNonQuery();

            return (int?)returnParameter.Value ?? 0;
        }

        public object ExecuteStoredProcedureWithReturnValue(string storeProcedureName) =>
            ExecuteStoredProcedureWithReturnValue(storeProcedureName, null);

        public object ExecuteStoredProcedureWithReturnValue(string storeProcedureName, SqlParameter[] parameters)
        {
            var cmd = GetCommand(storeProcedureName, parameters);
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd.ExecuteScalar();
        }

        public string ExecStoredProcWithOutputValue(string storeProcedureName, string outputParam,SqlDbType type,int size,SqlParameter[] parameters)
        {
            var cmd = GetCommand(storeProcedureName, parameters);
            var param = cmd.Parameters.Add(outputParam,type,size);
            param.Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            return param.Value.ToString();
        }

        public DataTable GetDataTable(string sqlQuery) => GetDataTable(sqlQuery, null);

        public DataTable GetDataTable(string sqlQuery, SqlParameter[] parameters)
        {
            var cmd = GetCommand(sqlQuery, parameters);
            var adp = new SqlDataAdapter(cmd);
            var dtbl = new DataTable();
            adp.Fill(dtbl);
            return dtbl;
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (_disposed) return;
            // If disposing equals true, dispose all managed
            // and unmanaged resources.

            if (disposing)
            {
                // Dispose managed resources.
                _conn.Dispose();
            }

            _disposed = true;
        }

        ~DbHandler()
        {
            Dispose(false);
        }


        
    }
}