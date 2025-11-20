using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NPC
{
    public string Name { get; set; }
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string Race { get; set; }
    public string Description { get; set; }
    public string Goal { get; set; }
    public string Home { get; set; }

    public NPC(string name, string race, string description, string goal, string home)
    {
        Name = name;
        Race = race;
        Description = description;
        Goal = goal;
        Home = home;
    }
}