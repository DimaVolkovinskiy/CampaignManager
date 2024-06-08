using System.Globalization;

namespace CampaignManager.Logic
{
    class Program
    {
        private const string DELIMITER = ",";
        private const string CUSTOMER_CSV_NAME = @"..\\..\\..\\Resources\\Customers\\customers.csv";
        private const string SENDS_FILE_PATH = "sends.txt";

        private CancellationTokenSource cts = new CancellationTokenSource();
        private CampaignManager CampaignManager = new CampaignManager();
        private List<Customer> Customers = new List<Customer>();
        private Dictionary<int, DateTime> customersSentToday = new Dictionary<int, DateTime>();

        static async void Main(string[] args)
        {
            ReadCustomersCSV();

            await MonitorCampaignsAsync(cts.Token);

        }

        private async Task MonitorCampaignsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var sortedCampaigns = CampaignManager.GetCampaigns();

                foreach (var campaign in sortedCampaigns)
                {
                    if (campaign.Time <= now)
                    {
                        SendCampaign(campaign);
                        CampaignManager.GetCampaigns().Remove(campaign);
                        break;
                    }
                }

                await Task.Delay(60000);
            }
        }

        private void SendCampaign(Campaign campaign)
        {
            ReadCustomersCSV();

            List<Customer> filteredCustomers = Customers.Where(campaign.Condition).ToList();

            foreach (Customer customer in filteredCustomers) 
            {
                if (HasReceivedCampaignToday(customer.Id) == false)
                {
                    WriteToFile(($"Sending campaign: {campaign.Template}, Priority: {campaign.Priority}, Customer: {customer.Id} Time: {campaign.Time}"));
                    MarkCampaignAsSent(customer.Id);
                }
                
            }
        }

        private bool HasReceivedCampaignToday(int customerId)
        {
            return customersSentToday.ContainsKey(customerId) && customersSentToday[customerId].Date == DateTime.Now.Date;
        }

        private void MarkCampaignAsSent(int customerId)
        {
            if (customersSentToday.ContainsKey(customerId))
            {
                customersSentToday[customerId] = DateTime.Now;
            }
            else
            {
                customersSentToday.Add(customerId, DateTime.Now);
            }
        }

        private void ReadCustomersCSV()
        {
            List<string> Lines = File.ReadLines(CUSTOMER_CSV_NAME).ToList();
            Customers = Lines
            .Skip(1)
            .Select(line => line.Split(DELIMITER))
            .Select(values => new Customer
            {
                Id = int.Parse(values[0]),
                Age = int.Parse(values[1]),
                Gender = values[2],
                City = values[3],
                Deposit = double.Parse(values[4], CultureInfo.InvariantCulture),
                IsNew = values[5] == "1"
            })
            .ToList();
        }

        private void WriteToFile(string message)
        {
            using (var writer = new StreamWriter(SENDS_FILE_PATH))
            {
                writer.WriteLine(message);
            }
        }
    }

}

