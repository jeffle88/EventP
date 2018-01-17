using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Project.Controllers
{
    public class ProjectController : Controller
    {
        private ProjectContext _context;

        public int ActivityId { get; private set; }

        public ProjectController(ProjectContext context)
        {
            _context = context;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [HttpGet]
        [Route("Home")]
        public IActionResult Home()
        {
            int? UserId = HttpContext.Session.GetInt32("LogUserId");
            if(UserId == null)
            {
                return Redirect("/");
            }
            User user = _context.user.Where(b => b.UserId == UserId).SingleOrDefault();
            ViewBag.CurUser = user;
            List<Activity> AllEvent = _context.activity.Where(c => c.UserId != user.UserId).Where(c => c.Date > DateTime.Now).Include(a => a.Participants).OrderBy(f => f.Date).ToList();
            ViewBag.All = AllEvent;
            List<Activity> MyEvent = _context.activity.Where(c => c.Date > DateTime.Now).Include(a => a.Participants).OrderBy(f => f.Date).ToList();
            ViewBag.MyEvent = MyEvent;
            ViewBag.NewNew = _context.user.Where(p => p.UserId == user.UserId).Include(z => z.Activities).SingleOrDefault();
            return View();
        }

        [HttpGet]
        [Route("New")]
        public IActionResult New()
        {
            int? UserId = HttpContext.Session.GetInt32("LogUserId");
            if(UserId == null)
            {
                return Redirect("Index");
            }
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(Activity Event)
        {
            if(ModelState.IsValid)
            {
                int? UserId = HttpContext.Session.GetInt32("LogUserId");
                if(UserId == null)
                {
                    return Redirect("Index");
                }
                var findid = (int)HttpContext.Session.GetInt32("LogUserId");
                var thisname = _context.user.SingleOrDefault(b => b.UserId == findid).First_name;
                Activity NewActivity = new Activity
                {   
                    Title = Event.Title, 
                    Time = Event.Time, 
                    Date = Event.Date, 
                    Duration = Event.Duration,
                    Metric = Event.Metric,
                    Description = Event.Description, 
                    UserId = findid, 
                    CreaterName = thisname
                };
                _context.activity.Add(NewActivity);
                _context.SaveChanges();
                var findact = _context.activity.Where(a => a.Title == Event.Title).SingleOrDefault(b => b.Date == Event.Date);
                UserActivity Rsvp = new UserActivity
                {
                    UserId = (int)HttpContext.Session.GetInt32("LogUserId"), 
                    ActivityId = findact.ActivityId, 
                    Name = thisname,
                };
                _context.useractivity.Add(Rsvp);
                _context.SaveChanges();
                MessageBoard NewMsgB = new MessageBoard
                {
                    ActivityId = findact.ActivityId,
                };
                _context.messageboard.Add(NewMsgB);
                _context.SaveChanges();
                return Redirect("Home"); 
            }
            return View("New");

        }

        [HttpGet]
        [Route("Show/{id}")]
        public IActionResult Show(int id)
        {
            int? UserId = HttpContext.Session.GetInt32("LogUserId");
            if(UserId == null)
            {
                return Redirect("/");
            }
            ViewBag.ShowOne = _context.activity.Where(b => b.ActivityId == id).Include(a => a.Participants).SingleOrDefault();
            ViewBag.CurUser = _context.user.Where(b => b.UserId == UserId).SingleOrDefault();
            var ShowMsg = _context.messageboard.Where(b => b.ActivityId == id).Include(a => a.Chats).SingleOrDefault();
            ViewBag.MessageBoard = ShowMsg.Chats.OrderBy(x => x.ChatId);
            return View("Show");
        }
        [HttpPost]
        [Route("/Chat/{id}")]
        public IActionResult Chat(Chat Msg, int id)
        {
            User user = _context.user.Where(a => a.UserId == (int)HttpContext.Session.GetInt32("LogUserId")).SingleOrDefault();
            MessageBoard MsgB = _context.messageboard.Where(b => b.ActivityId == id).SingleOrDefault();
            Activity Act = _context.activity.Where(c => c.ActivityId == id).Include(d => d.Participants).SingleOrDefault();
            if(ModelState.IsValid)
            {
                var count = 0;
                foreach(var a in Act.Participants)
                {
                    if(a.UserId == user.UserId)
                    {
                        count++;
                        break;
                    }
                }
                if(count == 1)
                {
                    Chat Newchat = new Chat
                    {
                        UserId = user.UserId,
                        MessageBoardId = MsgB.MessageBoardId,
                        Name = user.First_name,
                        Message = Msg.Message,
                    };
                    _context.chat.Add(Newchat);
                    _context.SaveChanges();
                    return RedirectToAction("Show");
                }
                else
                {
                    return RedirectToAction("Show");
                };
            };
            return RedirectToAction("Show");
            
        }
        [Route("/Join/{id}")]
        public IActionResult Join (int id)
        {
            
            User user = _context.user.Where(b => b.UserId == (int)HttpContext.Session.GetInt32("LogUserId")).SingleOrDefault();
            UserActivity Rsvp = new UserActivity
            {
                UserId = (int)HttpContext.Session.GetInt32("LogUserId"), 
                ActivityId = id, 
                Name = user.First_name
            };
            _context.useractivity.Add(Rsvp);
            _context.SaveChanges();
            return Redirect("/Home");
        }
        [Route("/Leave/{id}")]
        public IActionResult Leave (int id)
        {
            User user = _context.user.Where(b => b.UserId == (int)HttpContext.Session.GetInt32("LogUserId")).SingleOrDefault();
            UserActivity unRsvp = _context.useractivity.Where(c => c.UserId == user.UserId).Where(c => c.ActivityId == id).SingleOrDefault();
            _context.useractivity.Remove(unRsvp);
            _context.SaveChanges();
            return Redirect("/Home");
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            User user = _context.user.Where(b => b.UserId == (int)HttpContext.Session.GetInt32("LogUserId")).SingleOrDefault();
            Activity toDelete = _context.activity.Where(d => d.ActivityId == id).SingleOrDefault();
            MessageBoard toDeleteMsg = _context.messageboard.Where(a => a.ActivityId == id).SingleOrDefault();
            if(toDelete.UserId == user.UserId)
            {
                _context.activity.Remove(toDelete);
                _context.SaveChanges();
                _context.messageboard.Remove(toDeleteMsg);
                _context.SaveChanges();
            }
            return Redirect("/Home");
        }
        public IActionResult Error()
        {
            return View();
        }         
    }
}