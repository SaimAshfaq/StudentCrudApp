using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrudApp.Controllers
{
    public class StudentController : Controller
    {
        CRUDEntities4 db = new CRUDEntities4();
        string[] AllowedExtensions = new string[] { ".jpg", ".gif", ".png" };
        // GET: Student
        public ActionResult Index()
        {
            //CRUDEntities4 getstd = new CRUDEntities4();
            //var std = db.Students.ToList();
            var std = db.Students
                      .ToList()
                      .Select(
                        s => new Student
                        {
                            StudentID = s.StudentID,
                            StudentName = s.StudentName,
                            Email = s.Email,
                            RegistrationNo = s.RegistrationNo,
                            Address = s.Address,
                            City = s.City,
                            Province = s.Province,
                            Picture = s.Picture
                        }
                     );
            //var std = getstd.Students.ToList().Select(s => new { s.StudentName, s.RegistrationNo, s.Province, s.City, s.Address, s.Email });
            return View(std);
        }

        [HttpGet]
        public ActionResult CreateStudent()
        {
            ViewBag.Province = GetProvince();
            ViewBag.City = GetCity();
            return View();
        }

        [HttpPost]
        public ActionResult CreateStudent(Student obj)
        {
            string[] ext = {".jpg",".png",".jpeg", ".gif" };

            ViewBag.Province = GetProvince();

            ViewBag.City = GetCity();

            if (obj.ImageFile != null)
            {
                string filename = Path.GetFileNameWithoutExtension(obj.ImageFile.FileName);

                string gid = Guid.NewGuid().ToString();

                string extension = Path.GetExtension(obj.ImageFile.FileName);

                var validExtention = ext.Contains(extension);

                if (!validExtention)
                {
                    ModelState.AddModelError("Picture", "Only image files are allowed. Please enter .jpg, .jpeg or .png file.");
                    return View(obj);
                }

                filename = filename + gid;

                filename = filename + extension;

                obj.Picture = "~/Images/" + filename;

                filename = Path.Combine(Server.MapPath("~/Images/"), filename);

                obj.ImageFile.SaveAs(filename);
            }

            if (ModelState.IsValid)
            {
                //db.Entry(obj).State = System.Data.Entity.EntityState.Added;

                db.Students.Add(obj);

                db.SaveChanges();

                return RedirectToAction("Index");
            }
           
            return View();
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var s = db.Students.Where(std => std.StudentID == id).ToList().FirstOrDefault();
            return View(s);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var editStd = db.Students.Where(s => s.StudentID == id).ToList().FirstOrDefault();
            //ViewBag.pName = editStd.Province.ProvinceName;
            //ViewBag.cName = editStd.City.CityName;
            ViewBag.Province = GetProvince();
            ViewBag.City = GetCity();
            //if (editStd != null)
            //{
            //    obj.Picture = editStd.Picture;
            //}
            //ViewBag.pName = GetProvinceName(editStd);
            //ViewBag.cName = GetCityName(editStd);
            return View(editStd); ;
        }

        [HttpPost]
        public ActionResult Edit(int id, Student obj)
        {
            string[] ext = { ".jpg", ".png", ".jpeg", ".gif" };

            var getStd = db.Students.Where(s => s.StudentID == id).ToList().FirstOrDefault();

            ViewBag.Province = GetProvince();

            ViewBag.City = GetCity();

            if(obj.ImageFile!=null)
            {
                string filename = Path.GetFileNameWithoutExtension(obj.ImageFile.FileName);

                string gid = Guid.NewGuid().ToString();

                string extension = Path.GetExtension(obj.ImageFile.FileName);

                var validExtention = ext.Contains(extension);

                if (!validExtention)
                {
                    ModelState.AddModelError("Picture", "Only image files are allowed. Please enter .jpg, .jpeg or .png file.");
                    return View(obj);
                }

                filename = filename + gid;

                filename = filename + extension;

                obj.Picture = "~/Images/" + filename;

                filename = Path.Combine(Server.MapPath("~/Images/"), filename);

                obj.ImageFile.SaveAs(filename);
            }

            if (ModelState.IsValid)
            {
                getStd.StudentName = obj.StudentName;

                getStd.RegistrationNo = obj.RegistrationNo;

                getStd.Email = obj.Email;

                getStd.Address = obj.Address;

                getStd.CityID = obj.CityID;

                getStd.ProvinceID = obj.ProvinceID;

                getStd.Picture = obj.Picture;

                db.Entry(getStd).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var getStd = db.Students.Where(s => s.StudentID == id).ToList().FirstOrDefault();
            return View(getStd);
        }

        [HttpPost]
        public ActionResult Delete(int id, Student obj)
        {
            var delStd = db.Students.Where(s => s.StudentID == id).ToList().FirstOrDefault();
            var imgpath = delStd.Picture;
            string path = Server.MapPath(imgpath);
            if (delStd != null)
            {
                db.Students.Remove(delStd);
                System.IO.File.Delete(path);
                //db.Entry(delStd).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public SelectList GetProvince()
        {
            //CRUDEntities4 p = new CRUDEntities4();
            var provinceList = db.Provinces.ToList();
            var provList = new SelectList(provinceList, "ProvinceID", "ProvinceName");
            return provList;
        }

        public SelectList GetCity()
        {
            //CRUDEntities4 c = new CRUDEntities4();
            var cityList = db.Cities.ToList();
            SelectList cList = new SelectList(cityList, "CityID", "CityName");
            return cList;
        }

    }
}