using Dapper;
using System.Data;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using System.Linq;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using VL.Libraries.TenantDataAccess.Resolver;
using Microsoft.AspNetCore.Http;
using VL.Libraries.Common.Helpers.Header;

namespace VL.Health.DB.Repositories
{
	public class MedicalControlRepository : IMedicalControlRepository
	{
		private readonly string _connectionString;
		public MedicalControlRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
		}

		public MedicalControl Get(int id)
        {
            var sql = GetMedicalControlQuery() + " WHERE MedicalControl.Id = @id";

            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("id", id);

			using (var connection = new SqlConnection(_connectionString))
			{
				MedicalControlTracking medicalControl = GetMedicalControls(sql, dynamicParameters, connection).FirstOrDefault();

				return (MedicalControl)medicalControl;
			}
        }

        public MedicalControl GetWithTracking(int id)
		{
			var sql = @$"{GetMedicalControlQuery()} 
						WHERE 
							MedicalControl.Id = @id OR MedicalControl.IdParent = @id 
						ORDER BY 
							MedicalControl.IdParent, 
							MedicalControl.[Date] DESC, 
							MedicalControl.Id DESC";
			
			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("id", id);

			using (var connection = new SqlConnection(_connectionString))
			{
				List<MedicalControlTracking> medicalControlTrackingList = GetMedicalControls(sql, dynamicParameters, connection).AsList<MedicalControlTracking>();

				var parent = medicalControlTrackingList.FirstOrDefault();
				if (parent == null)
				{
					return null;
				}

				var medicalControl = new MedicalControl()
				{
					Id = parent.Id,
					Employee = parent.Employee,
					Date = parent.Date,
					ControlType = parent.ControlType,
					Action = parent.Action,
					MedicalService = parent.MedicalService,
					OccupationalDoctor = parent.OccupationalDoctor,
					PrivateDoctorName = parent.PrivateDoctorName,
					PrivateDoctorEnrollment = parent.PrivateDoctorEnrollment,
					Diagnosis = parent.Diagnosis,
					Absence = parent.Absence,
					TrackingType = parent.TrackingType,
					IdFile = parent.IdFile,
					BreakTime = parent.BreakTime,
					TestDate = parent.TestDate,
					TestResult = parent.TestResult,
					Pathologies = parent.Pathologies
				};

				if (medicalControlTrackingList.Count > 1)
				{
					medicalControl.Tracking = medicalControlTrackingList?.Skip(1)?.ToList();
				}
				else
					medicalControl.Tracking = new List<MedicalControlTracking>();

				return medicalControl;
			}
		}

		public int Create(MedicalControlTracking medicalControl)
		{
			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("IdEmployee", medicalControl.Employee.Id);
			dynamicParameters.Add("Date", medicalControl.Date);
			dynamicParameters.Add("IdControlType", medicalControl.ControlType.Id);
			dynamicParameters.Add("IdMedicalService", medicalControl.MedicalService.Id);
			dynamicParameters.Add("IdOccupationalDoctor", medicalControl.OccupationalDoctor?.Id);
			dynamicParameters.Add("Diagnosis", medicalControl.Diagnosis);
			dynamicParameters.Add("PrivateDoctorName", medicalControl.PrivateDoctorName);
			dynamicParameters.Add("PrivateDoctorEnrollment", medicalControl.PrivateDoctorEnrollment);
			dynamicParameters.Add("IdFile", medicalControl.IdFile);
			dynamicParameters.Add("IdAction", medicalControl.Action?.Id);
			dynamicParameters.Add("BreakTime", medicalControl.BreakTime);
			dynamicParameters.Add("TestDate", medicalControl.TestDate);
			dynamicParameters.Add("TestResult", medicalControl.TestResult.HasValue ? (int?)Convert.ToInt32(medicalControl.TestResult) : null);
			dynamicParameters.Add("IdAbsence", medicalControl.Absence?.Id);
			dynamicParameters.Add("IdParent", medicalControl.IdParent);
			dynamicParameters.Add("IdTrackingType", medicalControl.TrackingType?.Id);

			var sql = @"INSERT INTO VL_MedicalControl (
									IdEmployee,
									[Date],
									Diagnosis,
									PrivateDoctorName,
									PrivateDoctorEnrollment,
									IdControlType,
									IdAction,
									IdMedicalService,
									IdOccupationalDoctor,
									IdFile,
									BreakTime,
									TestDate,
									TestResult,
									IdAbsence,
									IdParent,
									IdTrackingType
						) VALUES (
									@IdEmployee,
									@Date,
									@Diagnosis,
									@PrivateDoctorName,
									@PrivateDoctorEnrollment,
									@IdControlType,
									@IdAction,
									@IdMedicalService,
									@IdOccupationalDoctor,
									@IdFile,
									@BreakTime,
									@TestDate,
									@TestResult,
									@IdAbsence,
									@IdParent,
									@IdTrackingType
						)
                        SELECT CAST (SCOPE_IDENTITY() as int)";

			using (var connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				using var transaction = connection.BeginTransaction();
				try
				{
					var idMedicalControl = connection.ExecuteScalar<int>(sql, dynamicParameters, transaction: transaction);

					InsertPathologies(idMedicalControl, medicalControl.Pathologies, connection, transaction);

					transaction.Commit();

					return idMedicalControl;
				}
				catch (SqlException)
				{
					transaction.Rollback();
					throw;
				}
			}
        }

		public int Update(MedicalControlTracking medicalControl)
		{
			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("Id", medicalControl.Id);
			dynamicParameters.Add("Date", medicalControl.Date);
			dynamicParameters.Add("IdControlType", medicalControl.ControlType.Id);
			dynamicParameters.Add("IdMedicalService", medicalControl.MedicalService.Id);
			dynamicParameters.Add("IdOccupationalDoctor", medicalControl.OccupationalDoctor.Id == 0 ? null : (int?)medicalControl.OccupationalDoctor.Id);
			dynamicParameters.Add("Diagnosis", medicalControl.Diagnosis);
			dynamicParameters.Add("IdFile", medicalControl.IdFile);
			dynamicParameters.Add("PrivateDoctorName", medicalControl.PrivateDoctorName);
			dynamicParameters.Add("PrivateDoctorEnrollment", medicalControl.PrivateDoctorEnrollment);
			dynamicParameters.Add("IdAction", (medicalControl.Action == null) ? null : (int?)medicalControl.Action.Id);
			dynamicParameters.Add("IdFileComplaint", medicalControl.IdFileComplaint);

			var sql = @"UPDATE VL_MedicalControl 
							SET
								[Date] = @Date,
								IdControlType = @IdControlType,
								IdMedicalService = @IdMedicalService,
								IdOccupationalDoctor = @IdOccupationalDoctor,
								Diagnosis = @Diagnosis,
								IdFile = @IdFile,
								PrivateDoctorName = @PrivateDoctorName,
								PrivateDoctorEnrollment = @PrivateDoctorEnrollment,
								IdAction = @IdAction,
								IdFileComplaint = @IdFileComplaint
							WHERE
								Id = @Id";

			using (var connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				using var transaction = connection.BeginTransaction();
				try
				{
					var affectedRows = connection.Execute(sql, dynamicParameters, transaction: transaction);

					InsertPathologies(medicalControl.Id, medicalControl.Pathologies, connection, transaction);

					transaction.Commit();

					return affectedRows;
				}
				catch (SqlException)
				{
					transaction.Rollback();
					throw;
				}
			}
		}


		public int UpdateAbsenceId(MedicalControlTracking medicalControl)
		{
			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("Id", medicalControl.Id);
			dynamicParameters.Add("IdAbsence", medicalControl.Absence.Id);

			var sql = @"UPDATE VL_MedicalControl 
							SET
								IdAbsence = @IdAbsence 
							WHERE
								Id = @Id";

			using (var connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				using var transaction = connection.BeginTransaction();
				try
				{
					var affectedRows = connection.Execute(sql, dynamicParameters, transaction: transaction);
					transaction.Commit();
					return affectedRows;
				}
				catch (SqlException)
				{
					transaction.Rollback();
					throw;
				}
			}
		}


		public MedicalControlTracking Delete(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				using var transaction = connection.BeginTransaction();
				try
				{
					string sql = $@"{GetMedicalControlQuery()} WHERE MedicalControl.Id = @id";

					var dynamicParameters = new DynamicParameters();
					dynamicParameters.Add("id", id);

					MedicalControlTracking medicalControl = GetMedicalControls(sql, dynamicParameters, connection, transaction).FirstOrDefault();

					sql = $@"DELETE FROM VL_MedicalControlPathology WHERE IdMedicalControl = @id;
						 DELETE FROM VL_MedicalControl WHERE VL_MedicalControl.Id = @id OR VL_MedicalControl.IdParent = @id;";

					var affectedRows = connection.Execute(sql, dynamicParameters, transaction);

					transaction.Commit();

					return medicalControl;
				}
				catch (SqlException)
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		public bool AbsenceRelationshipExists(int idCurrentMedicalControl, int idAbsence)
        {
			var sql = @"SELECT 
							COUNT(1) 
						FROM VL_MedicalControl 
						WHERE 
							IdAbsence = @idAbsence";

			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("idAbsence", idAbsence);

			if (idCurrentMedicalControl != 0)
            {
				sql += " AND Id<> @id";
				dynamicParameters.Add("id", idCurrentMedicalControl);
			}

			using (var connection = new SqlConnection(_connectionString))
			{
				var count = (int)connection.ExecuteScalar(sql, dynamicParameters);

				return count > 0;
			}
        }


		public bool IsValidDate(DateTime medicalControlDate, int idAbsence)
        {
			var sql = @"SELECT 
							elfechadesde 
						FROM emp_lic
						WHERE 
							emp_licnro = @idAbsence";

			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("idAbsence", idAbsence);

			using (var connection = new SqlConnection(_connectionString))
			{
				var AbsenceDate = (DateTime)connection.ExecuteScalar(sql, dynamicParameters);

				return AbsenceDate <= medicalControlDate;
			}
		}

		public bool Exists(int id)
		{
			string sql = $@"SELECT IIF( 
                                EXISTS 
                                     (SELECT 1 FROM VL_MedicalControl
                                      WHERE Id = @Id), 1, 0)";
			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("id", id);

			using (var connection = new SqlConnection(_connectionString))
			{
				return Convert.ToBoolean(connection.ExecuteScalar(sql, dynamicParameters));
			}
		}


		public bool TrackingDateValid(MedicalControlTracking medicalControl)
        {
			string sql = @"DECLARE @lastDate DateTime;
							SET @lastDate = (SELECT top 1 [Date] 
												FROM VL_MedicalControl 
												WHERE IdParent = @idParent";
			if (medicalControl.Id != 0)
            {
				sql += " AND id <> @id ";
			}
												
			sql += @" ORDER BY [Date] DESC);
							SELECT IIF( @lastDate is Null or @date > @lastDate , 1, 0);";

			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("id", medicalControl.Id);
			dynamicParameters.Add("idParent", medicalControl.IdParent);
			dynamicParameters.Add("date", medicalControl.Date);

			using (var connection = new SqlConnection(_connectionString))
			{
				return Convert.ToBoolean(connection.ExecuteScalar(sql, dynamicParameters));
			}
        }

		public MedicalControlTracking GetLastControlTracking(int idParent)
		{
			var result = GetWithTracking(idParent).Tracking.First();
			return result;
		}

		public bool IsParentOrLastControlTracking(int idControl)
		{
			string sql =
			 $@"DECLARE @idParent int, @controlDate DateTime, @lastDate DateTime, @childrenAmmount int;
				SELECT @idParent = idParent, @controlDate = [Date]
				FROM VL_MedicalControl 
				WHERE Id = @idControl
				SET @lastDate = 
					(SELECT top 1 [Date] 
					FROM VL_MedicalControl 
					WHERE IdParent = @idParent
					ORDER BY [Date] DESC);
				SET @childrenAmmount = 
					(SELECT Count(*) 
					 FROM VL_MedicalControl 
					 WHERE IdParent = @idControl);
				SELECT IIF((@lastDate is Null AND  @childrenAmmount = 0) OR @controlDate >= @lastDate , 1, 0);";

			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("idControl", idControl);
			using (var connection = new SqlConnection(_connectionString))
			{
				return Convert.ToBoolean(connection.ExecuteScalar(sql, dynamicParameters));
			}
		}


		public MedicalControlTracking GetByAbsenceId(int idAbsence)
		{
			var sql = GetMedicalControlQuery() + " WHERE MedicalControl.idAbsence = @idAbsence";

			var dynamicParameters = new DynamicParameters();
			dynamicParameters.Add("idAbsence", idAbsence);
			using (var connection = new SqlConnection(_connectionString))
			{
				MedicalControlTracking medicalControl = GetMedicalControls(sql, dynamicParameters, connection).FirstOrDefault();

				return medicalControl;
			}
		}



	#region private methods
		private string GetMedicalControlQuery()
		{
			var sql = @$"SELECT
							MedicalControl.Id,
							MedicalControl.Date,
							MedicalControl.Diagnosis,
							MedicalControl.PrivateDoctorName,
							MedicalControl.PrivateDoctorEnrollment,
							MedicalControl.IdFile,
							MedicalControl.BreakTime,
							MedicalControl.TestDate,
							MedicalControl.TestResult,
							MedicalControl.IdParent,
							MedicalControl.idFileComplaint idFileComplaint,
							MedicalControl.IdEmployee Id,
							MedicalControl.IdControlType Id,
							MedicalControl.IdOccupationalDoctor Id,
							MedicalControl.IdMedicalService Id,				
							MedicalControl.IdAction Id,
							MedicalControl.IdAbsence Id,
							MedicalControl.IdTrackingType Id,
							MedicalControlPathology.IdPathology Id,
							Pathology.patologiadesabr + ' - ' + isNull(Pathology.PathologyCode,'') + ' - ' + PathologyGroup.[name] Description
						FROM VL_MedicalControl MedicalControl
						LEFT JOIN VL_MedicalControlPathology MedicalControlPathology ON MedicalControlPathology.IdMedicalControl = MedicalControl.Id
						LEFT JOIN sopatologias Pathology ON Pathology.patologianro = MedicalControlPathology.IdPathology
						LEFT JOIN VL_PathologyGroup PathologyGroup ON PathologyGroup.Id = Pathology.IdPathologyGroup
						";

			return sql;
		}

		private IEnumerable<MedicalControl> GetMedicalControls(string sql, DynamicParameters dynamicParameters, IDbConnection connection, IDbTransaction transaction = null)
		{
			var types = new[] { 
				typeof(MedicalControl), 
				typeof(Employee), 
				typeof(MedicalControlType), 
				typeof(Doctor), 
				typeof(MedicalService), 
				typeof(MedicalControlAction), 
				typeof(Absence), 
				typeof(MedicalControlTrackingType), 
				typeof(Pathology) 
			};

			var medicalControlDictionary = new Dictionary<int, MedicalControl>();
           
			return connection.Query(sql, types, 
				objects =>
				{
					var control = objects[0] as MedicalControl;
					Pathology patology = objects[8] as Pathology;

					if (!medicalControlDictionary.TryGetValue(control.Id, out MedicalControl medicalControl))
                    {
                        medicalControl = control;
						medicalControl.Employee = objects[1] as Employee;
						medicalControl.ControlType = objects[2] as MedicalControlType;
						medicalControl.OccupationalDoctor = objects[3] as Doctor;
						medicalControl.MedicalService = objects[4] as MedicalService;
						medicalControl.Action = objects[5] as MedicalControlAction;
						medicalControl.Absence = objects[6] as Absence;
						medicalControl.TrackingType = objects[7] as MedicalControlTrackingType;

						medicalControl.Pathologies = new List<Pathology>();
                        medicalControlDictionary.Add(control.Id, medicalControl);
                    }

					if (patology != null)
                    {
						medicalControl.Pathologies.Add(patology);
					}

					return medicalControl;

				}, dynamicParameters, transaction: transaction,
					splitOn: "Id").Distinct();

		}


		private void InsertPathologies(int idMedicalControl, List<Pathology> Pathologies, IDbConnection connection, IDbTransaction transaction)
        {
			var sql = $"DELETE FROM VL_MedicalControlPathology WHERE IdMedicalControl={idMedicalControl}";
			connection.Execute(sql, transaction: transaction);

			if (Pathologies.Count == 0)
            {
				return;
            }

			sql = "INSERT INTO VL_MedicalControlPathology (IdMedicalControl, IdPathology) VALUES ";
			foreach (var pathology in Pathologies)
            {
				sql += $"({idMedicalControl}, {pathology.Id}),";
            }
			connection.Execute(sql[0..^1], transaction: transaction);
		}

        #endregion
    }
}
