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
    public class EmployeeVaccineRepository : IEmployeeVaccineRepository
    {
        private readonly string _connectionString;

        public EmployeeVaccineRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
        }

        public List<EmployeeVaccine> Get(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @$"SELECT
							 Id,
							 IdEmployee,
                             IdVaccine,
                             ApplicationDate
						 FROM VL_Employee_Vaccine
                         where IdEmployee = @idEmployee";

                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("idEmployee", id);

                return connection.Query<EmployeeVaccine>(sql, dynamicParameters).AsList();
            }
        }

        public int Update(EmployeeVaccines employeeVaccines)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("idEmployee", employeeVaccines.IdEmployee);

                        var query = $"DELETE FROM [VL_Employee_Vaccine] WHERE IdEmployee = @idEmployee";
                        connection.Execute(query, parameters, transaction);

						foreach (var employeeVaccine in employeeVaccines.Vaccines)
						{
                            parameters = new DynamicParameters();
                            parameters.Add("idEmployee", employeeVaccines.IdEmployee);
                            parameters.Add("idVaccine", employeeVaccine.IdVaccine);
                            parameters.Add("applicationDate", employeeVaccine.ApplicationDate);

                            query = "INSERT INTO [VL_Employee_Vaccine] ( IdEmployee, IdVaccine, ApplicationDate ) VALUES ";
                            query += $"( @idEmployee, @idVaccine, @applicationDate )";

                            affectedRows += connection.Execute(query, parameters, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return affectedRows;
        }
    }
}
