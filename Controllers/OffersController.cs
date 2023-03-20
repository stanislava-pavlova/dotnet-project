using JobBoard.ActionFilters;
using JobBoard.Entities;
using JobBoard.ExtentionMethods;
using JobBoard.Repositories;
using JobBoard.ViewModels.Offers;
using JobBoard.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JobBoard.Controllers
{

    [AuthenticationFilter]
    public class OffersController : Controller
    {
        public IActionResult Index(IndexVM model)
        {
            User loggedUser = this.HttpContext.Session.GetObject<User>("loggedUser");
            JbDbContext context = new JbDbContext();

            model.Pager = model.Pager ?? new PagerVM();

            model.Pager.Page = model.Pager.Page <= 0 ? 1 : model.Pager.Page;

            model.Pager.ItemsPerPage = model.Pager.ItemsPerPage <= 0 ? 8 : model.Pager.ItemsPerPage;

            List<int> sharedOfferIds = context.UserToOffers
                                           .Where(i => i.UserId == loggedUser.Id)
                                           .Select(i => i.OfferId)
                                           .ToList();

            model.Filter ??= new FilterVM(); 

            Expression<Func<Offer, bool>> filter = o =>
                   (o.OwnerId == loggedUser.Id || sharedOfferIds.Contains(o.Id)) &&
                   (string.IsNullOrEmpty(model.Filter.Title) || o.Title.Contains(model.Filter.Title)) &&
                   (string.IsNullOrEmpty(model.Filter.Category) || o.Category.Contains(model.Filter.Category)) &&
                   (string.IsNullOrEmpty(model.Filter.Description) || o.Description.Contains(model.Filter.Description));


            model.Pager.PagesCount = (int)Math.Ceiling(context.Offers.Where(o => o.OwnerId == loggedUser.Id ||
                                                       sharedOfferIds.Contains(o.Id)).Count(filter)
                                     / (double)model.Pager.ItemsPerPage);

            model.Items = context.Offers
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
            User loggedUser = this.HttpContext.Session.GetObject<User>("loggedUser");

            CreateVM model = new CreateVM();
            model.OwnerId = loggedUser.Id;

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateVM model)
        {
            User loggedUser = this.HttpContext.Session.GetObject<User>("loggedUser");

            if (model.OwnerId != loggedUser.Id)
            {
                ModelState.AddModelError("summaryError", "Owner impersonation attempt detected!");
                return View(model);
            }

            JbDbContext context = new JbDbContext();

            Offer item = new Offer();
            item.OwnerId = model.OwnerId;
            item.Title = model.Title;
            item.Description = model.Description;
            item.Category = model.Category;

            context.Offers.Add(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Offers");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            User loggedUser = this.HttpContext.Session.GetObject<User>("loggedUser");

            JbDbContext context = new JbDbContext();
            Offer item = context.Offers.Where(o => o.Id == id)
                                       .FirstOrDefault();

            if (item == null)
                return RedirectToAction("Index", "Projects");

            if (item.OwnerId != loggedUser.Id)
                return RedirectToAction("Index", "Projects");

            EditVM model = new EditVM();
            model.Id = item.Id;
            model.OwnerId = item.OwnerId;
            model.Title = item.Title;
            model.Description = item.Description;
            model.Category = item.Category;

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditVM model)
        {
            User loggedUser = this.HttpContext.Session.GetObject<User>("loggedUser");

            JbDbContext context = new JbDbContext();
            Offer item = context.Offers
                                       .Where(p => p.Id == model.Id)
                                       .FirstOrDefault();

            // проверяваме дали оригиналния запис в базата принадлежи на юзъра, чиято е сесията
            if (item.OwnerId != loggedUser.Id)
            {
                ModelState.AddModelError("summaryError", "Owner impersonation attempt detected!");
                return View(model);
            }

            // не позволяваме да се променя owner - да му слагаш различна стойност от логнатия юзър
            if (model.OwnerId != loggedUser.Id)
            {
                ModelState.AddModelError("summaryError", "Owner impersonation attempt detected!");
                return View(model);
            }

            item.Id = model.Id;
            item.OwnerId = model.OwnerId;
            item.Title = model.Title;
            item.Description = model.Description;
            item.Category = model.Category;

            context.Offers.Update(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Offers");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            JbDbContext context = new JbDbContext();
            Offer item = new Offer();
            item.Id = id;

            context.Offers.Remove(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Offers");
        }

        [HttpGet]
        public IActionResult Share(int id)
        {
            JbDbContext context = new JbDbContext();

            ShareVM model = new ShareVM();
            model.Offer = context.Offers
                                    .Where(i => i.Id == id)
                                    .FirstOrDefault();

            model.Shares = context.UserToOffers
                                    .Where(i => i.OfferId == id)
                                    .ToList();

            List<int> usersSharedList = model.Shares
                                                    .Select(i => i.UserId)
                                                    .ToList();
            usersSharedList.Add(model.Offer.OwnerId);

            model.Users = context.Users
                                       .Where(i => !usersSharedList.Contains(i.Id))
                                       .ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult CreateShare(ShareVM model)
        {
            JbDbContext context = new JbDbContext();

            foreach (int userId in model.UserIds)
            {
                UserToOffer item = new UserToOffer();
                item.UserId = userId;
                item.OfferId = model.OfferId;

                context.UserToOffers.Add(item);
                context.SaveChanges();
            }

            return RedirectToAction("Share", "Offers", new { Id = model.OfferId });
        }
        
        public IActionResult RevokeShare(int id)
        {
            JbDbContext context = new JbDbContext();

            UserToOffer item = context.UserToOffers
                                            .Where(utp => utp.Id == id)
                                            .FirstOrDefault();

            context.UserToOffers.Remove(item);
            context.SaveChanges();

            return RedirectToAction("Share", "Offers", new { Id = item.OfferId });
        }
    }
}
