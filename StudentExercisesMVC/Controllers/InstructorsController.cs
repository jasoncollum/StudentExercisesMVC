﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercises.Models.ViewModels;
using StudentExercisesMVC.Models;

namespace StudentExercisesMVC.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IConfiguration _config;

        public InstructorsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Instructors
        public ActionResult Index()
        {
            var instructors = new List<Instructor>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, SlackHandle, Specialty, CohortId
                        FROM Instructor
                   ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        instructors.Add(new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        });
                    }
                    reader.Close();
                }
            }

            return View(instructors);
        }

        // GET: Instructors/Details/5
        public ActionResult Details(int id)
        {
            Instructor instructor = GetOneInstructor(id);

            return View(instructor);
        }

        // GET: Instructors/Create
        public ActionResult Create()
        {
            var viewModel = new InstructorCreateViewModel(_config.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Instructor instructor)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO Instructor
                            ( FirstName, LastName, SlackHandle, Specialty, CohortId )
                            VALUES
                            ( @firstName, @lastName, @slackHandle, @specialty, @cohortId )";
                        cmd.Parameters.Add(new SqlParameter("@firstName", instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", instructor.Specialty));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", instructor.CohortId));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Instructors/Edit/5
        public ActionResult Edit(int id)
        {
            Instructor instructor = GetOneInstructor(id);

            return View(instructor);
        }

        // POST: Instructors/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, InstructorEditViewModel model)
        //{
        //    try
        //    {
        //        using(SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using(SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"UPDATE Instructor
        //                                    SET
        //                                        FirstName = @firstName,
        //                                        LastName = @lastName,
        //                                        SlackHandle = @slackHandle,
        //                                        Specialy = @specialty,
        //                                        CohortId = @cohortId
        //                                    ";
        //                cmd.Parameters.Add(new SqlParameter("@firstName", model.Instructor.FirstName));
        //                cmd.Parameters.Add(new SqlParameter("@lastName", model.Instructor.LastName));
        //                cmd.Parameters.Add(new SqlParameter("@slackHandle", model.Instructor.SlackHandle));
        //                cmd.Parameters.Add(new SqlParameter("@slackHandle", model.Instructor.Specialty));
        //                cmd.Parameters.Add(new SqlParameter("@cohortId", model.Instructor.CohortId));

        //                cmd.ExecuteNonQuery();
        //            }
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Instructor/Delete/5
        public ActionResult Delete(int id)
        {
            Instructor instructor = GetOneInstructor(id);

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM StudentExercise
                                            WHERE InsructorId = @id;
                                            DELETE FROM Instructor
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public Instructor GetOneInstructor(int id)
        {
            Instructor instructor = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, SlackHandle, Specialty, CohortId
                        FROM Instructor
                        WHERE Id = @id
                   ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        instructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };
                    }
                    reader.Close();
                }
            }

            return instructor;
        }
    }
}