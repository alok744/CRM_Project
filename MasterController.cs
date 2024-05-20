using Loginpage_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Loginpage_project.Controllers
{
    public class MasterController : Controller
    {
        private readonly myloginpage_Dbcontext DB;

        public MasterController(myloginpage_Dbcontext db)
        {
            DB = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult login_page(Loginpage uc)
        {
            DB.Add(uc);
            DB.SaveChanges();
            ViewBag.message = "The user " + uc.Name + " is saved successfully";
            return View();
        }


        public IActionResult m_country_list()
        {
            List<m_country> m_country_list = TempData["m_country_list"] != null ? JsonConvert.DeserializeObject<List<m_country>>(TempData["m_country_list"] as string) : DB.m_country.Where(x => x.del_status == false).OrderByDescending(x => x.created_date).Take(10).ToList();
            ViewBag.m_country_list = m_country_list;

            ViewBag.search_item_count = TempData["search_item_count"] != null ? TempData["search_item_count"] : "10";
            ViewBag.item_count = ViewBag.search_item_count;

            ViewBag.search_item_page = TempData["search_item_page"] != null ? TempData["search_item_page"] : "1";

            //ViewBag.page_number = (10 - 1) * 10 + 1;
            ViewBag.page_number = ViewBag.search_item_page;

            ViewBag.page_count = TempData["page_count"] != null ? TempData["page_count"] : ((DB.m_country.Where(x => x.del_status == false).Count() / int.Parse(ViewBag.search_item_count)) + ((DB.m_country.Where(x => x.del_status == false).Count() % int.Parse(ViewBag.search_item_count)) == 0 ? 0 : 1)).ToString();

            //ViewBag.page_count = TempData["page_count"] != null ? TempData["page_count"] : ((DB.m_country.Where(x => x.del_status == false).Count() / int.Parse(ViewBag.search_item_count)) + ((DB.m_country.Where(x => x.del_status == false).Count() % int.Parse(ViewBag.search_item_count)) == 0 ? 0 : 1)).ToString();

            ViewBag.total_m_country = TempData["total_m_country"] != null ? TempData["total_m_country"] : DB.m_country.Where(x => x.del_status == false).Count().ToString();

            ViewBag.search_items_filter = TempData["search_items_filter"] != null ? TempData["search_items_filter"] : "";
            ViewBag.search_item_text = TempData["search_item_text"] != null ? TempData["search_item_text"] : "";
            //ViewBag.country_list = DB.m_country.Where(x => x.del_status == false).ToList();
            //m_country_list.Reverse();
            //ViewBag.m_country_list = m_country_list;

            ViewBag.error = TempData["error"];
            ViewBag.message = TempData["message"];

            return View();
        }

        public JsonResult master_country_list()
        {
            List<m_country> m_country_list = DB.m_country.Where(x => x.del_status == false).OrderByDescending(x => x.created_date).Take(10).ToList();

            return Json(new { success = true, value = "True", country_list = m_country_list });
        }
        /// <summary>
        /// country name search, count, filter, text, page..........
        /// </summary>
        /// 

        //public List<m_state> Get_state_List_update(int country_id)
        //{
        //    try
        //    {
        //        var state_list = DB.m_state.Where(x => x.country_id == country_id).ToList();
        //        ViewBag.state_list = state_list;
        //        return state_list;
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //}


        public IActionResult View_Country(int id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }
            else
            {
                var country_data = DB.m_country.Find(id);
                return Json(new { success = true, value = "True", country_name = country_data.country_name });
            }

        }


        public IActionResult search_m_country(string search_item_count, string search_items_filter, string search_item_text, string search_item_page)
        {
            search_item_page = int.Parse(search_item_page) < 1 ? "1" : search_item_page;
            string page_count = null;
            int total_m_country = 0;

            List<m_country> M_Country_List = new List<m_country>();
            switch (search_items_filter)
            {
                case "country_name":
                    if (search_item_text == null)
                    {
                        return RedirectToAction("m_country_list", "Master");
                    }
                    else
                    {

                        M_Country_List = DB.m_country.Where(x => x.country_name.Contains(search_item_text.ToLower()) && x.del_status == false).
                        Skip((int.Parse(search_item_page) - 1) * int.Parse(search_item_count)).Take(int.Parse(search_item_count)).ToList();
                        total_m_country = DB.m_country.Where(x => x.country_name.Contains(search_item_text.ToLower()) && x.del_status == false).Count();
                        page_count = (total_m_country / int.Parse(search_item_count) + ((total_m_country % int.Parse(search_item_count)) == 0 ? 0 : 1)).ToString();

                    }
                    break;


                default:
                    M_Country_List = DB.m_country.Where(x => x.del_status == false).Skip((int.Parse(search_item_page) - 1) * int.Parse(search_item_count)).Take(int.Parse(search_item_count)).ToList();
                    total_m_country = DB.m_country.Where(x => x.del_status == false).Count();
                    page_count = (total_m_country / int.Parse(search_item_count) + ((total_m_country % int.Parse(search_item_count)) == 0 ? 0 : 1)).ToString();
                    break;
            }

            TempData["m_country_list"] = JsonConvert.SerializeObject(M_Country_List.Where(x => x.del_status == false).OrderByDescending(x => x.created_date));
            TempData["search_item_count"] = search_item_count;
            TempData["search_items_filter"] = search_items_filter;
            TempData["search_item_text"] = search_item_text;
            TempData["search_item_page"] = search_item_page;
            TempData["page_count"] = page_count;
            TempData["total_m_country"] = total_m_country.ToString();
            return RedirectToAction("m_country_list", "Master");
        }

        /// <summary>
        /// add country....
        /// </summary>



        [HttpGet]
        public IActionResult add_m_country()
        {
            return View();
        }


        [HttpPost]
        public IActionResult add_m_country(m_country add_obj)
        {
            string error = null;
            string message = null;
            try
            {

                if (DB.m_country.Where(x => x.country_id != add_obj.country_id).Any(x => x.country_name == add_obj.country_name.Trim() && x.del_status == false))
                {
                    error = "This name already exists.";
                }
                else
                {
                    DB.m_country.Add(new m_country
                    {
                        country_name = add_obj.country_name,
                        created_date = DateTime.Now,
                        created_by = "1",
                        del_status = Convert.ToBoolean(0)
                    });
                    DB.SaveChanges();

                    return Json(new { success = true, value = "True" });

                }

            }
            catch (Exception ex)
            {
                //error = "Unable to add country detail to database. " + ex.ToString();
                return Json(new { success = false, value = "invalid_id" + ex.ToString() });
            }
            //ModelState.Clear();
            //ViewBag.message = message;
            //ViewBag.error = error;
            return RedirectToAction("m_country_list", "Master");
        }

        //[HttpPost]
        //public IActionResult add_m_country(m_country add_obj)
        //{
        //    string error = null;
        //    string message = null;
        //    try
        //    {
        //        if (DB.m_country.Where(x => x.country_id != add_obj.country_id).Any(x => x.country_name == add_obj.country_name.Trim() && x.del_status == false))
        //        {
        //            error = "This name already exists.";
        //        }
        //        else
        //        {
        //            DB.m_country.Add(new m_country
        //            {
        //                country_name = add_obj.country_name,
        //                created_date = DateTime.Now.Date,
        //                created_by = "1",
        //                del_status = Convert.ToBoolean(0)
        //            });
        //            DB.SaveChanges();

        //            message = "Country detail added to database successfully.";
        //            //ViewBag.SweetAlertScript = "Swal.fire" +
        //            //    "({ position: 'middle', " +
        //            //    "icon: 'success', " +
        //            //    "title: 'Data Updated Successfully', " +
        //            //    "showConfirmButton: false, " +
        //            //    "timer: 1500, " +
        //            //    "showCloseButton: false });";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        error = "Unable to add country detail to database. " + ex.ToString();
        //    }
        //    ModelState.Clear();
        //    ViewBag.message = message;
        //    ViewBag.error = error;
        //    return RedirectToAction("m_country_list", "Master");
        //}

        // [HttpGet]
        public JsonResult edit_m_country(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }
            else
            {
                var get_m_country_detail = DB.m_country.Find(id);
                return Json(new { success = true, value = "True", country_id = get_m_country_detail.country_id, country_name = get_m_country_detail.country_name });
            }
        }

        [HttpPost]
        public JsonResult edit_m_country(int country_id, string country_name)
        {
            //string error = null;
            //string message = null;
            var country_data = DB.m_country.Where(x => x.del_status == false && x.country_id == country_id).FirstOrDefault();
            try
            {
                country_data.country_name = country_name;
                country_data.del_status = Convert.ToBoolean(0);
                country_data.created_date = DateTime.Now;
                country_data.created_by = "1";
                DB.SaveChanges();
                return Json(new { success = true, value = "True" });
            }
            catch (Exception ex)
            {
                //error = "Unable to update country detail to database. " + ex.ToString();
                return Json(new { success = false, value = "invalid_id" + ex.ToString() });
                //return RedirectToAction("m_state_list", "Master");
            }
            //ViewBag.message = message;
            //ViewBag.error = error;
            //return RedirectToAction("m_country_list", "Master");

        }

        //[HttpPost]
        //public IActionResult edit_m_country(m_country edit_obj)
        //{
        //    string error = null;
        //    string message = null;

        //    try
        //    {
        //        if (DB.m_country.Where(x => x.country_id != edit_obj.country_id).Any(x => x.country_name == edit_obj.country_name.Trim() && x.del_status == false))
        //        {
        //            error = "This name already exists.";
        //        }
        //        else
        //        {
        //            edit_obj.del_status = Convert.ToBoolean(0);
        //            edit_obj.created_date = DateTime.Now.Date;
        //            edit_obj.created_by = "1";

        //            DB.Update(edit_obj);

        //            DB.SaveChanges();
        //            message = "Country detail updated to database sucessfully.";
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        error = "Unable to update country detail to database. " + ex.ToString();
        //    }
        //    ViewBag.message = message;
        //    ViewBag.error = error;
        //    return RedirectToAction("m_country_list", "Master");
        //    //return View(edit_obj);
        //}

        public IActionResult delete_m_country(int? country_id)
        {
            try
            {
                if (country_id == null)
                {
                    return RedirectToAction("m_country_list", "Master");
                }
                else
                {
                    var m_country_list = DB.m_country.Where(x => x.country_id == country_id).FirstOrDefault();
                    m_country_list.del_status = Convert.ToBoolean(1);
                    DB.SaveChanges();
                    TempData["message"] = "Your Data Is Deleted Successfully";
                    return Json(new { success = true, value = "True" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, value = "invalid_id" + ex.ToString() });
            }

            //return RedirectToAction("m_country_list");
        }



        public IActionResult m_state_list()
        {
            List<m_state> m_state_list = TempData["m_state_list"] != null ? JsonConvert.DeserializeObject<List<m_state>>(TempData["m_state_list"] as string) : DB.m_state.Where(x => x.del_status == false).OrderByDescending(x => x.created_date).Take(10).ToList();
            ViewBag.m_state_list = m_state_list;

            ViewBag.search_item_count = TempData["search_item_count"] != null ? TempData["search_item_count"] : "10";
            ViewBag.item_count = ViewBag.search_item_count;

            ViewBag.search_item_page = TempData["search_item_page"] != null ? TempData["search_item_page"] : "1";
            ViewBag.page_number = ViewBag.search_item_page;

            ViewBag.page_count = TempData["page_count"] != null ? TempData["page_count"] : ((DB.m_state.Where(x => x.del_status == false).Count() / int.Parse(ViewBag.search_item_count)) + ((DB.m_state.Where(x => x.del_status == false).Count() % int.Parse(ViewBag.search_item_count)) == 0 ? 0 : 1)).ToString();

            ViewBag.total_m_state = TempData["total_m_state"] != null ? TempData["total_m_state"] : DB.m_state.Where(x => x.del_status == false).Count().ToString();

            ViewBag.search_items_filter = TempData["search_items_filter"] != null ? TempData["search_items_filter"] : "";
            ViewBag.search_item_text = TempData["search_item_text"] != null ? TempData["search_item_text"] : "";
            ViewBag.country_list = DB.m_country.Where(x => x.del_status == false).ToList();

            var country_list = (from country in DB.m_country.Where(x => x.del_status == false).OrderBy(x => x.country_name)
                                select new SelectListItem()
                                {
                                    Text = country.country_name,
                                    Value = country.country_id.ToString(),
                                }).ToList();

            country_list.Insert(0, new SelectListItem()
            {
                Text = "--Select--",
                Value = ""
            });

            ViewBag.listofcountry = new SelectList(country_list, "Value", "Text");
            ViewBag.listofcountry = country_list;
            ViewBag.error = TempData["error"];
            ViewBag.message = TempData["message"];
            //ViewBag.country_list = country_list;
            return View();
        }
        //public JsonResult master_state_list()
        //{
        //    List<m_state> m_state_list = DB.m_state.Where(x => x.del_status == false).OrderByDescending(x => x.created_date).Take(10).ToList();
        //    return Json(new { success = true, value = "True", state_list = m_state_list});
        //}

        public JsonResult master_state_list()
        {
            var m_state_list = DB.m_state.Where(x => x.del_status == false).OrderByDescending(x => x.created_date).Take(10)
                .Select(x => new
                {
                    x.state_id,
                    x.state_name,
                    x.country_id,
                    country_name = DB.m_country.FirstOrDefault(c => c.country_id == x.country_id).country_name // Assuming m_country is your country table and it has country_name column
                })
                .ToList();

            return Json(new { success = true, value = "True", state_list = m_state_list });
        }



        public IActionResult search_m_state(string search_item_count, string search_items_filter, string search_item_text, string search_item_page)
        {

            search_item_page = int.Parse(search_item_page) < 1 ? "1" : search_item_page;
            string page_count = null;
            int total_m_state = 0;

            List<m_state> M_State_List = new List<m_state>();
            List<m_country> M_Country_List = new List<m_country>();
            switch (search_items_filter)
            {
                case "country_name":
                    if (search_item_text == null)
                    {
                        return RedirectToAction("m_state_list", "Master");
                    }
                    else
                    {
                        M_Country_List = DB.m_country.Where(x => x.country_name == search_item_text).ToList();
                        M_State_List = DB.m_state.Where(x => x.country_id == M_Country_List[0].country_id && x.del_status == false).
                        Skip((int.Parse(search_item_page) - 1) * int.Parse(search_item_count)).Take(int.Parse(search_item_count)).ToList();
                        total_m_state = DB.m_state.Where(x => x.country_id == M_Country_List[0].country_id && x.del_status == false).Count();
                        page_count = (total_m_state / int.Parse(search_item_count) + ((total_m_state % int.Parse(search_item_count)) == 0 ? 0 : 1)).ToString();
                    }
                    break;

                case "state_name":
                    if (search_item_text == null)
                    {
                        return RedirectToAction("m_state_list", "Master");
                    }
                    else
                    {
                        M_State_List = DB.m_state.Where(x => x.state_name.Contains(search_item_text.ToLower()) && x.del_status == false).
                        Skip((int.Parse(search_item_page) - 1) * int.Parse(search_item_count)).Take(int.Parse(search_item_count)).ToList();
                        total_m_state = DB.m_state.Where(x => x.state_name.Contains(search_item_text.ToLower()) && x.del_status == false).Count();
                        page_count = (total_m_state / int.Parse(search_item_count) + ((total_m_state % int.Parse(search_item_count)) == 0 ? 0 : 1)).ToString();
                    }
                    break;

                default:
                    M_State_List = DB.m_state.Where(x => x.del_status == false).Skip((int.Parse(search_item_page) - 1) * int.Parse(search_item_count)).Take(int.Parse(search_item_count)).ToList();
                    total_m_state = DB.m_state.Where(x => x.del_status == false).Count();
                    page_count = (total_m_state / int.Parse(search_item_count) + ((total_m_state % int.Parse(search_item_count)) == 0 ? 0 : 1)).ToString();
                    break;
            }

            TempData["m_state_list"] = JsonConvert.SerializeObject(M_State_List.Where(x => x.del_status == false).OrderByDescending(x => x.created_date));
            TempData["search_item_count"] = search_item_count;
            TempData["search_items_filter"] = search_items_filter;
            TempData["search_item_text"] = search_item_text;
            TempData["search_item_page"] = search_item_page;
            TempData["page_count"] = page_count;
            TempData["total_m_state"] = total_m_state.ToString();
            return RedirectToAction("m_state_list");
        }

        /// <summary>
        /// add state....
        /// </summary>

        [HttpGet]
        public IActionResult add_m_state()
        {
            var country_list = (from country in DB.m_country.Where(x => x.del_status == false).OrderBy(x => x.country_name)
                                select new SelectListItem()
                                {
                                    Text = country.country_name,
                                    Value = country.country_id.ToString(),
                                }).ToList();

            country_list.Insert(0, new SelectListItem()
            {
                Text = "--Select--",
                Value = ""
            });

            ViewBag.listofcountry = new SelectList(country_list, "Value", "Text");
            return RedirectToAction("m_state_list", "Master");
        }

        [HttpPost]
        public IActionResult add_m_state(m_state add_obj)
        {
            string error = null;
            string message = null;
            try
            {
                if (DB.m_state.Where(x => x.state_id != add_obj.state_id).Any(x => x.state_name == add_obj.state_name.Trim() && x.del_status == false))
                {
                    error = "This name already exists.";
                }
                else
                {
                    DB.m_state.Add(new m_state
                    {
                        country_id = add_obj.country_id,
                        state_name = add_obj.state_name,
                        created_date = DateTime.Now,
                        created_by = "1",
                        del_status = Convert.ToBoolean(0),

                    });
                    DB.SaveChanges();
                    return Json(new { success = true, value = "True" });
                    //message = "State detail added to database successfully.";
                }

            }
            catch (Exception ex)
            {
                //error = "Unable to add state detail to database. " + ex.ToString();
                return Json(new { success = false, value = "invalid_id" + ex.ToString() });

            }
            ModelState.Clear();
            ViewBag.message = message;
            ViewBag.error = error;
            var country_list = (from country in DB.m_country.Where(x => x.del_status == false).OrderBy(x => x.country_name)
                                select new SelectListItem()
                                {
                                    Text = country.country_name,
                                    Value = country.country_id.ToString(),
                                }).ToList();

            country_list.Insert(0, new SelectListItem()
            {
                Text = "--Select--",
                Value = ""
            });

            ViewBag.listofcountry = new SelectList(country_list, "Value", "Text");
            return RedirectToAction("m_state_list", "Master");
        }

        // [HttpGet]

        public IActionResult View_State(int id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }
            else
            {
                var get_m_state_detail = DB.m_state.Find(id);
                var country_list = DB.m_country.Where(x => x.del_status == false && x.country_id == get_m_state_detail.country_id).FirstOrDefault();

                return Json(new { success = true, value = "True", state_id = get_m_state_detail.state_id, state_name = get_m_state_detail.state_name, country_list = country_list });
            }

        }





        public JsonResult edit_m_state(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }
            else
            {
                var get_m_state_detail = DB.m_state.Find(id);
                var country_list = DB.m_country.Where(x => x.del_status == false && x.country_id == get_m_state_detail.country_id).FirstOrDefault();

                return Json(new { success = true, value = "True", state_id = get_m_state_detail.state_id, state_name = get_m_state_detail.state_name, country_list = country_list });
            }

        }

        [HttpPost]
        public IActionResult edit_m_state(m_state edit_obj)
        {
            string error = null;
            string message = null;
            try
            {
                if (DB.m_state.Where(x => x.state_id != edit_obj.state_id).Any(x => x.state_name == edit_obj.state_name.Trim() && x.del_status == false))
                {
                    error = "This name already exists.";

                }
                else
                {
                    edit_obj.del_status = Convert.ToBoolean(0);
                    edit_obj.created_date = DateTime.Now;
                    edit_obj.created_by = "1";


                    DB.Update(edit_obj);

                    DB.SaveChanges();
                    //message = "state detail updated to database successfully.";
                    return Json(new { success = true, value = "True" });

                }


            }
            catch (Exception ex)
            {
                //error = "Unable to update state detail to database. " + ex.ToString();
                return Json(new { success = false, value = "invalid_id" + ex.ToString() });
            }

            var country_list = (from country in DB.m_country.Where(x => x.del_status == false).OrderBy(x => x.country_name)
                                select new SelectListItem()
                                {
                                    Text = country.country_name,
                                    Value = country.country_id.ToString(),
                                }).ToList();
            country_list.Insert(0, new SelectListItem()
            {
                Text = "--Select--",
                Value = ""
            });

            ViewBag.listofcountry = new SelectList(country_list, "Value", "Text");
            ViewBag.message = message;
            ViewBag.error = error;
            return RedirectToAction("m_state_list", "Master");

        }

        public IActionResult delete_m_state(int? state_id)
        {
            try
            {
                if (state_id == null)
                {
                    return RedirectToAction("m_state_list", "Master");
                }
                else
                {
                    var m_state_list = DB.m_state.Where(x => x.state_id == state_id).FirstOrDefault();
                    m_state_list.del_status = Convert.ToBoolean(1);
                    DB.SaveChanges();
                    TempData["message"] = "Your Data Is Deleted Successfully";
                    //return RedirectToAction("m_state_list", "Master");
                    return Json(new { success = true, value = "True" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, value = "invalid_id" + ex.ToString() });
            }

        }


        public IActionResult m_city_list()
        {
            List<m_city> m_city_list = TempData["m_city_list"] != null ? JsonConvert.DeserializeObject<List<m_city>>(TempData["m_city_list"] as string) : DB.m_city.Where(x => x.del_status == false).OrderByDescending(x => x.created_date).Take(10).ToList();
            ViewBag.m_city_list = m_city_list;

            ViewBag.search_item_count = TempData["search_item_count"] != null ? TempData["search_item_count"] : "10";
            ViewBag.item_count = ViewBag.search_item_count;

            ViewBag.search_item_page = TempData["search_item_page"] != null ? TempData["search_item_page"] : "1";
            ViewBag.page_number = ViewBag.search_item_page;

            ViewBag.page_count = TempData["page_count"] != null ? TempData["page_count"] : ((DB.m_city.Where(x => x.del_status == false).Count() / int.Parse(ViewBag.search_item_count)) + ((DB.m_city.Where(x => x.del_status == false).Count() % int.Parse(ViewBag.search_item_count)) == 0 ? 0 : 1)).ToString();

            ViewBag.total_m_city = TempData["total_m_city"] != null ? TempData["total_m_city"] : DB.m_city.Where(x => x.del_status == false).Count().ToString();

            ViewBag.search_items_filter = TempData["search_items_filter"] != null ? TempData["search_items_filter"] : "";
            ViewBag.search_item_text = TempData["search_item_text"] != null ? TempData["search_item_text"] : "";
            ViewBag.state_list = DB.m_state.Where(x => x.del_status == false).ToList();
            var state_list = (from state in DB.m_state.Where(x => x.del_status == false).OrderBy(x => x.state_name)
                              select new SelectListItem()
                              {
                                  Text = state.state_name,
                                  Value = state.state_id.ToString(),
                              }).ToList();

            state_list.Insert(0, new SelectListItem()
            {
                Text = "--Select--",
                Value = ""
            });

            ViewBag.listofstate = new SelectList(state_list, "Value", "Text");

            ViewBag.error = TempData["error"];
            ViewBag.message = TempData["message"];

            return View();
        }

        /// <summary>
        /// state name search, count, filter, text, page..........
        /// </summary>

        public JsonResult master_city_list()
        {
            var m_city_list = DB.m_city.Where(x => x.del_status == false).OrderByDescending(x => x.created_date).Take(10)
                .Select(x => new
                {
                    x.city_id,
                    x.city_name,
                    x.state_id,
                    state_name = DB.m_state.FirstOrDefault(c => c.state_id == x.state_id).state_name // Assuming m_country is your country table and it has country_name column
                })
                .ToList();

            return Json(new { success = true, value = "True", city_list = m_city_list });
        }



        public IActionResult search_m_city(string search_item_count, string search_items_filter, string search_item_text, string search_item_page)
        {

            search_item_page = int.Parse(search_item_page) < 1 ? "1" : search_item_page;
            string page_count = null;
            int total_m_city = 0;

            List<m_city> M_City_List = new List<m_city>();
            List<m_state> M_State_List = new List<m_state>();
            switch (search_items_filter)
            {
                case "state_name":
                    if (search_item_text == null)
                    {
                        return RedirectToAction("m_city_list", "Master");
                    }
                    else
                    {
                        M_State_List = DB.m_state.Where(x => x.state_name == search_item_text).ToList();
                        M_City_List = DB.m_city.Where(x => x.state_id == M_State_List[0].state_id && x.del_status == false).
                                     Skip((int.Parse(search_item_page) - 1) * int.Parse(search_item_count)).Take(int.Parse(search_item_count)).ToList();
                        total_m_city = DB.m_city.Where(x => x.state_id == M_State_List[0].state_id && x.del_status == false).Count();
                        page_count = (total_m_city / int.Parse(search_item_count) + ((total_m_city % int.Parse(search_item_count)) == 0 ? 0 : 1)).ToString();
                    }
                    break;

                case "city_name":
                    if (search_item_text == null)
                    {
                        return RedirectToAction("m_city_list", "Master");
                    }
                    else
                    {
                        M_City_List = DB.m_city.Where(x => x.city_name.Contains(search_item_text.ToLower()) && x.del_status == false).
                        Skip((int.Parse(search_item_page) - 1) * int.Parse(search_item_count)).Take(int.Parse(search_item_count)).ToList();
                        total_m_city = DB.m_city.Where(x => x.city_name.Contains(search_item_text.ToLower()) && x.del_status == false).Count();
                        page_count = (total_m_city / int.Parse(search_item_count) + ((total_m_city % int.Parse(search_item_count)) == 0 ? 0 : 1)).ToString();
                    }
                    break;

                default:
                    M_City_List = DB.m_city.Where(x => x.del_status == false).Skip((int.Parse(search_item_page) - 1) * int.Parse(search_item_count)).Take(int.Parse(search_item_count)).ToList();
                    total_m_city = DB.m_state.Where(x => x.del_status == false).Count();
                    page_count = (total_m_city / int.Parse(search_item_count) + ((total_m_city % int.Parse(search_item_count)) == 0 ? 0 : 1)).ToString();
                    break;
            }

            TempData["m_city_list"] = JsonConvert.SerializeObject(M_City_List.Where(x => x.del_status == false).OrderByDescending(x => x.created_date));
            TempData["search_item_count"] = search_item_count;
            TempData["search_items_filter"] = search_items_filter;
            TempData["search_item_text"] = search_item_text;
            TempData["search_item_page"] = search_item_page;
            TempData["page_count"] = page_count;
            TempData["total_m_city"] = total_m_city.ToString();
            return RedirectToAction("m_city_list", "Master");
        }

        /// <summary>
        /// add city....
        /// </summary>

        [HttpGet]
        public IActionResult add_m_city()
        {
            var state_list = (from state in DB.m_state.Where(x => x.del_status == false).OrderBy(x => x.state_name)
                              select new SelectListItem()
                              {
                                  Text = state.state_name,
                                  Value = state.state_id.ToString(),
                              }).ToList();

            state_list.Insert(0, new SelectListItem()
            {
                Text = "--Select--",
                Value = ""
            });

            ViewBag.listofstate = new SelectList(state_list, "Value", "Text");
            return RedirectToAction("m_city_list", "Master");
        }

        [HttpPost]
        public IActionResult add_m_city(m_city add_obj)
        {
            string error = null;
            //string message = null;
            try
            {
                if (DB.m_city.Where(x => x.city_id == add_obj.city_id).Any(x => x.city_name == add_obj.city_name.Trim() && x.del_status == false))
                {
                    error = "This name already exists.";
                }
                else
                {
                    DB.m_city.Add(new m_city
                    {
                        state_id = add_obj.state_id,
                        city_name = add_obj.city_name,
                        created_date = DateTime.Now,
                        created_by = "1",
                        del_status = Convert.ToBoolean(0)
                    });
                    DB.SaveChanges();
                    return Json(new { success = true, value = "True" });
                    //message = "City detail added to database successfully.";

                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, value = "invalid_id" + ex.ToString() });
                //error = "Unable to add city detail to database. " + ex.ToString();

            }
            //ModelState.Clear();
            //ViewBag.message = message;
            //ViewBag.error = error;
            var state_list = (from state in DB.m_state.Where(x => x.del_status == false).OrderBy(x => x.state_name)
                              select new SelectListItem()
                              {
                                  Text = state.state_name,
                                  Value = state.state_id.ToString(),
                              }).ToList();

            state_list.Insert(0, new SelectListItem()
            {
                Text = "--Select--",
                Value = ""
            });

            ViewBag.listofstate = new SelectList(state_list, "Value", "Text");
            return RedirectToAction("m_city_list", "Master");
        }
        public JsonResult View_City(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }
            else
            {
                var get_m_city_detail = DB.m_city.Find(id);
                var state_list = DB.m_state.Where(x => x.del_status == false && x.state_id == get_m_city_detail.state_id).FirstOrDefault();

                return Json(new { success = true, value = "True", city_id = get_m_city_detail.city_id, city_name = get_m_city_detail.city_name, state_list = state_list });
            }
        }
        // [HttpGet]
        public JsonResult edit_m_city(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }
            else
            {
                var get_m_city_detail = DB.m_city.Find(id);
                var state_list = DB.m_state.Where(x => x.del_status == false && x.state_id == get_m_city_detail.state_id).FirstOrDefault();

                return Json(new { success = true, value = "True", city_id = get_m_city_detail.city_id, city_name = get_m_city_detail.city_name, state_list = state_list });
            }
        }

        [HttpPost]
        public IActionResult edit_m_city(m_city edit_obj)
        {
            string error = null;
            string message = null;

            try
            {
                if (DB.m_city.Where(x => x.city_id == edit_obj.city_id).Any(x => x.city_name != edit_obj.city_name.Trim() && x.del_status == false))
                {
                    error = "This name already exists.";
                }
                else
                {
                    edit_obj.del_status = Convert.ToBoolean(0);
                    edit_obj.created_date = DateTime.Now;
                    edit_obj.created_by = "1";


                    DB.Update(edit_obj);

                    DB.SaveChanges();
                    return Json(new { success = true, value = "True" });
                    //message = "state detail updated to database successfully.";

                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, value = "invalid_id" + ex.ToString() });
                //error = "Unable to update state detail to database. " + ex.ToString();

            }

            var state_list = (from state in DB.m_state.Where(x => x.del_status == false).OrderBy(x => x.state_name)
                              select new SelectListItem()
                              {
                                  Text = state.state_name,
                                  Value = state.state_id.ToString(),
                              }).ToList();

            state_list.Insert(0, new SelectListItem()
            {
                Text = "--Select--",
                Value = ""
            });

            ViewBag.listofstate = new SelectList(state_list, "Value", "Text");
            //ViewBag.message = message;
            //ViewBag.error = error;
            return RedirectToAction("m_city_list", "Master");
            //return View(edit_obj);
        }

        public IActionResult delete_m_city(int? city_id)
        {
            try
            {
                if (city_id == null)
                {
                    return RedirectToAction("m_city_list", "Master");
                }
                else
                {
                    var m_city_list = DB.m_city.Where(x => x.city_id == city_id).FirstOrDefault();
                    m_city_list.del_status = Convert.ToBoolean(1);
                    DB.SaveChanges();
                    TempData["message"] = "Your Data Is Deleted Successfully";
                    return Json(new { success = true, value = "True" });
                    //return RedirectToAction("m_city_list");

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, value = "invalid_id" + ex.ToString() });
            }
        }


    }
}
