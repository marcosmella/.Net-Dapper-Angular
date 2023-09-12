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
	public class DoctorRepository : IDoctorRepository
	{
		private readonly string _connectionString;

		public DoctorRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public List<Doctor> Get()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = GetDoctors();
				return connection.Query<Doctor>(sql).AsList();
			}
		}

		public Doctor Get(int idDoctor)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = GetDoctors() + @" WHERE id = @idDoctor";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idDoctor", idDoctor);

				return connection.QueryFirstOrDefault<Doctor>(sql, dynamicParameters);
			}
		}

		public int Create(Doctor doctor)
        {
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"INSERT INTO VL_doctor (Name, Surname, Enrollment, EnrollmentExpirationDate, Document, DocumentExpirationDate)
                                    VALUES(@FirstName, @Lastname, @Enrollment, @EnrollmentExpirationDate, @DocumentNumber, @DocumentExpirationDate)
                     SELECT SCOPE_IDENTITY()";

				return connection.ExecuteScalar<int>(sql, doctor);
			}
        }

        public int Update(Doctor doctor)
        {
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = 0;
				var sql = $@"UPDATE VL_doctor
                           SET Name = @FirstName,
							  Surname = @LastName,
							  Enrollment = @Enrollment,
							  EnrollmentExpirationDate = @EnrollmentExpirationDate,
							  Document = @DocumentNumber,
							  DocumentExpirationDate = @DocumentExpirationDate
                         WHERE Id = @Id";

				affectedRows = connection.Execute(sql, doctor);
				return affectedRows;
			}
        }

		public Doctor Delete(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = GetDoctors() + $@" WHERE Id = @idDoctor";
				sql += $@"; DELETE VL_doctor WHERE Id = @idDoctor ";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idDoctor", id);

				Doctor doctor;
				using (var multi = connection.QueryMultiple(sql, dynamicParameters))
				{
					doctor = multi.Read<Doctor>().FirstOrDefault();
				}
				return doctor;
			}

		}

		private string GetDoctors()
        {
			var sql = @$"SELECT							
							Id,
							Name as FirstName,
							SurName as LastName,
							Enrollment,
							EnrollmentExpirationDate,
							Document as DocumentNumber,
							DocumentExpirationDate 
						FROM
							VL_doctor";

			return sql;
		}

		public bool Exists(Doctor doctor)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [VL_doctor]
                                      WHERE Id != @Id AND Enrollment = @Enrollment), 1, 0)";

				return Convert.ToBoolean(connection.ExecuteScalar(sql, doctor));
			}
		}


		public bool Exists(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM [VL_doctor]
                                      WHERE Id = @idDoctor), 1, 0)";

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("idDoctor", id);

				return Convert.ToBoolean(connection.ExecuteScalar(sql, dynamicParameters));
			}
		}

	}
}
