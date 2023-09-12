using Dapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Libraries.Common.Helpers.Header;
using VL.Libraries.TenantDataAccess.Resolver;

namespace VL.Health.DB.Repositories
{
    public class MedicalControlTypeRepository : IMedicalControlTypeRepository
    {
		private readonly string _connectionString;
		public MedicalControlTypeRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<MedicalControlType> Get()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @$"SELECT
							 medicalControlType.tipvinro AS Id,
							 medicalControlType.tipvidesabr AS Description
						 FROM sotipvisitas medicalControlType
						 ORDER BY medicalControlType.tipvidesabr ASC";

				return connection.Query<MedicalControlType>(sql).AsList();
			}
		}


		public bool Exists(int id)
        {
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [sotipvisitas]
                                      WHERE tipvinro = @Id), 1, 0)";
				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("id", id);

				return Convert.ToBoolean(connection.ExecuteScalar(sql, dynamicParameters));
			}
		}
	}
}
