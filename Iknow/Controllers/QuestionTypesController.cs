using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Iknow.Data;
using Iknow.Models;
using Newtonsoft.Json;
using Iknow.Models.DataWithReference;

namespace Iknow.Controllers
{
    public class QuestionTypesController : Controller
    {
        private readonly ApplicationDbContext context;

        public QuestionTypesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // GET: QuestionTypes
        public async Task<IActionResult> Index()
        {
            context.Categories.Load();
            return View(await context.QuestionsTypes.ToListAsync());
        }

        // GET: QuestionTypes/Details/5
        /*public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionType = await context.QuestionsTypes
                .FirstOrDefaultAsync(m => m.ID == id);

            if (questionType == null)
            {
                return NotFound();
            }

            context.Entry(questionType).Reference(x => x.category).Load();

            return View(questionType);
        }*/

        // GET: QuestionTypes/Create
        public IActionResult Create()
        {

            var allowCreate = context.MasterTable.FirstOrDefault(x => x.key == "allow_create_types");
            if (allowCreate == null || allowCreate.value != "true")
                return RedirectToAction(nameof(Index));
            var categories = context.Categories.Where(x => x.locked).Select(x => new string[] { x.name, x.ID.ToString() }).ToArray();
            ViewBag.Categories = categories;

            return View();
        }

        // POST: QuestionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,type,Category")] QuestionTypeWithCategory questionType)
        {
            if (ModelState.IsValid)
            {
                var category = context.Categories.Find(int.Parse(questionType.Category));
                if (category == null)
                {
                    ViewData["Error"] = "There is no such category";
                    return View();
                }
                questionType.category = category;

                context.Add(questionType);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(questionType);
        }

        // GET: QuestionTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = context.Categories.Where(x => x.locked).Select(x => new string[] { x.name, x.ID.ToString() }).ToArrayAsync();
            var questionType = await context.QuestionsTypes.FindAsync(id);

            context.Entry(questionType).Reference(x => x.category).Load();
            if (questionType == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await categories;

            return View(new QuestionTypeWithCategory(questionType, questionType.category == null ? "" : questionType.category.name));
        }

        // POST: QuestionTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,type,Category")] QuestionTypeWithCategory questionType)
        {
            if (id != questionType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var category = context.Categories.Find(int.Parse(questionType.Category));

                    questionType.category = category;

                    context.Update(questionType);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionTypeExists(questionType.ID))
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
            return View(questionType);
        }

        // GET: QuestionTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionType = await context.QuestionsTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (questionType == null)
            {
                return NotFound();
            }

            return View(questionType);
        }

        // POST: QuestionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionType = await context.QuestionsTypes.FindAsync(id);

            context.Entry(questionType).Collection(x => x.questions).Load();
            questionType.questions.ToList().ForEach(qt =>
            {
                qt.questionType = null;
                context.Update(qt);
            });
            context.QuestionsTypes.Remove(questionType);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionTypeExists(int id)
        {
            return context.QuestionsTypes.Any(e => e.ID == id);
        }
    }
}
