using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCAWebAndAPI.Web.Helpers
{
    public static class MathHelper
    {
        //Function to get random number
        static readonly Random getrandom = new Random();
        static readonly object syncLock = new object();
        public static int GetRandomNumber()
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(0, 1000);
            }
        }
    }
}