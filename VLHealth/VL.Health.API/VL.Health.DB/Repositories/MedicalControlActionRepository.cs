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
    public class MedicalControlActionRepository : IMedicalControlActionRepository
	{
		private readonly string _connectionString;
		public MedicalControlActionRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<MedicalControlAction> Get()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @$"SELECT
							 Id,
							 Description,
							CreateAbsence
						 FROM VL_MedicalControlAction";

				return connection.Query<MedicalControlAction>(sql).AsList();
			}
		}

		public List<MedicalControlAction> GetByControlType(int idControlType)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @$"SELECT
							 Id,
							 Description,
							 CreateAbsence
						FROM VL_MedicalControlAction 
						WHERE Id IN (SELECT IdAction FROM VL_MedicalControlAction_ControlType WHERE IdControlType = @idControlType)";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idControlType", idControlType);

				return connection.Query<MedicalControlAction>(sql, dynamicParameters).AsList();
			}
		}

		public bool Exists(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [VL_MedicalControlAction]
                                      WHERE Id = @Id), 1, 0)";
				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("id", id);

				return Convert.ToBoolean(connection.ExecuteScalar(sql, dynamicParameters));
			}
		}

	}
}
