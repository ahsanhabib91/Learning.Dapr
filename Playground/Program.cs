using Dapr.Client;

string DAPR_STORE_NAME = "statestore";


using var client = new DaprClientBuilder().Build();

Random random = new Random();
int orderId = random.Next(1,1000);
await client.SaveStateAsync(DAPR_STORE_NAME, "order_1", orderId.ToString());
await client.SaveStateAsync(DAPR_STORE_NAME, "order_2", orderId.ToString());
var result = await client.GetStateAsync<string>(DAPR_STORE_NAME, "order_1");
Console.WriteLine("Result after get: " + result);


