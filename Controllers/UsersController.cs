using Microsoft.AspNetCore.Mvc;
using JobBoard.Entities;
using JobBoard.Repositories;
using JobBoard.ViewModels.Users;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobBoard.ActionFilters;
using JobBoard.ViewModels.Shared;
using System.Linq.Expressions;

namespace JobBoard.Controllers
{
    [AuthenticationFilter] 
    public class UsersController : Controller
    {
        [HttpGet] 
        public IActionResult Index(IndexVM model) 
        {
            JbDbContext context = new JbDbContext();

            model.Pager ??= new PagerVM(); // if not null return PagerVM //same as:
            //model.Pager = model.Pager ?? new PagerVM();

            model.Pager.Page = model.Pager.Page <= 0 ? 1 : model.Pager.Page;

            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 10 : model.Pager.ItemsPerPage;

            model.Filter ??= new FilterVM();

            Expression<Func<User, bool>> filter = u =>
                   (string.IsNullOrEmpty(model.Filter.Username)  || u.Username.Contains(model.Filter.Username)) &&
                   (string.IsNullOrEmpty(model.Filter.FirstName) || u.FirstName.Contains(model.Filter.FirstName)) &&
                   (string.IsNullOrEmpty(model.Filter.LastName)  || u.LastName.Contains(model.Filter.LastName));

            model.Pager.PagesCount = (int)Math.Ceiling(context.Users.Count(filter) / (double)model.Pager.ItemsPerPage);

            model.Items = context.Users
                                        .Where(filter)
                                        .OrderBy(i => i.Id)
                                        .Skip(model.Pager.ItemsPerPage * (model.Pager.Page - 1))
                                        .Take(model.Pager.ItemsPerPage)
                                        .ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            JbDbContext context = new JbDbContext();

            if (context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("authError", "User with this username already exists!");
                return View(model);
            }

            // save new user in the Db
            User item = new User();
            item.Username = model.Username;
            item.Password = model.Password;
            item.FirstName = model.FirstName;
            item.LastName = model.LastName;

            context.Users.Add(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Users");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            JbDbContext context = new JbDbContext();
            User item = context.Users.Where(u => u.Id == id)
                                     .FirstOrDefault();

            if (item == null)
                return RedirectToAction("Index", "Users");

            EditVM model = new EditVM();
            model.Id = item.Id;
            model.Username = item.Username;
            model.Password = item.Password;
            model.FirstName = item.FirstName;
            model.LastName = item.LastName;

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            JbDbContext context = new JbDbContext();

            if (context.Users.Any(u => u.Username == model.Username && u.Id != model.Id))
            {
                ModelState.AddModelError("authError", "User with this username already exists!");
                return View(model);
            }

            User item = new User();
            item.Id = model.Id;
            item.Username = model.Username;
            item.Password = model.Password;
            item.FirstName = model.FirstName;
            item.LastName = model.LastName;

            context.Users.Update(item);
            context.SaveChanges();

            //return RedirectToAction("Index", "Users");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            JbDbContext context = new JbDbContext();
            User item = new User();
            item.Id = id;

            context.Users.Remove(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Users");
        }
    }
}
