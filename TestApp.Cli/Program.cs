// See https://aka.ms/new-console-template for more information

if (args.Length > 0 && args[0] == "say-hello")
{
    Console.WriteLine("Hello, World!");
}
else
{
    Console.WriteLine("Running migrations...");
    await Task.Delay(30_000);//  Thread.Sleep(30_000);
    Console.WriteLine("Migrations complete!");
}
