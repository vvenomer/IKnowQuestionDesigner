﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Iknow.Data;
using Iknow.Models;
using Iknow.Models.DataWith;
using Microsoft.AspNetCore.Identity;

namespace Iknow.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private const int maxNrOfCategories = 8;
        public CategoriesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            //to be deleted after reveal
            List<CategoryWithQuestionTypeCountAndQuestionCount> model;
            ViewBag.showCounts = false;
            var showAll = context.MasterTable.FirstOrDefault(x => x.key == "show_all_categories");
            if (showAll!=null && showAll.value == "true")
            {
                context.Users.Load(); context.QuestionsTypes.Load(); context.Questions.Load();
                model = context.Categories
                    .Join(context.QuestionsTypes, c => c, qt => qt.category, (c, qt) => new { c = c, qt = qt })
                    .GroupJoin(context.Questions, cqt => cqt.qt, q => q.questionType, (cqt, q) => new { c = cqt.c, qt = cqt.qt, qC = q.Count())
                    .GroupBy(g => g.c)
                    .Select(s => new CategoryWithQuestionTypeCountAndQuestionCount(s.Key, s.Count(), s.Sum(x => x.qC)))
                    .OrderBy(x=>x.User.UserName)
                    .ToList();
                ViewBag.showCounts = true;
            }
            else if (userManager.GetUserId(HttpContext.User) == null)
            {
                model = new List<CategoryWithQuestionTypeCountAndQuestionCount>();
            }
            else
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                model = context.Categories.Where(x => x.User == user).Select(x => new CategoryWithQuestionTypeCountAndQuestionCount(x, 0, 0)).ToList();
            }
            return View(model);
        }

        // GET: Categories/Details/5
        /*public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await context.Categories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }*/

        // GET: Categories/Create
        public IActionResult Create()
        {
            if (userManager.GetUserId(HttpContext.User) == null)
                return RedirectToAction("Account/Login", "Identity");

            var user = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
            var nrOfCategories = context.Categories.Where(x => x.User == user).Count();
            if (nrOfCategories >= maxNrOfCategories)
            {
                ViewData["max"] = "You already have 8 categories. You won't be able to create more";
            }
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,name,description,locked")] Category category)
        {
            var user = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
            var nrOfCategories = context.Categories.Where(x => x.User == user).Count();
            if (nrOfCategories >= maxNrOfCategories)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                category.User = user;
                context.Add(category);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5f
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var isOwner = await IsOwnerAsync(category);
            return isOwner == null ? View(category) : (IActionResult)isOwner;
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,name,description,locked")] Category category)
        {
            if (id != category.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(category);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.ID))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await context.Categories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (category == null)
            {
                return NotFound();
            }

            var isOwner = await IsOwnerAsync(category);
            return isOwner == null ? View(category) : (IActionResult)isOwner;
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await context.Categories.FindAsync(id);

            context.Entry(category).Collection(x => x.questionTypes).Load();
            category.questionTypes.ToList().ForEach(qt =>
            {
                qt.category = null;
                context.Update(qt);
            });
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return context.Categories.Any(e => e.ID == id);
        }
        private async Task<RedirectToActionResult> IsOwnerAsync(Category category)
        {
            context.Entry(category).Reference(x => x.User).Load();

            if (category.User != await userManager.GetUserAsync(HttpContext.User))
            {
                return RedirectToAction("Account/Login", "Identity");
            }
            return null;
        }
    }
}
