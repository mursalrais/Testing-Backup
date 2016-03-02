using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using System;

namespace MCAWebAndAPI.Console
{
    class Program
    {
        static ITaskService service = new TaskService();

        static void Main(string[] args)
        {
            try
            {
                UpdateTodayValue();
            }catch(Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            System.Console.WriteLine("DONE");
            System.Console.ReadLine();
        }
        static void UpdateTodayValue()
        {
            service.UpdateTodayValue();
        }
        static void CalculateTaskSummary()
        {
            int numberOfUpdatedSummaryTasks = 0;
            System.Console.WriteLine("START to CALCULATE TASKS");
            numberOfUpdatedSummaryTasks = service.CalculateTaskColumns();

            System.Console.WriteLine(string.Format("{0} summary tasks have been updated", numberOfUpdatedSummaryTasks));
        }
    }
}
