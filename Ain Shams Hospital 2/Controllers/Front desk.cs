﻿using Ain_Shams_Hospital.Data.Entities;
using Ain_Shams_Hospital.ViewModels;
using AinShamsHospital.ViewModels;
using HospitalManagementSystem.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication10.ViewModels;

namespace Ain_Shams_Hospital.Controllers
{
    public class Front_deskController : Controller
    {
        private readonly HospitalDbContext _asu;
        public Front_deskController(HospitalDbContext asu)
        {
            _asu = asu;
        }
        public IActionResult Done()
        {
            return View();
        }
        public IActionResult message()
        {
            return View();
        }
        public IActionResult Patientmessage()
        {
            return View();
        }
      
        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Delete(delete vm,checkVM ch)
        {
            Facility_Reservation FR = new Facility_Reservation();
            var PatientName = _asu.Patients.Where(f => f.Name == vm.PatientName)
               .Select(s => s.Id).Single();
            //var RoomID = _asu.Facility_Reservations.Where(f => f.Patient_Id == PatientName)
            //.OrderByDescending(d=>d);
            var h = _asu.Facility_Reservations.Where(f => f.Patient_Id == PatientName)
                .Select(s=>new Facility_Reservation{ Id=s.Id ,Staff_Id=s.Staff_Id})
                .OrderByDescending(s=>s.Id)
             .FirstOrDefault();
            String ROOMName = HttpContext.Session.GetString("Roomname");
            String start = HttpContext.Session.GetString("START");
            String end = HttpContext.Session.GetString("END");
            var HID = _asu.Hospital_Facilities.Where(f => f.Type == ROOMName).Select(s => s.Id).Single();
            FR.Start_Hour = start;  //ob.From;
            FR.End_Hour = end;
            FR.Hospital_Facility_Id = HID;
            //FR.Hospital_Facility_Id =no ;
            FR.Patient_Id = PatientName;
            FR.Staff_Id = h.Staff_Id;
            _asu.Add(FR);
            _asu.SaveChanges();
            int x = h.Id;
            var model = _asu.Facility_Reservations.Find(x);
            _asu.Remove(model);
            _asu.SaveChanges();
            ViewBag.UserMessage3 = "Deleted successfully";

            return View();
        }
        [HttpGet]
        public IActionResult Event()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Event(mainVM m)
        {
            Facility_Reservation FR = new Facility_Reservation();
            Hospital_Facility H = new Hospital_Facility();
            var ID = _asu.Hospital_Facilities.Where(f => f.Type == m.Room_Id).Select(s => s.Id).Single();
            var h = _asu.Facility_Reservations.Include(p=>p.Patient).Where(f => f.Hospital_Facility_Id ==ID )
                .OrderByDescending(s => s.End_Hour).ToList();

            ViewBag.Roomnumber = m.Room_Id;
            ViewBag.D1 = h;
           
   

            return View();
        }
        [HttpGet]
       
        public IActionResult main()
        {
            var NameRexist = _asu.Hospital_Facilities.Where(f => f.Type.Substring(0,4)=="Room")
                .Select(s=>new Hospital_Facility { Type = s.Type })
                . ToList();
            ViewBag.h = NameRexist;
            return View();
        }
        [HttpGet]
        public IActionResult checkavalabilty()
        {
           
            return View();
        }
        [HttpPost]
            public IActionResult checkavalabilty(checkVM ch)
        {
            var NameRexist = _asu.Hospital_Facilities.Where(f => f.Type.Substring(0, 4) == "Room")
                   .Select(s => new Hospital_Facility { Type = s.Type })
                   .ToList();
            var NameSurgeryRexist = _asu.Hospital_Facilities.Where(f => f.Type.Substring(0, 6) == "Surgery")
                  .Select(s => new Hospital_Facility { Type = s.Type })
                  .ToList();

            int y=0; //flag 
            
            foreach (var x in NameRexist)
            {


                var HID = _asu.Hospital_Facilities.Where(f => f.Type == x.Type).Select(s => s.Id).Single();
                var availableroom = _asu.Facility_Reservations.Where(f => f.Hospital_Facility_Id == HID)
                    .Select(s => new { date = s.End_Hour, date1 = s.Start_Hour }).ToList();
                
                foreach (var V in availableroom)
                {
                    DateTime parse1 = DateTime.Parse(ch.From);
                    DateTime parse2 = DateTime.Parse(ch.To);
                    DateTime parse3 = DateTime.Parse(V.date);
                    DateTime parse4 = DateTime.Parse(V.date1);
                    if ((parse1 > parse3) || (parse2 < parse4))
                    {
                        y=1;
                       
                    }
                    else 
                    {
                        y=2;
                        break;
                        
                    }
                }
                if (y==1) 
                {
                    // TempData["roomname"] = _asu.Hospital_Facilities.Where(f => f.Id == HID).Select(s => s.Type).Single();
                    var roomname = _asu.Hospital_Facilities.Where(f => f.Id == HID).Select(s => s.Type).Single();
                    ViewBag.Roomname = roomname;
                    HttpContext.Session.SetString("Roomname",roomname );
                    HttpContext.Session.SetString("START", ch.From);
                    HttpContext.Session.SetString("END", ch.To);
                    return View();
                }
            }
           
                ViewBag.UserMessage = "this room is not available";
                return View();
        }

        public IActionResult Roomavailabilty()
        {
             
            return View();
        }
        [HttpGet]
        public IActionResult Transfer(delete ch)
        {
            return View(); 
        }
        [HttpPost]
        /*public IActionResult Transfer(delete ch)
        { 
            return View();
        }*/
        [HttpGet]
        public IActionResult Roomreservation(checkVM ch)
        {
            String ROOMName= HttpContext.Session.GetString("Roomname");
            String start=HttpContext.Session.GetString("START");
           String end= HttpContext.Session.GetString("END");
            //var ROOMNAME = TempData["roomname"];

            //TempData["ROOMNAME"] = TempData["roomname"].ToString();
            // TempData[" HID"] = _asu.Hospital_Facilities.Where(f => f.Type ==( TempData["roomname"].ToString()) ).Select(s => s.Id).Single();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Roomreservation(roomVM ob , checkVM ch)
        {
            String ROOMName = HttpContext.Session.GetString("Roomname");
            String start = HttpContext.Session.GetString("START");
          String end= HttpContext.Session.GetString("END");
            Facility_Reservation FR = new Facility_Reservation();
            var NamePexist = _asu.Patients.ToList().Any(f => f.Name == ob.PatientName);
            var NameSexist = _asu.Staff.ToList().Any(F => F.Name == ob.DoctorName);
            // var ROOMNAME = TempData["roomname"];
           
            var HID = _asu.Hospital_Facilities.Where(f => f.Type == ROOMName).Select(s => s.Id).Single();
            
            if (NamePexist)
            {
                TempData["Patient_Id"] = _asu.Patients.Where(f => f.Name == ob.PatientName).Select(s => s.Id).Single();
                if (NameSexist)
                {
                    //string start = TempData["Start"].ToString();
                    //string end = TempData["end"].ToString();
                    //int no = (int)TempData["HID"];
                    TempData["Staff_Id"] = _asu.Staff.Where(f => f.Name == ob.DoctorName).Select(s => s.Id).Single();
                    
                    FR.Start_Hour = start;  //ob.From;
                    FR.End_Hour = end;
                    FR.Hospital_Facility_Id = HID;
                    //FR.Hospital_Facility_Id =no ;
                    FR.Patient_Id = (int)TempData["Patient_Id"];
                    FR.Staff_Id = (int)TempData["Staff_Id"];
                    _asu.Add(FR);
                    _asu.SaveChanges();
                    ViewBag.UserMessage2 = "Done";
                    return View();
                }
                else
                {
                    ViewBag.UserMessage1 = "This doctor is not in our hospital";
                    return View();
                }
            }
            else
            {
                return Redirect("/Front_desk/PatientMESSAGE");
            }
            
        }

            /* public IActionResult Roomreservation(RoomReservationVM ob)
             {
                 Hospital_Facility H = new Hospital_Facility();
                 Facility_Reservation FR = new Facility_Reservation();
                 //Patient p = new Patient();
                 //var x = H.Available;

                 var NamePexist = _asu.Patients.ToList().Any(f => f.Name == ob.PatientName);
                 TempData["Staff_Id"] = _asu.Staff.Where(f => f.Name == ob.DoctorName).Select(s => s.Id).Single();
                 var NameSexist = _asu.Staff.ToList().Any(F => F.Name == ob.DoctorName);
                 var Availabilty = _asu.Hospital_Facilities.Where(f => f.Type == ob.Room).Select(s => s.Available).Single();
                 var HID = _asu.Hospital_Facilities.Where(f => f.Type == ob.Room).Select(s => s.Id).Single();
                 var availableroom = _asu.Facility_Reservations.Where(f => f.Hospital_Facility_Id == HID)
                     .Select(s => new { date = s.End_Hour, date1 = s.Start_Hour }).ToList();
                 bool av; //flag

                 var ROOMNAME = TempData["roomname"];
                 if (availableroom == null)
                 {
                     if (NamePexist)
                     {
                         TempData["Patient_Id"] = _asu.Patients.Where(f => f.Name == ob.PatientName).Select(s => s.Id).Single();
                         if (NameSexist)
                         {
                             TempData["Staff_Id"] = _asu.Staff.Where(f => f.Name == ob.DoctorName).Select(s => s.Id).Single();
                             FR.Start_Hour = ob.From;
                             FR.End_Hour = ob.To;
                             FR.Hospital_Facility_Id = HID;
                             FR.Patient_Id = (int)TempData["Patient_Id"];
                             FR.Staff_Id = (int)TempData["Staff_Id"];
                             _asu.Add(FR);
                             _asu.SaveChanges();

                             return Redirect("/Front_desk/SRoomreservation");
                         }
                         else
                         {
                             return Redirect("/Front_desk/MESSAGE");
                         }
                     }
                     else
                     {
                         return Redirect("/Front_desk/PatientMESSAGE");
                     }
                 }
                 else
                 {

                     foreach (var V in availableroom)
                     {
                         DateTime parse1 = DateTime.Parse(ob.From);
                         DateTime parse2 = DateTime.Parse(ob.To);
                         DateTime parse3 = DateTime.Parse(V.date);
                         DateTime parse4 = DateTime.Parse(V.date1);
                         if ((parse1 > parse3) || (parse2 < parse4))
                         {
                             av = true;
                         }
                         else
                         {
                             av = false;
                             ViewBag.UserMessage = "This room is Not available";
                             return View();
                             //return Redirect("/Front_desk/Roomavailabilty");
                         }
                     }
                     if (av = true)
                     {
                         if (NamePexist)
                         {
                             TempData["Patient_Id"] = _asu.Patients.Where(f => f.Name == ob.PatientName).Select(s => s.Id).Single();
                             if (NameSexist)
                             {
                                 TempData["Staff_Id"] = _asu.Staff.Where(f => f.Name == ob.DoctorName).Select(s => s.Id).Single();
                                 FR.Start_Hour = ob.From;
                                 FR.End_Hour = ob.To;
                                 FR.Hospital_Facility_Id = HID;
                                 FR.Patient_Id = (int)TempData["Patient_Id"];
                                 FR.Staff_Id = (int)TempData["Staff_Id"];
                                 _asu.Add(FR);
                                 _asu.SaveChanges();

                                 return Redirect("/Front_desk/SRoomreservation");
                             }
                             else
                             {
                                 return Redirect("/Front_desk/MESSAGE");
                             }
                         }
                         else
                         {
                             return Redirect("/Front_desk/PatientMESSAGE");
                         }
                     }
                     else
                     {

                         return Redirect("/Front_desk/Roomavailabilty");
                     }
                 }

             }*/
            public IActionResult NotAvailable()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(SearchVm ob)
        {
            Facility_Reservation FR = new Facility_Reservation();
            var ID = _asu.Patients.Where(f => f.Name == ob.patientName).Select(s => s.Id).Single();
            var NamePexist = _asu.Facility_Reservations.ToList().Any(f => f.Patient_Id == ID);
            if (NamePexist)
            {

                /*
                                var Startavailable = _asu.Facility_Reservations.Where(f => f.Patient_Id == ID)
                                    .Select(s => s.Start_Hour).Single();
                                var Endavailable = _asu.Facility_Reservations.Where(f => f.Patient_Id == ID)
                                  .Select(s => s.End_Hour).Single();
               
                var Endavailable = _asu.Facility_Reservations.Where(f => f.Patient_Id == ID)
                                .Select(s =>new { s.End_Hour ,s.Start_Hour}).ToList();
                DateTime parse1 = DateTime.Parse(Startavailable);
                DateTime parse2 = DateTime.Parse(Endavailable);
 */
                var Endavailable = _asu.Facility_Reservations.Where(f => f.Patient_Id == ID)
                                          .Select(s => new { dates = s.Start_Hour, dateE = s.End_Hour, name = s.Id })
                                          .OrderByDescending(w => w.dateE).FirstOrDefault();
                                          //.ToList();
                //foreach (var V in Endavailable)
                //{
                    DateTime parse3 = DateTime.Parse(Endavailable.dates);
                    DateTime parse4 = DateTime.Parse(Endavailable.dateE);
                    //int i = V.name;
                    if ((parse3<= ob.Today && ob.Today <= parse4))
                    {
                        var roomnumber = _asu.Facility_Reservations
                        .Where(F => F.Id==Endavailable.name )
                        .Select(s => s.Hospital_Facility_Id).Single();
                        TempData["room"] = _asu.Hospital_Facilities.Where(t => t.Id == roomnumber)
                            .Select(S => S.Type).Single();
                        return Redirect("/Front_desk/Searchresult");
                    }
                    else {
                        return Redirect("/Front_desk/NotAvailable");
                    }
                //}
            }
            return Redirect("/Front_desk/NotAvailable");
        }
        public IActionResult SearchResult()
        {
            ViewBag.D = TempData["Endavailable"];
            ViewBag.DD = TempData["room"];
            return View();
        }
        [HttpGet]
        public IActionResult SRoomreservation()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SRoomreservation(SRoomReservationVM ob)
        {
            if (ob.SurgeryRoom == "No Surgery")
            {
                return Redirect("/Front_desk/done");
            }
            else
            {
                Facility_Reservation F = new Facility_Reservation();
                var SHID = _asu.Hospital_Facilities.Where(f => f.Type == ob.SurgeryRoom).Select(s => s.Id).Single();
                var Savailableroom = _asu.Facility_Reservations.Where(f => f.Hospital_Facility_Id == SHID)
                    .Select(s => new { date = s.End_Hour, date1 = s.Start_Hour }).ToList();
                bool av;
                F.Patient_Id = (int)TempData["Patient_Id"];
                F.Staff_Id = (int)TempData["Staff_Id"];
                if (Savailableroom == null)
                {
                    F.Start_Hour = ob.Start_Hour;
                    F.End_Hour = ob.End_Hour;
                    F.Hospital_Facility_Id = SHID;

                    _asu.Add(F);
                    _asu.SaveChanges();

                    return Redirect("/Front_desk/done");
                }
                else
                {

                    foreach (var V in Savailableroom)
                    {
                        DateTime parse1 = DateTime.Parse(ob.Start_Hour);
                        DateTime parse2 = DateTime.Parse(ob.End_Hour);
                        DateTime parse3 = DateTime.Parse(V.date);
                        DateTime parse4 = DateTime.Parse(V.date1);
                        if ((parse1 > parse3) || (parse2 < parse3))
                        {
                            av = true;
                        }
                        else
                        {
                            av = false;
                            return Redirect("/Front_desk/Roomavailabilty");
                        }

                    }
                    if (av = true)
                    {
                        F.Start_Hour = ob.Start_Hour;
                        F.End_Hour = ob.End_Hour;
                        F.Hospital_Facility_Id = SHID;
                        _asu.Add(F);
                        _asu.SaveChanges();
                        return Redirect("/Front_desk/done");
                    }
                }
            }
                return View();
        }
    }
}


/*
else
{
    DateTime parse3 = DateTime.Parse(startavailableroom);

    DateTime parse1 = DateTime.Parse(ob.From);
    DateTime parse2 = DateTime.Parse(ob.To);
    if ((parse1 > parse3) || (parse2 < parse3))
    //if (Availabilty == true)
    {
        if (NamePexist)
        {
            if (NameSexist)
            {
                FR.Start_Hour = ob.From;
                FR.End_Hour = ob.To;
                FR.Hospital_Facility_Id = HID;
                FR.Patient_Id = PID;
                FR.Staff_Id = SID;
                _asu.Add(FR);
                _asu.SaveChanges();

                return Redirect("/Front_desk/done");
            }
            else
            {
                return Redirect("/Front_desk/MESSAGE");
            }
        }
        else
        {
            return Redirect("/Front_desk/PatientMESSAGE");
        }
    }
    else
    {
        return Redirect("/Front_desk/Roomavailabilty");
    }
}
*/




