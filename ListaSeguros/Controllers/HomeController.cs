using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ListaSeguros.Data;
using ListaSeguros.Models;
using System;
using System.Threading.Tasks;

namespace ListaSeguros.Controllers
{
    public class HomeController : Controller
    {
        private readonly ListaSegurosContext _context;

        public HomeController(ListaSegurosContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new Pesquisa());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index([Bind] Pesquisa pesquisa)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Seguro seguro = null;
                    var seguros = from m in _context.Seguros
                                  select m;
                    if (pesquisa.TipoPesquisa == Enum.TipoPesquisa.Id)
                    {
                        seguro =  seguros.SingleOrDefault(f => f.Id.ToString().Equals(pesquisa.Search));
                    }
                    else if (pesquisa.TipoPesquisa == Enum.TipoPesquisa.Placa)
                    {
                        seguro = seguros.SingleOrDefault(f => f.ObjetoSegurado.Equals(pesquisa.Search));
                    }

                    if (seguro != null)
                    {
                        return RedirectToAction(nameof(Detail), "Home", new { seguro.Id });
                    }
                }
                catch
                {
                }
                pesquisa.Resultado = "Seguro não localizado";
            }
            return View(pesquisa);
        }

        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguro = _context.Seguros.SingleOrDefault(m => m.Id == id);
            if (seguro == null)
            {
                return NotFound();
            }
            if (seguro.CpfCnpj.Length == 14)
            {
                seguro.TipoPessoa = Enum.TipoPessoa.PF;
            }
            else
            {
                seguro.TipoPessoa = Enum.TipoPessoa.PJ;
            }
            return View(seguro);
        }

        public async Task<IActionResult> List(string searchString, int? pageIndex, int? pageSize)
        {
            var seguros = from s in _context.Seguros
                         select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                seguros = seguros.Where(s => s.Id.ToString().Contains(searchString) || s.CpfCnpj.Contains(searchString) || s.TipoSeguro.ToString().Contains(searchString) || s.ObjetoSegurado.Contains(searchString));
            }
            seguros = seguros.OrderBy(s => s.Id);

            return View(new Filtro() {SearchString = searchString, PageSize = pageSize ?? 20, Seguros = await PaginatedList<Seguro>.CreateAsync(seguros.AsNoTracking(), pageIndex ?? 1, pageSize ?? 20) });
        }

        public IActionResult New()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New([Bind] Seguro seguro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(seguro);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), "Home");
            }
            return View(seguro);
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguro = _context.Seguros.SingleOrDefault(m => m.Id == id);
            if (seguro == null)
            {
                return NotFound();
            }
            if (seguro.CpfCnpj.Length == 14)
            {
                seguro.TipoPessoa = Enum.TipoPessoa.PF;
            } else
            {
                seguro.TipoPessoa = Enum.TipoPessoa.PJ;
            }
            return View(seguro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind] Seguro seguro)
        {
            if (id != seguro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seguro);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Seguros.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), "Home");
            }
            return View(seguro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var seguro =  _context.Seguros.SingleOrDefault(m => m.Id == id);
            _context.Seguros.Remove(seguro);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
