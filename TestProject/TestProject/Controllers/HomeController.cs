using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Data;
using TestProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace TestProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM();

            homeVM.Cards = await _context.Cards.Where(n => !n.IsDeleted).ToListAsync();

            return View(homeVM);
        }
    }
}
