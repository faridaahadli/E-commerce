using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.DB
{
    public static class SqlDataReaderExt
    {
        public static double GetDouble(this SqlDataReader dr, string col)
        {
            return (dr[col] as double?) ?? 0;
        }

        public static string GetString(this SqlDataReader dr, string col)
        {
            return dr[col].ToString();
        }

        public static int GetInt(this SqlDataReader dr, string col)
        {
            return (dr[col] as int?) ?? 0;
        }

        public static short GetShort(this SqlDataReader dr, string col)
        {
            return (dr[col] as short?) ?? 0;
        }

        public static byte GetByte(this SqlDataReader dr, string col)
        {
            return (dr[col] as byte?) ?? 0;
        }

        public static DateTime? GetDateTime(this SqlDataReader dr, string col)
        {
            if (dr.IsDBNull(dr.GetOrdinal(col)))
                return null;

            return dr.GetDateTime(dr.GetOrdinal(col));
        }

        public static bool GetBoolean(this SqlDataReader dr, string col)
        {
            return !dr.IsDBNull(dr.GetOrdinal(col)) && dr[col].ToString().Equals("1");
        }
    }
}