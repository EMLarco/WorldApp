using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldApp.Models;

namespace WorldApp.Controllers;

[Authorize]
public class CountriesController : Controller
{
    private readonly WorldContext _context;
    private readonly ILogger<CountriesController> _logger;

    public CountriesController(WorldContext context, ILogger<CountriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Countries
    public async Task<IActionResult> Index(string searchString, int? page, int? pageSize)
    {
        try
        {
            int defaultPageSize = pageSize ?? 10;
            int pageNumber = page ?? 1;

            var query = _context.Countries.Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString) || c.Code.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            query = query.OrderBy(c => c.Name);

            int totalItems = await query.CountAsync();
            var countries = await query
                .Skip((pageNumber - 1) * defaultPageSize)
                .Take(defaultPageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)defaultPageSize);
            ViewBag.PageSize = defaultPageSize;
            return View(countries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar países");
            TempData["Error"] = "No se pudo cargar el listado. Intente nuevamente.";
            return View(new List<Country>());
        }
    }

    // GET: Countries/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null) return NotFound();

        try
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.Code == id && !c.IsDeleted);
            if (country == null) return NotFound();
            return View(country);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalles de país {Code}", id);
            TempData["Error"] = "No se pudo cargar el país solicitado.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Countries/Create
    public IActionResult Create() => View();

    // POST: Countries/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Code,Name,Continent,Region,SurfaceArea,IndepYear,Population,LifeExpectancy,GNP,GNPOld,LocalName,GovernmentForm,HeadOfState,Capital,Code2")] Country country)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Countries.Add(country);
                await _context.SaveChangesAsync();
                TempData["Success"] = "País creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar país");
                ModelState.AddModelError("", "No se pudo guardar. Verifique que el código no esté duplicado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear país");
                ModelState.AddModelError("", "Ocurrió un error inesperado.");
            }
        }
        return View(country);
    }

    // GET: Countries/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null) return NotFound();

        var country = await _context.Countries.FindAsync(id);
        if (country == null || country.IsDeleted) return NotFound();
        return View(country);
    }

    // POST: Countries/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Code,Name,Continent,Region,SurfaceArea,IndepYear,Population,LifeExpectancy,GNP,GNPOld,LocalName,GovernmentForm,HeadOfState,Capital,Code2,IsDeleted")] Country country)
    {
        if (id != country.Code) return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Countries.Update(country);
                await _context.SaveChangesAsync();
                TempData["Success"] = "País actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrencia al editar país {Code}", id);
                if (!CountryExists(country.Code)) return NotFound();
                else ModelState.AddModelError("", "Los datos fueron modificados por otro usuario.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar país {Code}", id);
                ModelState.AddModelError("", "No se pudo actualizar el país.");
            }
        }
        return View(country);
    }

    // GET: Countries/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return NotFound();

        var country = await _context.Countries
            .FirstOrDefaultAsync(c => c.Code == id && !c.IsDeleted);
        if (country == null) return NotFound();
        return View(country);
    }

    // POST: Countries/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                country.IsDeleted = true;
                _context.Countries.Update(country);
                await _context.SaveChangesAsync();
                TempData["Success"] = "País eliminado lógicamente.";
            }
            else
            {
                TempData["Error"] = "El país no existe.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar país {Code}", id);
            TempData["Error"] = "No se pudo eliminar el país.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CountryExists(string id) => _context.Countries.Any(e => e.Code == id);
}