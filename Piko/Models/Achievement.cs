namespace Piko.Models
{
    public class Achievement
    {
        public string Id          { get; set; } = "";
        public string Name        { get; set; } = "";
        public string Description { get; set; } = "";
        public string Icon        { get; set; } = "o";
        public int    XpReward    { get; set; } = 25;
    }
}
