using Microsoft.EntityFrameworkCore;
using RegTecnico.DAL;
using RegTecnico.Models;
using System.Linq.Expressions;

namespace RegTecnico.Services
{
    public class ClienteService
    {
        private readonly IDbContextFactory<Contexto> DbFactory;

        public ClienteService(IDbContextFactory<Contexto> DbFactory)
        {
            this.DbFactory = DbFactory;
        }

        public async Task<bool> Existe(int id)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Clientes.AnyAsync(c => c.ClienteId == id);
        }

        public async Task<bool> ExistePorNombre(string nombre)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Clientes.AnyAsync(c => c.Nombres == nombre);
        }

        public async Task<bool> ExistePorRnc(string rnc)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Clientes.AnyAsync(c => c.RNC == rnc);
        }

        private async Task<bool> Insertar(Clientes cliente)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Clientes.Add(cliente);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<bool> Modificar(Clientes cliente)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Clientes.Update(cliente);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<bool> Guardar(Clientes cliente)
        {
            if (!await Existe(cliente.ClienteId))
                return await Insertar(cliente);
            else
                return await Modificar(cliente);
        }

        public async Task<bool> Eliminar(int id)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            var cliente = await contexto.Clientes.FindAsync(id);
            if (cliente == null) return false;

            contexto.Clientes.Remove(cliente);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<Clientes?> Buscar(int id)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(t => t.ClienteId == id);
        }

        public async Task<Clientes?> BuscarNombres(string nombre)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Nombres == nombre);
        }

        public async Task<List<Clientes>> Listar(Expression<Func<Clientes, bool>> criterio)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Clientes.AsNoTracking()
                .Where(criterio)
                .ToListAsync();
        }

        public async Task<Clientes?> ObtenerClientePorRnc(string rnc)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.RNC == rnc);
        }

        public async Task<bool> CrearCliente(Clientes cliente)
        {
            if (await ExistePorRnc(cliente.RNC) || await ExistePorNombre(cliente.Nombres))
            {
                return false;
            }

            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Clientes.Add(cliente);
            return await contexto.SaveChangesAsync() > 0;
        }
    }
}