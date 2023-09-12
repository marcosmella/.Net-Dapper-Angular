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
    public class PathologyRepository : IPathologyRepository
	{
		private readonly string _connectionString;

		public PathologyRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<Pathology> Get(string pathologyFilter)
        {
			using (var connection = new SqlConnection(_connectionString))
			{
				var (filter, parameters) = GetPathologyFilter(pathologyFilter);

				var sql = @$"SELECT
							Pathology.patologianro AS Id,
							Pathology.patologiadesabr + ' - ' + isNull(Pathology.PathologyCode,'') + ' - ' + PathologyGroup.[name] AS Description
						FROM
							sopatologias Pathology
						INNER JOIN VL_PathologyGroup PathologyGroup ON PathologyGroup.Id = Pathology.IdPathologyGroup
						{filter}
						ORDER BY PathologyGroup.[name] ASC, pathology.patologiadesabr ASC";

				return connection.Query<Pathology>(sql, parameters).AsList();
			}
		}

		public Pathology Get(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @$"SELECT
							Pathology.patologianro AS Id,
							Pathology.patologiadesabr + ' - ' + isNull(Pathology.PathologyCode,'') + ' - ' + PathologyGroup.[name] AS Description
						FROM
							sopatologias Pathology
						INNER JOIN VL_PathologyGroup PathologyGroup ON PathologyGroup.Id = Pathology.IdPathologyGroup
						WHERE Pathology.patologianro = @id 
						ORDER BY PathologyGroup.[name] ASC, pathology.patologiadesabr ASC";
				var parameters = new DynamicParameters();
				parameters.Add("id", id);

				return connection.QueryFirstOrDefault<Pathology>(sql, parameters);
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
				FROM [sopatologias]
				WHERE patologianro IN @ids";

				var parameters = new DynamicParameters();
				parameters.Add("ids", ids);

				return Convert.ToBoolean(connection.ExecuteScalar(sql, parameters));
			}
		}


		private Tuple<string, DynamicParameters> GetPathologyFilter(string pathologyFilter)
        {
			var filter = "";
			var parameters = new DynamicParameters();

			if (pathologyFilter != null && pathologyFilter != "")
            {
				filter = "WHERE 1=1";
				var words = pathologyFilter.Split(' ');
				for (var i=0; i< words.Length; i++)
                {
					filter += @$" AND CONCAT(Pathology.patologiadesabr,isNull(Pathology.PathologyCode,''),PathologyGroup.[name]) LIKE '%' + @param{i} + '%'";
					parameters.Add($"param{i}", words[i]);
				}
			}
			return Tuple.Create(filter, parameters);
        }
	}
}
