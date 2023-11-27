using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScheduleSample.Models;

namespace ScheduleSample.Controllers
{
    public class HomeController : Controller
    {
        ScheduleDataDataContext db = new ScheduleDataDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadData(Params param)  // Here we get the Start and End Date of current view, based on that can filter the data and return to Scheduler 
        {
            var data = db.ScheduleEventDatas.Where(app => (app.StartTime >= param.StartDate && app.StartTime <= param.EndDate) || (app.RecurrenceRule != null && app.RecurrenceRule != "")).ToList();  // Here filtering the events based on the start and end date value. 
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public class Params
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        [HttpPost]
        public JsonResult UpdateData(EditParams param)
        {
            if (param.action == "insert" || (param.action == "batch" && param.added != null)) // this block of code will execute while inserting the appointments
            {
                for (var i = 0; i < param.added.Count; i++)
                {
                    var value = (param.action == "insert") ? param.value : param.added[i];
                    int intMax = db.ScheduleEventDatas.ToList().Count > 0 ? db.ScheduleEventDatas.ToList().Max(p => p.Id) : 1;
                    DateTime startTime = Convert.ToDateTime(value.StartTime);
                    DateTime endTime = Convert.ToDateTime(value.EndTime);
                    ScheduleEventData appointment = new ScheduleEventData()
                    {
                        Id = intMax + 1,
                        StartTime = startTime,
                        EndTime = endTime,
                        Subject = value.Subject,
                        Location = value.Location,
                        Description = value.Description,
                        IsAllDay = value.IsAllDay,
                        StartTimezone = value.StartTimezone,
                        EndTimezone = value.EndTimezone,
                        RecurrenceRule = value.RecurrenceRule,
                        RecurrenceID = value.RecurrenceID,
                        RecurrenceException = value.RecurrenceException,
                        //GroupID = value.GroupID.ToString()
                    };
                    db.ScheduleEventDatas.InsertOnSubmit(appointment);
                    db.SubmitChanges();
                }
            }
            if (param.action == "update" || (param.action == "batch" && param.changed != null)) // this block of code will execute while updating the appointment
            {
                for (var i = 0; i < param.changed.Count; i++)
                {
                    var value = (param.action == "update") ? param.value : param.changed[i];
                    var filterData = db.ScheduleEventDatas.Where(c => c.Id == Convert.ToInt32(value.Id));
                    if (filterData.Count() > 0)
                    {
                        DateTime startTime = Convert.ToDateTime(value.StartTime);
                        DateTime endTime = Convert.ToDateTime(value.EndTime);
                        ScheduleEventData appointment = db.ScheduleEventDatas.Single(A => A.Id == Convert.ToInt32(value.Id));
                        appointment.StartTime = startTime;
                        appointment.EndTime = endTime;
                        appointment.StartTimezone = value.StartTimezone;
                        appointment.EndTimezone = value.EndTimezone;
                        appointment.Subject = value.Subject;
                        appointment.Location = value.Location;
                        appointment.Description = value.Description;
                        appointment.IsAllDay = value.IsAllDay;
                        appointment.RecurrenceRule = value.RecurrenceRule;
                        appointment.RecurrenceID = value.RecurrenceID;
                        appointment.RecurrenceException = value.RecurrenceException;
                        //appointment.GroupID = value.GroupID.ToString();
                    }
                    db.SubmitChanges();

                }
            }
            if (param.action == "remove" || (param.action == "batch" && param.deleted != null)) // this block of code will execute while removing the appointment
            {
                if (param.action == "remove")
                {
                    int key = Convert.ToInt32(param.key);
                    ScheduleEventData appointment = db.ScheduleEventDatas.Where(c => c.Id == key).FirstOrDefault();
                    if (appointment != null) db.ScheduleEventDatas.DeleteOnSubmit(appointment);
                }
                else
                {
                    foreach (var apps in param.deleted)
                    {
                        ScheduleEventData appointment = db.ScheduleEventDatas.Where(c => c.Id == apps.Id).FirstOrDefault();
                        if (apps != null) db.ScheduleEventDatas.DeleteOnSubmit(appointment);
                    }
                }
                db.SubmitChanges();
            }
            var data = db.ScheduleEventDatas.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public class EditParams
        {
            public string key { get; set; }
            public string action { get; set; }
            public List<ScheduleEventData> added { get; set; }
            public List<ScheduleEventData> changed { get; set; }
            public List<ScheduleEventData> deleted { get; set; }
            public ScheduleEventData value { get; set; }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}