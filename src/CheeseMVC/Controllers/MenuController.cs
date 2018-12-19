using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseMVC.ViewModels;
using CheeseMVC.Models;
using CheeseMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            IList<Menu> menuList = context.Menus.ToList();
            return View(menuList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu
                {
                    Name = addMenuViewModel.Name
                };
                context.Menus.Add(newMenu);
                context.SaveChanges();
                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult ViewMenu(int id)
        {
            Menu menu = context.Menus.Single(c => c.ID == id);
            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();
            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel
            {
                Menu = menu,
                Items = items
            };
            return View(viewMenuViewModel);


        }

        [HttpGet]
        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.Single(c => c.ID == id);
            List<Cheese> cheeseList = context.Cheeses.ToList();
            AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(menu, cheeseList);
            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if (ModelState.IsValid)
            {
                

                IList<CheeseMenu> existingItems = context.CheeseMenus
                .Where(cm => cm.CheeseID == addMenuItemViewModel.CheeseID)
                .Where(cm => cm.MenuID == addMenuItemViewModel.MenuID).ToList();
                if (existingItems.Count == 0)
                {
                    Cheese newCheese =
                   context.Cheeses.SingleOrDefault(c => c.ID == addMenuItemViewModel.CheeseID);
                    CheeseMenu cheeseMenu = new CheeseMenu
                    {
                        Menu = addMenuItemViewModel.Menu,
                        MenuID = addMenuItemViewModel.MenuID,
                        Cheese = newCheese,
                        CheeseID = newCheese.ID
                    };
                    context.CheeseMenus.Add(cheeseMenu);
                    context.SaveChanges();
                    return RedirectToAction("ViewMenu", new { id = cheeseMenu.MenuID });
                }
                else
                {
                    return View(addMenuItemViewModel);
                }
            }
            else
            {
                return View(addMenuItemViewModel);
            }
        }
    }
}
