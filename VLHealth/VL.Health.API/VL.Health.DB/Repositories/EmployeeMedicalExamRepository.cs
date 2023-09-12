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
    public class EmployeeMedicalExamRepository : IEmployeeMedicalExamRepository
    {
		private readonly string _connectionString;

		public EmployeeMedicalExamRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<EmployeeMedicalExam> Get()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = GetEmployeeMedicalExams();
				return connection.Query<EmployeeMedicalExam>(sql).AsList();
			}
		}

		public EmployeeMedicalExam Get(int idEmployeeMedicalExam)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = GetEmployeeMedicalExams() + @" WHERE id = @idEmployeeMedicalExam";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idEmployeeMedicalExam", idEmployeeMedicalExam);

				return connection.QueryFirstOrDefault<EmployeeMedicalExam>(sql, dynamicParameters);
			}
		}

		public int Create(EmployeeMedicalExam employeeMedicalExam)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"INSERT INTO VL_MedicalExam (IdEmployee, IdFileType, IdFile, ExpirationDate, ExamDate)
                                    VALUES(@IdEmployee, @IdFileType, @IdFile, @ExpirationDate, @ExamDate)
                     SELECT SCOPE_IDENTITY()";

				return connection.ExecuteScalar<int>(sql, employeeMedicalExam);
			}
		}

		public int Update(EmployeeMedicalExam employeeMedicalExam)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = 0;
				var sql = $@"UPDATE VL_MedicalExam
                           SET IdEmployee = @IdEmployee,
							  IdFileType = @IdFileType,
							  IdFile = @IdFile,
							  ExpirationDate = @ExpirationDate,
							  ExamDate = @ExamDate	
                         WHERE Id = @Id";

				affectedRows = connection.Execute(sql, employeeMedicalExam);
				return affectedRows;
			}
		}

        public EmployeeMedicalExam Delete(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = GetEmployeeMedicalExams() + $@" WHERE Id = @idGetEmployeeMedicalExams";
				sql += $@"; DELETE VL_MedicalExam WHERE Id = @idGetEmployeeMedicalExams ";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idGetEmployeeMedicalExams", id);

				EmployeeMedicalExam employeeMedicalExam;
				using (var multi = connection.QueryMultiple(sql, dynamicParameters))
				{
					employeeMedicalExam = multi.Read<EmployeeMedicalExam>().FirstOrDefault();
				}
				return employeeMedicalExam;
			}

		}

		#region private methods
		private string GetEmployeeMedicalExams()
		{
			var sql = @$"SELECT							
							Id,
							IdEmployee,
							IdFileType,
							IdFile,
							ExpirationDate,
							ExamDate
						FROM
							VL_MedicalExam";

			return sql;
		}

		public bool Exists(EmployeeMedicalExam employeeMedicalExam)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [VL_MedicalExam]
                                      WHERE Id != @Id AND IdEmployee = @IdEmployee AND IdFileType = @IdFileType AND ExamDate = @ExamDate), 1, 0)";

				return Convert.ToBoolean(connection.ExecuteScalar(sql, employeeMedicalExam));
			}
		}

		public bool ExistsFileType(int idFileType)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [tipoarchivo]
                                      WHERE tiparchnro = {idFileType}), 1, 0)";

				return Convert.ToBoolean(connection.ExecuteScalar(sql));
			}
		}
		#endregion
	}
}
