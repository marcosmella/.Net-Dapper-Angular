using Dapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Libraries.Common.Helpers.Header;
using VL.Libraries.TenantDataAccess.Resolver;

namespace VL.Health.DB.Repositories
{
    public class MedicalServiceRepository : IMedicalServiceRepository
    {
		private readonly string _connectionString;

		public MedicalServiceRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<MedicalService> Get()
        {
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = GetMedicalServices();
				return connection.Query<MedicalService>(sql).AsList();
			}
		}

		public MedicalService Get(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = GetMedicalServices() + @" WHERE id = @idMedicalService ";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idMedicalService", id);

				return connection.QueryFirstOrDefault<MedicalService>(sql, dynamicParameters);
			}
		}

		public int Create(MedicalService medicalService)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"INSERT INTO VL_MedicalService (Company, Phone)
                                    VALUES(@Company, @Phone)
                     SELECT SCOPE_IDENTITY()";

				return connection.ExecuteScalar<int>(sql, medicalService);
			}
		}

		public int Update(MedicalService medicalService)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = 0;
				var sql = $@"UPDATE VL_MedicalService
                           SET Company = @Company, Phone = @Phone
                         WHERE Id = @Id";

				affectedRows = connection.Execute(sql, medicalService);
				return affectedRows;
			}
		}

		public MedicalService Delete(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = GetMedicalServices() + $@" WHERE Id = @idMedicalService";
				sql += $@"; DELETE VL_MedicalService WHERE Id = @idMedicalService ";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idMedicalService", id);

				MedicalService medicalService;
				using (var multi = connection.QueryMultiple(sql, dynamicParameters))
				{
					medicalService = multi.Read<MedicalService>().FirstOrDefault();
				}
				return medicalService;
			}
			
		}

		private string GetMedicalServices()
		{
			var sql = @$"SELECT							
							Id,
							Company,
							Phone
						FROM
							VL_MedicalService ";

			return sql;
		}

		public bool NameExists(MedicalService medicalService)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [VL_MedicalService]
                                      WHERE Id != @Id AND LOWER(Company) = LOWER(@Company)), 1, 0)";

				return Convert.ToBoolean(connection.ExecuteScalar(sql, medicalService));
			}
		}

		public bool Exists(int id)
        {
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [VL_MedicalService]
                                      WHERE Id = @Id), 1, 0)";
				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("id", id);

				return Convert.ToBoolean(connection.ExecuteScalar(sql, dynamicParameters));
			}
		}
	}
}
