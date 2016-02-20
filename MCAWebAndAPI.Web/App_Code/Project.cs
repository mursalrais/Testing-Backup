
public class ProjectInformation
{
    public string ProjectName { get; set; }
    public string ProjectManager { get; set; }
    public string ProjectStart { get; set; }
    public string ProjectFinish { get; set; }
    public double PercentComplete { get; set; }
    public string ScheduleStatus { get; set; }
    public string Url { get; set; }
}


public class Program
{
    public string ProgramTitle { get; set; }
    public int NoProject { get; set; }
    public int OnSchedule { get; set; }
    public int BehindSchedule { get; set; }
    public int SignificantlyBehindSchedule  { get; set; }
    public string Url { get; set; }
}