using Microsoft.AspNetCore.SignalR;

namespace SignalRWebAPI
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHubContext<StockHub> _stockHub;
        private const string stockName = "Basic Stock Name";
        private decimal stockPrice = 100;

        public Worker(ILogger<Worker> logger, IHubContext<StockHub> stockHub)
        {
            _logger = logger;
            _stockHub = stockHub;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    Random rnd = new Random();
                    decimal stockRaise = rnd.Next(1, 10000);
                    //List of stock names
                    string[] stockNames = { "Apple", "Microsoft", "Google", "Amazon", "Facebook" };
                    //Random stock name
                   var stockName = stockNames[rnd.Next(0, stockNames.Length)];
                    await _stockHub.Clients.All.SendAsync("ReceiveStockPrice", stockName, stockRaise);

                    _logger.LogInformation("Sent stock price: {stockName} - {stockRaise}", stockName, stockRaise);

                    await Task.Delay(4000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending stock price");
                }
            }
        }
    }
}
