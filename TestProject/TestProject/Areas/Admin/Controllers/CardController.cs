using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public readonly long mb = 1048576;

        public CardController(AppDbContext context,
                              IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var cards = await _context.Cards.Where(n => !n.IsDeleted).ToListAsync();

            if (cards is null)
            {
                return NotFound();
            }

            return View(cards);
        }

        public async Task<IActionResult> Details(int id)
        {
            var card = await _context.Cards.Where(n => n.Id == id && !n.IsDeleted).FirstOrDefaultAsync();

            if (card is null)
            {
                return NotFound();
            }

            return View(card);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card card)
        {
            if (card.Title is null || card.Description is null || card.ImageFile is null)
            {
                ModelState.AddModelError("", "All fields are required");
                return View();
            }

            if (!card.ImageFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("ImageFile", "You can upload only image files");
                return View();
            }

            if (card.ImageFile.Length > 2 * mb)
            {
                ModelState.AddModelError("ImageFile", "Image size is more than 2MB");
                return View();
            }

            string fileName = card.ImageFile.FileName;

            if (fileName.Length > 64)
            {
                fileName.Substring(fileName.Length - 64, 64);
            }

            string newFileName = Guid.NewGuid().ToString() + fileName;

            string path = Path.Combine(_environment.WebRootPath, "assets/uploads/images/", newFileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await card.ImageFile.CopyToAsync(stream);
            }

            card.ImageUrl = newFileName;

            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();

            return RedirectToAction(controllerName: nameof(Card), actionName: nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var card = await _context.Cards.Where(n => n.Id == id && !n.IsDeleted).FirstOrDefaultAsync();

            if (card is null)
            {
                return NotFound();
            }

            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,
                                                string cardTitle,
                                                string cardDescription,
                                                IFormFile cardImageFile)
        {
            var card = await _context.Cards.Where(n => n.Id == id && !n.IsDeleted).FirstOrDefaultAsync();

            if (card is null)
            {
                return NotFound();
            }

            if (cardTitle is null || cardDescription is null || card.ImageUrl is null)
            {
                ModelState.AddModelError("", "All fields are required");
                return View(card);
            }

            card.ImageFile = cardImageFile;

            if (!(cardImageFile is null))
            {
                if (!card.ImageFile.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("ImageFile", "You can upload only image files");
                    return View(card);
                }

                if (card.ImageFile.Length > 2 * mb)
                {
                    ModelState.AddModelError("ImageFile", "Image size is more than 2MB");
                    return View(card);
                }

                string fileName = card.ImageFile.FileName;

                if (fileName.Length > 64)
                {
                    fileName.Substring(fileName.Length - 64, 64);
                }

                string newFileName = Guid.NewGuid().ToString() + fileName;

                string path = Path.Combine(_environment.WebRootPath, "assets/uploads/images/", newFileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await card.ImageFile.CopyToAsync(stream);
                }

                card.ImageUrl = newFileName;
            }

            card.Title = cardTitle;
            card.Description = cardDescription;

            _context.Cards.Update(card);
            await _context.SaveChangesAsync();

            return RedirectToAction(controllerName: nameof(Card), actionName: nameof(Index), routeValues: new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var card = await _context.Cards.Where(n => n.Id == id && !n.IsDeleted).FirstOrDefaultAsync();

            if (card is null)
            {
                return NotFound();
            }

            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Card card)
        {
            var dbCard = await _context.Cards.Where(n => n.Id == card.Id && !n.IsDeleted).FirstOrDefaultAsync();

            if (dbCard is null)
            {
                return NotFound();
            }

            if (card is null)
            {
                return NotFound();
            }

            if (card.Title.Trim() != dbCard.Title)
            {
                ModelState.AddModelError("", "Titles don't match");
                return View(dbCard);
            }

            dbCard.IsDeleted = true;

            _context.Cards.Update(dbCard);
            await _context.SaveChangesAsync();

            return RedirectToAction(controllerName: nameof(Card), actionName: nameof(Index));
        }
    }
}
