﻿using BCP_Facial.Data;
using BCP_Facial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Services
{
    public class RecognizerService
    {
        public Recognizer Recognizer { get; set; }
        public bool IsAuthentic { get; set; } = false;
        public bool IsExist { get; set; }
        public ApplicationDbContext _db { get; set; }

        public RecognizerService(string RecognizerId, ApplicationDbContext db)
        {
            Recognizer = db.Recognizers.Where(e => e.Id.Equals(RecognizerId)).FirstOrDefault();
            if (Recognizer == null)
            {
                IsExist = false;
            } else
            {
                IsExist = true;
            }

            _db = db;
        }

        public RecognizerService(string RecognizerId, string RecognizerKey, ApplicationDbContext db)
        {
            Recognizer = db.Recognizers.Where(e => e.Id.Equals(RecognizerId)).FirstOrDefault();
            if (Recognizer == null)
            {
                IsExist = false;
            } else
            {
                if (Recognizer.Key.Equals(RecognizerKey))
                {
                    IsAuthentic = true;
                }

                IsExist = true;
            }

            _db = db;
        }

        public RecognizerTask GetLastUnreadTask()
        {
            return Recognizer.List_RecognizerTask.Where(e => e.Deleted == false).Where(e => e.Status == 1).OrderBy(e => e.DateCreated).FirstOrDefault();
        }

        public string GetStatus()
        {
            if (IsExist)
            {
                RecognizerTask task = Recognizer.List_RecognizerTask.Where(e => e.Deleted == false).Where(e => e.Status == 1 || e.Status == 2 ).OrderBy(e => e.DateCreated).FirstOrDefault();
                if (task == null)
                {
                    return "Idle";
                } else if (task.Command == "REGISTER_NEW_FACE")
                {
                    CheckExpiry();
                    return "Registering face";
                } else if (task.Command == "CAPTURE_CLASS_IMAGE")
                {
                    CheckExpiry();
                    return "Capturing class image";
                } else
                {
                    CheckExpiry();
                    return "Unknown";
                }
            } else
            {
                return null;
            }
        }

        public string GetDurationSince()
        {
            RecognizerTask task = Recognizer.List_RecognizerTask.Where(e => e.Deleted == false).OrderByDescending(e => e.DateModified).FirstOrDefault();
            TimeSpan duration;
            if (task == null)
            {
                duration = DateTime.UtcNow.AddHours(8) - Recognizer.DateCreated;
            } else
            {
                duration = DateTime.UtcNow.AddHours(8) - task.DateModified;
            }

            if (duration.TotalMinutes < 1)
            {
                return Math.Floor(duration.TotalSeconds) + " Second(s)";
            } else if (duration.TotalHours < 1)
            {
                return Math.Floor(duration.TotalMinutes) + " Minute(s), " + Math.Floor((duration.TotalSeconds - (Math.Floor(duration.TotalMinutes) * 60))) + " Second(s)";
            }
            else if (duration.TotalDays < 1)
            {
                return Math.Floor(duration.TotalHours) + " Hour(s), " + Math.Floor((duration.TotalMinutes - (Math.Floor(duration.TotalHours) * 60))) + " Minute(s)";
            }
            else
            {
                return Math.Floor(duration.TotalDays) + " Day(s)";
            }
        }

        public void CheckExpiry()
        {
            RecognizerTask task = Recognizer.List_RecognizerTask.Where(e => e.Deleted == false).OrderByDescending(e => e.DateModified).FirstOrDefault();
            TimeSpan timeSpan = DateTime.UtcNow.AddHours(8) - task.DateModified;
            if (timeSpan.TotalMinutes > 5)
            {
                task.Status = 0;
            }

            _db.SaveChanges();
        }
    }
}
