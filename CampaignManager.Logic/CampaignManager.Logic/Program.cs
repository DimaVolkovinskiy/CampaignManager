using System.Globalization;

namespace CampaignManager.Logic
{
    class Program
    {
        private const string DELIMITER = ",";
        private const string CUSTOMER_CSV_NAME = @"..\\..\\..\\Resources\\Customers\\customers.csv";

        static List<Customer> Customers = new List<Customer>();

        static List<Campaign> campaigns = new List<Campaign>
        {
        new Campaign { Template = "A", Condition = c => c.Gender == "male", Time = "10:15", Priority = 1 },
        new Campaign { Template = "B", Condition = c => c.Age > 45, Time = "10:05", Priority = 2 },
        new Campaign { Template = "C", Condition = c => c.City == "New York", Time = "10:10", Priority = 5 },
        new Campaign { Template = "A", Condition = c => c.Deposit > 100, Time = "10:15", Priority = 3 },
        new Campaign { Template = "C", Condition = c => c.IsNew, Time = "10:05", Priority = 4 },
        };

        static void Main(string[] args)
        {
            ReadCustomersCSV();
            var scheduledCampaigns = ScheduleCampaigns(Customers, campaigns);
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            WriteToFile(scheduledCampaigns, date);

            // Імітація очікування 30 хвилин
            Task.Delay(TimeSpan.FromMinutes(30)).Wait();
        }

        static void ReadCustomersCSV()
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

        static List<(int CustomerId, string Template, string Time, int Priority)> ScheduleCampaigns(List<Customer> customers, List<Campaign> campaigns)
        {
            var scheduled = new List<(int CustomerId, string Template, string Time, int Priority)>();

            foreach (var campaign in campaigns)
            {
                var filteredCustomers = customers.Where(campaign.Condition).ToList();
                foreach (var customer in filteredCustomers)
                {
                    if (!scheduled.Any(s => s.CustomerId == customer.Id))
                    {
                        scheduled.Add((customer.Id, campaign.Template, campaign.Time, campaign.Priority));
                    }
                }
            }

            return scheduled.OrderBy(s => s.Priority).ToList();
        }

        static void WriteToFile(List<(int CustomerId, string Template, string Time, int Priority)> scheduled, string date)
        {
            var filename = $"sends_{date}.txt";
            using (var writer = new StreamWriter(filename))
            {
                foreach (var entry in scheduled)
                {
                    writer.WriteLine($"{entry.CustomerId}, {entry.Template}, {entry.Time}, {entry.Priority}");
                }
            }
        }
    }

}

