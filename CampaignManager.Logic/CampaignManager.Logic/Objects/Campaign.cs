using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignManager.Logic
{
    public class Campaign
    {
        public string Template { get; set; }
        public Func<Customer, bool> Condition { get; set; }
        public DateTime Time { get; set; }
        public int Priority { get; set; }
    }

    public class CampaignManager
    {
        public event EventHandler CampaignAdded;

        private List<Campaign> campaigns = new List<Campaign>();

        public void AddCampaign(Campaign campaign)
        {
            campaigns.Add(campaign);
            OnCampaignAdded(EventArgs.Empty);
        }

        public List<Campaign> GetCampaigns()
        {
            return campaigns.OrderBy(c => c.Priority).ToList();
        }

        protected virtual void OnCampaignAdded(EventArgs e)
        {
            CampaignAdded?.Invoke(this, e);
        }
    }
}
