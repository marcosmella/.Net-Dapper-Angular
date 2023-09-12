using Dapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Data.SqlClient;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Libraries.Common.Helpers.Header;
using VL.Libraries.TenantDataAccess.Resolver;

namespace VL.Health.DB.Repositories
{
	public class EmployeeMedicalHistoryRepository : IEmployeeMedicalHistoryRepository
	{
		private readonly string _connectionString;

		public EmployeeMedicalHistoryRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public EmployeeMedicalHistory Get(int idPerson)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = GetMedicalHistoryQuery() + " WHERE [VL_MedicalHistory].IdPerson = @idEmployee";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idEmployee", idPerson);

				return connection.QueryFirstOrDefault<EmployeeMedicalHistory>(sql, dynamicParameters);
			}
		}

		public bool Exists(int idPerson)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [VL_MedicalHistory]
                                      WHERE IdPerson = {idPerson}), 1, 0)";

				return Convert.ToBoolean(connection.ExecuteScalar(sql));
			}
		}

		public int Create(EmployeeMedicalHistory medicalHistory)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = $@"INSERT INTO [VL_MedicalHistory]
							( IdPerson, IdBloodType, IdBloodPressure, IsRiskGroup )
						VALUES
							( @idPerson, @idBloodType, @idBloodPressure, @isRiskGroup )
						SELECT CAST (SCOPE_IDENTITY() as int)";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idPerson", medicalHistory.IdPerson);
				dynamicParameters.Add("idBloodType", medicalHistory.IdBloodType);
				dynamicParameters.Add("idBloodPressure", medicalHistory.IdBloodPressure);
				dynamicParameters.Add("isRiskGroup", medicalHistory.IsRiskGroup);

				return connection.ExecuteScalar<int>(sql, dynamicParameters);
			}
		}

		public int Update(EmployeeMedicalHistory medicalHistory)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = $@"UPDATE [VL_MedicalHistory]
						SET
							IdPerson = @idPerson,
							IdBloodType = @idBloodType,
							IdBloodPressure = @idBloodPressure,
							IsRiskGroup = @isRiskGroup
						WHERE
							Id = @id";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("id", medicalHistory.Id);
				dynamicParameters.Add("idPerson", medicalHistory.IdPerson);
				dynamicParameters.Add("idBloodType", medicalHistory.IdBloodType);
				dynamicParameters.Add("idBloodPressure", medicalHistory.IdBloodPressure);
				dynamicParameters.Add("isRiskGroup", medicalHistory.IsRiskGroup);

				return connection.Execute(sql, dynamicParameters);
			}
		}

		#region Private Methods

		private string GetMedicalHistoryQuery()
		{
			return $@"SELECT
						Id [Id],
						IdPerson [IdPerson],
						IdBloodType [IdBloodType],
						IdBloodPressure [IdBloodPressure],
						IsRiskGroup [IsRiskGroup]
					FROM [VL_MedicalHistory]";
		}

		#endregion
	}
}
