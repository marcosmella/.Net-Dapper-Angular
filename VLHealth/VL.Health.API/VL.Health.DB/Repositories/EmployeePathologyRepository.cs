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
    public class EmployeePathologyRepository : IEmployeePathologyRepository
    {
        private readonly string _connectionString;

        public EmployeePathologyRepository(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = tenantResolver.GetTenantConnection(new HeaderHelper(httpContextAccessor).GetTenant());
        }

        public List<EmployeePathology> Get(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @$"SELECT
							 IdPathology [Id],
                             Pathology.patologiadesabr + ' - ' + isNull(Pathology.PathologyCode,'') + ' - ' + PathologyGroup.[name] Description
						 FROM [VL_Employee_Pathology]
                         LEFT JOIN sopatologias Pathology ON Pathology.patologianro = VL_Employee_Pathology.IdPathology
                         LEFT JOIN VL_PathologyGroup PathologyGroup ON PathologyGroup.Id = Pathology.IdPathologyGroup
                         where IdEmployee = @idEmployee";

                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("idEmployee", id);

                return connection.Query<EmployeePathology>(sql, dynamicParameters).AsList();
            }
        }

        public int Update(EmployeePathologies employeePathologies)
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
                        parameters.Add("idEmployee", employeePathologies.IdEmployee);

                        var query = $"DELETE FROM [VL_Employee_Pathology] WHERE IdEmployee = @idEmployee";
                        connection.Execute(query, parameters, transaction);

                        foreach (var employeePathology in employeePathologies.Pathologies)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("idEmployee", employeePathologies.IdEmployee);
                            parameters.Add("idPathology", employeePathology.Id);

                            query = "INSERT INTO [VL_Employee_Pathology] ( IdEmployee, idPathology ) VALUES ";
                            query += $"( @idEmployee, @idPathology )";

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
