using Dapper;
using ManejoPresupuesto.Models;
using System.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioTransacciones
    {
        Task Crear(Transaccion transaccion);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"Transacciones_Insertar";
            var id = await connection.QuerySingleAsync<int>(qry, 
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }
        
        public async Task Actualizar(Transaccion transaccion, decimal MontoAnterior, int cuentaAnterior)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar", 
                new
                {
                    transaccion.Id, 
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    MontoAnterior,
                    cuentaAnterior
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var qry = @"SELECT Transacciones.*, cat.TipoOperacionId
                        FROM Transacciones
                        INNER JOIN Categorias cat
                        ON cat.Id = Transacciones.CategoriaId
                        WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId;";
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(qry, new { id, usuarioId });
        }
    }
}
