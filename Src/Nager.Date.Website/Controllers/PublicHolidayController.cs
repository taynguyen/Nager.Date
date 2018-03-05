﻿using Bia.Countries;
using Nager.Date.Website.Model;
using SimpleMvcSitemap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nager.Date.Website.Controllers
{
    public class PublicHolidayController : Controller
    {
        public ActionResult Country(string countrycode, int year = 0)
        {
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }

            CountryCode countryCode;
            if (!Enum.TryParse(countrycode, true, out countryCode))
            {
                return View("NotFound");
            }

            var isoCountry = Iso3166Countries.GetCountryByAlpha2(countryCode.ToString());

            var item = new PublicHolidayInfo();
            item.Country = isoCountry.ActiveDirectoryName;
            item.CountryCode = countrycode;
            item.Year = year;
            item.PublicHolidays = DateSystem.GetPublicHoliday(countryCode, year).ToList();
            item.LongWeekends = DateSystem.GetLongWeekend(countryCode, year).ToList();

            if (item.PublicHolidays.Count > 0)
            {
                return View(item);
            }

            return View("NotFound");
        }

        [OutputCache(Duration = 3600, VaryByParam = "countryCode")]
        public ActionResult Sitemap(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                var indexNodes = new List<SitemapIndexNode>();
                var items = from CountryCode o in Enum.GetValues(typeof(CountryCode)) select o;
                foreach (var item in items)
                {
                    indexNodes.Add(new SitemapIndexNode(Url.Action("Sitemap", "PublicHoliday", new { countryCode = item.ToString() })));
                }

                return new SitemapProvider().CreateSitemapIndex(new SitemapIndexModel(indexNodes));
            }

            var nodes = new List<SitemapNode>();
            for (var i = 1900; i < DateTime.Today.Year + 100; i++)
            {
                nodes.Add(new SitemapNode(Url.Action("Country", "PublicHoliday", new { countryCode = countryCode.ToString(), year = i })) { ChangeFrequency = ChangeFrequency.Monthly });
            }

            return new SitemapProvider().CreateSitemap(new SitemapModel(nodes));
        }
    }
}
