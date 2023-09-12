using Dapper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data.SqlClient;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Libraries.Common.Helpers.Header;
using VL.Libraries.TenantDataAccess.Resolver;

namespace VL.Health.DB.Repositories
{
	public class AccidentTypeRepository : IAccidentTypeRepository
	{
		private readonly string _connectionString;

		public AccidentTypeRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<AccidentType> Get()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @$"SELECT							
							soclaseaccidente.claseaccnro [Id],
							soclaseaccidente.claseaccdabr [Description]
						FROM
							soclaseaccidente
						ORDER BY soclaseaccidente.claseaccdabr ASC";

				return connection.Query<AccidentType>(sql).AsList();
			}
		}
	}
}