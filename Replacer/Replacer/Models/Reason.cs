namespace Replacer.Models
{
    public class Reason
    {
        public Reason()
        {
            this.NameReason = "Not found!!!";
            this.NameRecommendation = "Not found!!!";
            this.WasUsed = false;
        }
        public string NameReason { get; set; }
        public string NameRecommendation { get; set; }
        public bool WasUsed { get; set; }
    }
}
