using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorldApp.Models;

namespace WorldApp.Controllers;

[Authorize]
public class CitiesController : Controller
{
    private readonly WorldContext _context;
    private readonly ILogger<CitiesController> _logger;

    public CitiesController(WorldContext context, ILogger<CitiesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Cities
    public async Task<IActionResult> Index(string searchString, int? page, int? pageSize)
    {
        try
        {
            int defaultPageSize = pageSize ?? 10;
            int pageNumber = page ?? 1;

            // Construir consulta base (sin ordenar aún)
            var query = _context.Cities
                .Include(c => c.Country)
                .Where(c => !c.IsDeleted);

            // Aplicar filtro de búsqueda si existe
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString) ||
                                         (c.Country != null && c.Country.Name.Contains(searchString)));
                ViewBag.SearchString = searchString;
            }

            // Ordenar después de todos los filtros
            query = query.OrderBy(c => c.Name);

            int totalItems = await query.CountAsync();
            var cities = await query
                .Skip((pageNumber - 1) * defaultPageSize)
                .Take(defaultPageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)defaultPageSize);
            ViewBag.PageSize = defaultPageSize;
            return View(cities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar ciudades");
            TempData["Error"] = "No se pudo cargar el listado. Intente nuevamente.";
            return View(new List<City>());
        }
    }

    // GET: Cities/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        try
        {
            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (city == null) return NotFound();
            return View(city);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalles de ciudad {Id}", id);
            TempData["Error"] = "No se pudo cargar la ciudad solicitada.";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Create()
    {
        await LoadCountrySelectList();
        return View();
    }

    private async Task LoadCountrySelectList(object selectedValue = null)
    {
        var countries = await _context.Countries
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
        ViewBag.CountryCode = new SelectList(countries, "Code", "Name", selectedValue);
    }
    // POST: Cities/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,CountryCode,District,Population")] City city)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Cities.Add(city);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ciudad creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar ciudad");
                ModelState.AddModelError("", "No se pudo guardar. Verifique que el país exista.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear ciudad");
                ModelState.AddModelError("", "Ocurrió un error inesperado.");
            }
        }
        await LoadCountrySelectList(city.CountryCode);
        return View(city);
    }

    // GET: Cities/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var city = await _context.Cities.FindAsync(id);
        if (city == null || city.IsDeleted) return NotFound();

        await LoadCountrySelectList(city.CountryCode);
        return View(city);
    }

    // POST: Cities/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CountryCode,District,Population,IsDeleted")] City city)
    {
        if (id != city.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Cities.Update(city);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ciudad actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrencia al editar ciudad {Id}", id);
                if (!CityExists(city.Id)) return NotFound();
                else ModelState.AddModelError("", "Los datos fueron modificados por otro usuario. Recargue y vuelva a intentar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar ciudad {Id}", id);
                ModelState.AddModelError("", "No se pudo actualizar la ciudad.");
            }
        }
        await LoadCountrySelectList(city.CountryCode);
        return View(city);
    }

    // GET: Cities/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var city = await _context.Cities
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (city == null) return NotFound();
        return View(city);
    }

    // POST: Cities/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                city.IsDeleted = true;
                _context.Cities.Update(city);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ciudad eliminada lógicamente.";
            }
            else
            {
                TempData["Error"] = "La ciudad no existe.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar ciudad {Id}", id);
            TempData["Error"] = "No se pudo eliminar la ciudad.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CityExists(int id) => _context.Cities.Any(e => e.Id == id);

}