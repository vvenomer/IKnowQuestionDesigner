﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Iknow.Data;
using Iknow.Models;
using Newtonsoft.Json;
using Iknow.Models.DataWith;

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
            //liczba osób -> ile każda osoba ma pytań
            //jeśli ma więcej niż summa innych - zaznacz na czerowno
            ViewBag.Users = context.Users.Select(x => x.UserName).ToList();
            var model = context.QuestionsTypes
                .Include(qt => qt.category)
                .GroupJoin(context.Questions, qt => qt, q => q.questionType, (qt, q) => new
                {
                    qt,
                    uqC = q.Join(context.Users, qq => qq.User, u => u, (qq, u) => new { q = qq, u })
                        .GroupBy(g => g.u)
                        .Select(s => new { u = s.Key, qC = s.Count() })
                })
                .Select(s => new QuestionTypeWithQuestionCount(s.qt, s.uqC.Select(x => new UserWithQuestionCount { user = x.u.UserName, questionCount = x.qC }).ToList()))
                .ToListAsync();

            return View(await model);
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
                if (questionType.Category != null)
                    questionType.category = context.Categories.Find(int.Parse(questionType.Category));
                context.Add((QuestionType)questionType);
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
                    if(questionType.Category != null)
                        questionType.category = context.Categories.Find(int.Parse(questionType.Category));

                    context.Update((QuestionType)questionType);
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
