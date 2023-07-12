using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Searching.Data;
using Searching.Models.Domain;

namespace Searching.Controllers
{
    public class StudentsController : Controller
    {
        private readonly DTOLayer _context;

        public StudentsController(DTOLayer context)
        {
            _context = context;
        }


        public IActionResult Index(string option, string search, string sortOrder)
        {
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["GenderSortParam"] = sortOrder == "Gender" ? "gender_desc" : "Gender";
            ViewData["SubjectsSortParam"] = sortOrder == "Subjects" ? "subjects_desc" : "Subjects";
            var students = from i in _context.Students
                        select i;

            //if (!string.IsNullOrEmpty(search))
            //{
            //    students = students.Where(i => i.Name.Contains(search) || i.Gender.Contains(search) || i.Subjects.Contains(search));
            //}

            //if (option == "Subjects")
            //{
            //    students = students.Where(x => x.Subjects == search || search == null);
            //}
            //else if (option == "Gender")
            //{
            //    students = students.Where(x => x.Gender == search || search == null);
            //}
            //else
            //{
            //    students = students.Where(x => x.Name.StartsWith(search) || search == null);
            //}

            students = option switch
            {
                "Name" => students.Where(x => x.Name == search || search == null),
                "Gender" => students.Where(x => x.Gender == search || search == null),
                "Subjects" => students.Where(x => x.Subjects == search || search == null),
                _ => students.Where(i => i.Name.Contains(search) || i.Gender.Contains(search) || i.Subjects.Contains(search) || search == null),
            };
            students = sortOrder switch
            {
                "name_desc" => students.OrderByDescending(i => i.Name),
                "Gender" => students.OrderBy(i => i.Gender),
                "gender_desc" => students.OrderByDescending(i => i.Gender),
                "Subjects" => students.OrderByDescending(i => i.Subjects),
                "subjects_desc" => students.OrderByDescending(i => i.Subjects),
                _ => students.OrderBy(i => i.Name),
            };
            var searchResults = students.ToList();
            if (searchResults.Count == 0)
            {
                ViewBag.Message = "No Records Found";
            }
            return View("Index", searchResults);
        }


        public IActionResult GetSearchSuggestions(string term)
        {
            var suggestions = _context.Students
                .Where(i => i.Name.Contains(term) || i.Gender.Contains(term) || i.Subjects.Contains(term) || term == null)
                .Select(i => i.Name)
                .Distinct()
                .ToList();

            return Json(suggestions);
        }


        // GET: Students
        //public async Task<IActionResult> Index()
        //{
        //      return _context.Students != null ? 
        //                  View(await _context.Students.ToListAsync()) :
        //                  Problem("Entity set 'DTOLayer.Students'  is null.");
        //}

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Subjects")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Subjects")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'DTOLayer.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
          return (_context.Students?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
