using Dapper;
using ManejoPresupuesto.Models;
using System.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task<IEnumerable<Categoria>> Buscar(int usuarioId);
        Task Crear(Categoria categoria);
        Task<Categoria> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCategorias : IRepositorioCategorias
    {
        readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"INSERT INTO Categorias(Nombre, TipoOperacionId, UsuarioId)
                        VALUES (@Nombre, @TipoOperacionId, @UsuarioId);
                        SELECT SCOPE_IDENTITY();";
            var id = await connection.QuerySingleAsync<int>(qry, categoria);
            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"SELECT Id, Nombre, TipoOperacionId, UsuarioId
                        FROM Categorias
                        WHERE UsuarioId = @UsuarioId";
            return await connection.QueryAsync<Categoria>(qry, new { usuarioId });
        }

        public async Task<Categoria> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"SELECT Id, Nombre, TipoOperacionId, UsuarioId
                        FROM Categorias
                        WHERE Id = @Id AND UsuarioId = @UsuarioId";
            return await connection.QueryFirstOrDefaultAsync<Categoria>(qry, new {id, usuarioId});
        }

        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"UPDATE Categorias
                        SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
                        WHERE Id = @Id AND UsuarioId = @UsuarioId;";
            await connection.ExecuteAsync(qry, categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"DELETE FROM Categorias
                        WHERE Id = @Id";
            await connection.ExecuteAsync(qry, new { id });
        }
    }
}
