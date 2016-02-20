using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Console
{
    class Program
    {
        static ITaskService service = new TaskService();

        static void Main(string[] args)
        {
            var result = service.GetAllTask();


            System.Console.ReadLine();
        }
    }
}
