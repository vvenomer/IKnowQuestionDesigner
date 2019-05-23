using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Iknow.Data;
using Iknow.Models;
using Microsoft.AspNetCore.Identity;
using Iknow.Models.DataWith;

namespace Iknow.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;


        public QuestionsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            List<Question> model;

            if (userManager.GetUserId(HttpContext.User) == null)
            {
                model = new List<Question>();
            }
            else
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                model = context.Questions.Where(x => x.User == user)
                    .Include(x=>x.questionType).ThenInclude(x=>x.category)
                    .ToList();
            }
            return View(model);
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await context.Questions
                .FirstOrDefaultAsync(m => m.ID == id);
            context.Entry(question).Reference(x => x.questionType).Load();
            context.Entry(question.questionType).Reference(x => x.category).Load();
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Questions/Create
        public async Task<IActionResult> Create()
        {

            var allowCreate = context.MasterTable.FirstOrDefault(x => x.key == "allow_create_questions");
            if (allowCreate == null || allowCreate.value != "true")
                return RedirectToAction(nameof(Index));

            var questionTypes = context.QuestionsTypes.Select(x=>x.type).ToListAsync();

            if (userManager.GetUserId(HttpContext.User) == null)
                return Redirect("/Identity/Account/Login");

            ViewData["QuestionTypes"] = JsonConvert.SerializeObject(await questionTypes);
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,type,hint1,hint2,hint3,answer")] QuestionWithType question)
        {
            if (ModelState.IsValid)
            {
                var type = context.QuestionsTypes.FirstOrDefault(x => x.type == question.type);
                if (type == null)
                {
                    type = new QuestionType() { type = question.type };
                    context.QuestionsTypes.Add(type);
                }
                question.questionType = type;
                question.User = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();

                context.Add(question);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var questionTypes = context.QuestionsTypes.Select(x => x.type).ToListAsync();
            
            var question = await context.Questions.FindAsync(id);
            context.Entry(question).Reference(x => x.questionType).Load();
            //context.Entry(question.questionType).Reference(x => x.category).Load();
            if (question == null)
            {
                return NotFound();
            }

            ViewData["QuestionTypes"] = JsonConvert.SerializeObject(await questionTypes);
            

            var isOwner = await IsOwnerAsync(question);
            return isOwner == null ? 
                View(new QuestionWithType(question, question.questionType == null ?
                    "" :
                    question.questionType.type)) :
                    (IActionResult)isOwner;
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,type,hint1,hint2,hint3,answer")] QuestionWithType question)
        {
            if (id != question.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var type = context.QuestionsTypes.FirstOrDefault(x => x.type == question.type);
                    if (type == null)
                    {
                        type = new QuestionType() { type = question.type };
                        context.QuestionsTypes.Add(type);
                    }
                    question.questionType = type;
                    context.Update(question);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.ID))
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
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await context.Questions
                .FirstOrDefaultAsync(m => m.ID == id);
            if (question == null)
            {
                return NotFound();
            }

            var isOwner = await IsOwnerAsync(question);
            return isOwner == null ? View(question) : (IActionResult)isOwner;
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await context.Questions.FindAsync(id);
            context.Questions.Remove(question);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return context.Questions.Any(e => e.ID == id);
        }
        private async Task<RedirectToActionResult> IsOwnerAsync(Question question)
        {
            context.Entry(question).Reference(x => x.User).Load();

            if (question.User != await userManager.GetUserAsync(HttpContext.User))
            {
                return RedirectToAction("Index");
            }
            return null;
        }
    }
}