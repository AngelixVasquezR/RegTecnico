using Microsoft.EntityFrameworkCore;
using RegTecnico.DAL;
using RegTecnico.Models;
using System.Linq.Expressions;

namespace RegTecnico.Services;

public class TecnicosService
{
    private readonly IDbContextFactory<Contexto> DbFactory;

    public TecnicosService(IDbContextFactory<Contexto> dbFactory)
    {
        DbFactory = dbFactory;
    }

    public async Task<bool> Existe(int id)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AnyAsync(t => t.TecnicoId == id);
    }

    private async Task<bool> Insertar(Tecnicos tecnico)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Tecnicos.Add(tecnico);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Tecnicos tecnico)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
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
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var Tecnicos = await contexto.Tecnicos
            .Where(t => t.TecnicoId == id).ExecuteDeleteAsync();
        return Tecnicos > 0;
    }

    public async Task<Tecnicos?> Buscar(int id)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AsNoTracking().
            FirstOrDefaultAsync(t => t.TecnicoId == id);
    }

    public async Task<Tecnicos?> BuscarNombres(string nombre)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Nombres == nombre);
    }

    public async Task<bool> ExisteporNombres(string nombre)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AnyAsync(t => t.Nombres.Equals(nombre, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Tecnicos>> Listar(Expression<Func<Tecnicos, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos.AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }
}