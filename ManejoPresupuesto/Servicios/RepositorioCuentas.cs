using Dapper;
using ManejoPresupuesto.Models;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        } 

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"INSERT INTO Cuentas(Nombre, TipoCuentaId, Descripcion, Balance)
                        VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance);
                        SELECT SCOPE_IDENTITY();";
            var id = await connection.QuerySingleAsync<int>(qry, cuenta);

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"SELECT Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre AS TipoCuenta
                        FROM Cuentas
                        INNER JOIN TiposCuentas tc
                        ON tc.Id = Cuentas.TipoCuentaId
                        WHERE tc.UsuarioId = @UsuarioId
                        ORDER BY tc.Orden;";
            return await connection.QueryAsync<Cuenta>(qry, new {usuarioId});
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"SELECT Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, tc.Id
                        FROM Cuentas
                        INNER JOIN TiposCuentas tc
                        ON tc.Id = Cuentas.TipoCuentaId
                        WHERE tc.UsuarioId = @UsuarioId
                        AND Cuentas.Id = @Id;";
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(qry, new { usuarioId, id });
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"UPDATE Cuentas
                        SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion,
                        TipoCuentaId = @TipoCuentaId
                        WHERE Id = @Id;";
            await connection.ExecuteAsync(qry, cuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"DELETE Cuentas WHERE Id = @Id";
            await connection.ExecuteAsync(qry, new { id });
        }
    }
}
