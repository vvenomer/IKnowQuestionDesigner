using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Iknow.Data;
using Iknow.Models;

namespace Iknow.Controllers
{
    public class MasterTablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MasterTablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MasterTables
        public async Task<IActionResult> Index()
        {
            return View(await _context.MasterTable.ToListAsync());
        }

        // GET: MasterTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var masterTable = await _context.MasterTable
                .FirstOrDefaultAsync(m => m.ID == id);
            if (masterTable == null)
            {
                return NotFound();
            }

            return View(masterTable);
        }

        // GET: MasterTables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MasterTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,key,value")] MasterTable masterTable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(masterTable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(masterTable);
        }

        // GET: MasterTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var masterTable = await _context.MasterTable.FindAsync(id);
            if (masterTable == null)
            {
                return NotFound();
            }
            return View(masterTable);
        }

        // POST: MasterTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,key,value")] MasterTable masterTable)
        {
            if (id != masterTable.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(masterTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MasterTableExists(masterTable.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(masterTable);
        }

        // GET: MasterTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var masterTable = await _context.MasterTable
                .FirstOrDefaultAsync(m => m.ID == id);
            if (masterTable == null)
            {
                return NotFound();
            }

            return View(masterTable);
        }

        // POST: MasterTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var masterTable = await _context.MasterTable.FindAsync(id);
            _context.MasterTable.Remove(masterTable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MasterTableExists(int id)
        {
            return _context.MasterTable.Any(e => e.ID == id);
        }
    }
}
