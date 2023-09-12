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
	public class MedicalControlTrackingTypeRepository : IMedicalControlTrackingTypeRepository
	{
		private readonly string _connectionString;

		public MedicalControlTrackingTypeRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<MedicalControlTrackingType> Get()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @$"SELECT							
							Id,
							[Description],
							CreateAbsence
						FROM
							VL_MedicalControlTrackingType
						ORDER BY [Description] ASC";

				return connection.Query<MedicalControlTrackingType>(sql).AsList();
			}
		}

		public bool Exists(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM VL_MedicalControlTrackingType
                                      WHERE Id = {id}), 1, 0)";

				return Convert.ToBoolean(connection.ExecuteScalar(sql));
			}
		}
			
	}
}
