namespace SmarcGUI.MissionPlanning.Tasks
{
    public class CustomTask : Task
    {
        // a task that can be customized with a json string
        // so that ppl can run "only-defined-in-a-vehicle" stuff from the gui
        // we'll implement the task proper if the tests show that this custom thing is useful :)
        public CustomTask(string description, string jsonParams)
        {
            Name = "custom-task";
            Description = description;
            Params.Add("json-params", jsonParams);
        }
    }
}