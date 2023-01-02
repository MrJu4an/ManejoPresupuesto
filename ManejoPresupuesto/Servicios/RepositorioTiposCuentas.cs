using Dapper;
using ManejoPresupuesto.Models;
using System.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            var qry = $@"INSERT INTO TiposCuentas 
                        (Nombre, UsuarioId, Orden)
                        VALUES (@Nombre, @UsuarioId, 0);
                        SELECT SCOPE_IDENTITY();";

            var id = await connection.QuerySingleAsync<int>(qry, tipoCuenta);

            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            var qry = $@"SELECT 1
                        FROM TiposCuentas
                        WHERE Nombre = @Nombre 
                        AND UsuarioId = @UsuarioId;";

            var existe = await connection.QueryFirstOrDefaultAsync<int>(qry, new {nombre, usuarioId});

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            var qry = $@"SELECT Id, Nombre, Orden
                        FROM TiposCuentas
                        WHERE UsuarioId = @UsuarioId";
            return await connection.QueryAsync<TipoCuenta>(qry, new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            var qry = $@"UPDATE TiposCuentas
                        SET Nombre = @Nombre
                        WHERE Id = @Id";

            await connection.ExecuteAsync(qry, tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            var qry = $@"SELECT Id, Nombre, Orden
                        FROM TiposCuentas
                        WHERE Id = @Id AND UsuarioId = @UsuarioId";

            return await connection.QueryFirstAsync<TipoCuenta>(qry, new {id, usuarioId});
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);

            var qry = $@"DELETE TiposCuentas
                        WHERE Id = @Id";

            await connection.ExecuteAsync(qry, new { id });
        }
    }
}
