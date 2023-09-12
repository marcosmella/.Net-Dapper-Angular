using Dapper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;
using VL.Libraries.Common.Helpers.Header;
using VL.Libraries.TenantDataAccess.Provider;
using VL.Libraries.TenantDataAccess.Resolver;

namespace VL.Health.DB.Repositories
{
   public  class AccidentReopeningRepository: IAccidentReopeningRepository
   {
		private readonly string _connectionString;

		public AccidentReopeningRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<AccidentReopening> Get()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @$"SELECT
					soaccidente_valores_control.idvalor as Id,
					soaccidente_valores_control.valor as Description
					FROM 
						soaccidente_valores_control
					WHERE soaccidente_valores_control.idcontrol =  {(int)AccidentValueControl.Reopening}";

				return connection.Query<AccidentReopening>(sql).AsList();
			}
		}
	}
}
