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
    public class VaccineRepository : IVaccineRepository
    {
		private readonly string _connectionString;

		public VaccineRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<Vaccine> Get()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @$"SELECT							
							Id [Id],
							Description [Description]
						FROM
							[VL_Vaccine]
						ORDER BY Description ASC";

				return connection.Query<Vaccine>(sql).AsList();
			}
		}

		public bool Exists(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [VL_Vaccine]
                                      WHERE Id = {id}), 1, 0)";

				return Convert.ToBoolean(connection.ExecuteScalar(sql));
			}
		}

		public bool ExistsAll(int[] ids)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"
				SELECT
					CASE
						WHEN COUNT(1) = {ids.Length} THEN 1
						ELSE 0
					END [x]
				FROM [VL_Vaccine]
				WHERE Id IN @ids";

				var parameters = new DynamicParameters();
				parameters.Add("ids", ids);

				return Convert.ToBoolean(connection.ExecuteScalar(sql, parameters));
			}
		}
	}
}