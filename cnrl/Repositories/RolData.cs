using cnrl.Logica;
using cnrl.Models;
using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace cnrl.Repositories
{
    public static class RolData
    {
        private static readonly string conexion = ConfigurationManager.ConnectionStrings[Constantes.CONN].ConnectionString;

        const string QUERY = @"
            SELECT
                R.Id,                
                R.Name
            FROM
                AspNetRoles R
            {WHERE} 
            ";

        
        public static List<Rol> LeerTodo()
        {
            var query = QUERY.Replace(Constantes.WHERE, "");
            using (var db = new SqlConnection(conexion))
            {
                return db.Query<Rol>(query).ToList();
            }
        }

    }
}