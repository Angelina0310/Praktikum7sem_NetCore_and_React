using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KDays.Data;
using KDays.Models;
using System.IO;

namespace KDays.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DaysController : ControllerBase
    {
        private readonly ILogger<DaysController> _logger;
        private ApplicationDbContext db;

        public DaysController(ApplicationDbContext context, ILogger<DaysController> logger)
        {
            db = context;
            _logger = logger;
        }
        [HttpGet("get")]
        public List<string> Get([FromQuery] string user)
        {
            List<DateTime> Date = new List<DateTime>();
            IEnumerable<Days> days = db.Days.Where(e => e.UserID == user);
            foreach (Days day in days)
            {
                DateTime date = Convert.ToDateTime(day.KDStart);
                Date.Add(date);
            }
            Date.Sort();
            List<string> DateReturn = new List<string>();
            for (int i = (Date.Count - 1); i >= 0; i--)
            {
                string temp = Date[i].Date.Day + "." + Date[i].Date.Month + "." + Date[i].Date.Year;
                DateReturn.Add(temp);
            }
            return DateReturn;
        }
        [HttpGet("getNextKD")]
        public List<string> getNextKD([FromQuery] string user)
        {
            List<DateTime> Date = new List<DateTime>();
            IEnumerable<Days> days = db.Days.Where(e => e.UserID == user);
            foreach (Days day in days)
            {
                DateTime date = Convert.ToDateTime(day.KDStart);
                Date.Add(date);
            }
            Date.Sort();
            DateTime next = Date[Date.Count - 1];
            next = next.AddMonths(1);
            List<string> returnClass = new List<string>();
            returnClass.Add(next.Date.Day + "." + next.Date.Month + "." + next.Date.Year);
            return returnClass;
        }
        [HttpPost("add")]
        public void Add([FromBody]KDAddForm model)
        {
            DateTime date = Convert.ToDateTime(model.day);
            Days days = new Days();
            days.KDStart = date.Date.Date.ToString();
            days.UserID = model.user;
            db.Days.Add(days);
            db.SaveChanges();
        }

    }
}
