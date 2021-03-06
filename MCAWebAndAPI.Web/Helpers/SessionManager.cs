﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCAWebAndAPI.Web.Helpers
{
    public class SessionManager
    {
        public static T Get<T>(string key)
        {
            object sessionObject = HttpContext.Current.Session[key];
            if (sessionObject == null)
            {
                return default(T);
            }
            return (T)HttpContext.Current.Session[key];
        }

        public static T Get<T>(string key, string group)
        {
            object sessionObject = HttpContext.Current.Session[group + "_" + key];
            if (sessionObject == null)
            {
                return default(T);
            }
            return (T)HttpContext.Current.Session[group + "_" + key];
        }

        public static T Get<T>(string key, T defaultValue)
        {
            object sessionObject = HttpContext.Current.Session[key];
            if (sessionObject == null)
            {
                HttpContext.Current.Session[key] = defaultValue;
            }

            return (T)HttpContext.Current.Session[key];
        }

        public static T Get<T>(string key, string group, T defaultValue)
        {
            object sessionObject = HttpContext.Current.Session[group + "_" + key];
            if (sessionObject == null)
            {
                HttpContext.Current.Session[group + "_" + key] = defaultValue;
            }

            return (T)HttpContext.Current.Session[group + "_" + key];
        }

        public static void Set<T>(string key, T entity)
        {
            HttpContext.Current.Session[key] = entity;
        }

        public static void Set<T>(string key, string group, T entity)
        {
            HttpContext.Current.Session[group + "_" + key] = entity;
        }

        public static void Remove(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        public static void RemoveAllSessions()
        {
            if (HttpContext.Current.Session.Keys.Count > 0)
                HttpContext.Current.Session.Clear();
        }

    }
}