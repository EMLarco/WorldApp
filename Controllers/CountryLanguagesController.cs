using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorldApp.Models;

namespace WorldApp.Controllers;

[Authorize]
public class CountryLanguagesController : Controller
{
    private readonly WorldContext _context;
    private readonly ILogger<CountryLanguagesController> _logger;

    public CountryLanguagesController(WorldContext context, ILogger<CountryLanguagesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: CountryLanguages
    public async Task<IActionResult> Index(string searchString, int? page, int? pageSize)
    {
        try
        {
            int defaultPageSize = pageSize ?? 10;
            int pageNumber = page ?? 1;

            var query = _context.CountryLanguages
                .Include(l => l.Country)
                .Where(l => !l.IsDeleted);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.Language.Contains(searchString) ||
                                         (l.Country != null && l.Country.Name.Contains(searchString)));
                ViewBag.SearchString = searchString;
            }

            query = query.OrderBy(l => l.CountryCode).ThenBy(l => l.Language);

            int totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * defaultPageSize)
                .Take(defaultPageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)defaultPageSize);
            ViewBag.PageSize = defaultPageSize;
            return View(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar idiomas");
            TempData["Error"] = "No se pudo cargar el listado. Intente nuevamente.";
            return View(new List<CountryLanguage>());
        }
    }

    // GET: CountryLanguages/Details/5
    public async Task<IActionResult> Details(string countryCode, string language)
    {
        if (countryCode == null || language == null) return NotFound();

        try
        {
            var item = await _context.CountryLanguages
                .Include(l => l.Country)
                .FirstOrDefaultAsync(l => l.CountryCode == countryCode && l.Language == language && !l.IsDeleted);
            if (item == null) return NotFound();
            return View(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalles de idioma {CountryCode}/{Language}", countryCode, language);
            TempData["Error"] = "No se pudo cargar el idioma solicitado.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: CountryLanguages/Create


    // POST: CountryLanguages/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CountryCode,Language,IsOfficial,Percentage")] CountryLanguage countryLanguage)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.CountryLanguages.Add(countryLanguage);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Idioma agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar idioma");
                ModelState.AddModelError("", "No se pudo guardar. Verifique que el país exista y que el idioma no esté duplicado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear idioma");
                ModelState.AddModelError("", "Ocurrió un error inesperado.");
            }
        }
        await LoadCountrySelectList(countryLanguage.CountryCode);
        return View(countryLanguage);
    }

    // GET: CountryLanguages/Edit/5
    public async Task<IActionResult> Edit(string countryCode, string language)
    {
        if (countryCode == null || language == null) return NotFound();

        var item = await _context.CountryLanguages
            .FirstOrDefaultAsync(l => l.CountryCode == countryCode && l.Language == language && !l.IsDeleted);
        if (item == null) return NotFound();

        await LoadCountrySelectList(item.CountryCode);
        return View(item);
    }

    // POST: CountryLanguages/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string countryCode, string language, [Bind("CountryCode,Language,IsOfficial,Percentage,IsDeleted")] CountryLanguage countryLanguage)
    {
        if (countryCode != countryLanguage.CountryCode || language != countryLanguage.Language) return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                _context.CountryLanguages.Update(countryLanguage);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Idioma actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrencia al editar idioma {CountryCode}/{Language}", countryCode, language);
                if (!CountryLanguageExists(countryLanguage.CountryCode, countryLanguage.Language)) return NotFound();
                else ModelState.AddModelError("", "Los datos fueron modificados por otro usuario.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar idioma {CountryCode}/{Language}", countryCode, language);
                ModelState.AddModelError("", "No se pudo actualizar el idioma.");
            }
        }
        await LoadCountrySelectList(countryLanguage.CountryCode);
        return View(countryLanguage);
    }

    // GET: CountryLanguages/Delete/5
    public async Task<IActionResult> Delete(string countryCode, string language)
    {
        if (countryCode == null || language == null) return NotFound();

        var item = await _context.CountryLanguages
            .Include(l => l.Country)
            .FirstOrDefaultAsync(l => l.CountryCode == countryCode && l.Language == language && !l.IsDeleted);
        if (item == null) return NotFound();
        return View(item);
    }

    // POST: CountryLanguages/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string countryCode, string language)
    {
        try
        {
            var item = await _context.CountryLanguages
                .FirstOrDefaultAsync(l => l.CountryCode == countryCode && l.Language == language);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.CountryLanguages.Update(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Idioma eliminado lógicamente.";
            }
            else
            {
                TempData["Error"] = "El idioma no existe.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar idioma {CountryCode}/{Language}", countryCode, language);
            TempData["Error"] = "No se pudo eliminar el idioma.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CountryLanguageExists(string countryCode, string language) =>
        _context.CountryLanguages.Any(e => e.CountryCode == countryCode && e.Language == language);

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
}