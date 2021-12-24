using System;
using System.Data;
using CRMHalalBackEnd.DB;

namespace CRMHalalBackEnd.Repository
{
    public class FollowersRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int FollowerInsert(string domain, int userId = 0, string followerTenantId ="")
        {
            int followerId;
            try
            {
                using (var con = new DbHandler())
                {
                    followerId = con.ExecStoredProcWithReturnIntValue("FollowerInsert", new[]
                    {
                        DbHandler.SetParameter("@pDomain", SqlDbType.VarChar, 63, ParameterDirection.Input, domain),
                        DbHandler.SetParameter("@pFollowerTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input,
                            followerTenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });


                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not FollowerInsert...");
                Log.Error(ex);
                throw;
            }

            return followerId;
        }

        public int FollowerDelete(string domain, int userId = 0, string followerTenantId = "")
        {
            int followerId;
            try
            {
                using (var con = new DbHandler())
                {
                    followerId = con.ExecStoredProcWithReturnIntValue("FollowerDelete", new[]
                    {
                        DbHandler.SetParameter("@pDomain", SqlDbType.VarChar, 63, ParameterDirection.Input, domain),
                        DbHandler.SetParameter("@pFollowerTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input,
                            followerTenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });


                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not FollowerDelete...");
                Log.Error(ex);
                throw;
            }

            return followerId;
        }
    }
}