using Microsoft.EntityFrameworkCore;
using RegTecnico.DAL;
using RegTecnico.Models;
using System.Linq.Expressions;

namespace RegTecnico.Services;
public class TecnicoService
{
    private readonly IDbContextFactory<Contexto> _dbFactory;

    public TecnicoService(IDbContextFactory<Contexto> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<bool> Existe(int id)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AnyAsync(t => t.TecnicoId == id);
    }


    public async Task<bool> ExistePorNombre(string nombre)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AnyAsync(t => t.Nombres == nombre);
    }


    private async Task<bool> Insertar(Tecnicos tecnico)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync();
        contexto.Tecnicos.Add(tecnico);
        return await contexto.SaveChangesAsync() > 0;
    }


    public async Task<bool> Modificar(Tecnicos tecnico)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync();
        contexto.Tecnicos.Update(tecnico);
        return await contexto.SaveChangesAsync() > 0;
    }


    public async Task<bool> Guardar(Tecnicos tecnico)
    {
        if (!await Existe(tecnico.TecnicoId))
            return await Insertar(tecnico);
        else
            return await Modificar(tecnico);
    }
    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync();
        var eliminados = await contexto.Tecnicos
            .Where(t => t.TecnicoId == id)
            .ExecuteDeleteAsync();
        return eliminados > 0;
    }

    public async Task<Tecnicos?> Buscar(int id)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TecnicoId == id);
    }


    public async Task<Tecnicos?> BuscarNombres(string nombre)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Nombres == nombre);
    }


    public async Task<List<Tecnicos>> Listar(Expression<Func<Tecnicos, bool>> criterio)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<List<Tecnicos>> ObtenerLista()
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AsNoTracking().ToListAsync();
    }
}