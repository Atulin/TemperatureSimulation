using ScottPlot;

const int max = 1_000;
const int reps = 10;
const int desiredTemp = 15;

Console.WriteLine("Generate SVG? [y/n] (default [y])");
var svg = Console.ReadLine() is "" or "y";
Console.WriteLine("Generate CSV? [y/n] (default [y])");
var csv = Console.ReadLine() is "" or "y";

var runs = Enumerable.Range(1, reps).Select(Generate);
await Task.WhenAll(runs);
return;

async Task Generate(int id)
{
    Console.WriteLine($"Generating dataset {id}");
    var bias = 0d;
    var temp = 0d;

    var points = new List<double>();
    var count = -1;
    var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1));
    while (await timer.WaitForNextTickAsync() && ++count < max)
    {
        var point = TickFunction(count);
        points.Add(point);
    }

    if (csv)
    {
        await File.WriteAllTextAsync($"plot_{id}.csv", string.Join(',', points));
    }

    if (svg)
    {
        var plot = new Plot();
        plot.Add.Signal(points);
        plot.SaveSvg($"plot_{id}.svg", max + 500, 200);
    }

    Console.WriteLine($"Generating dataset {id} done");
    return;
    
    double TickFunction(int tick)
    {
        // If bias is already negative or positive, use it for the change sign
        var sign = bias != 0 ? Math.Sign(bias) : Random.Shared.Next(-1, 1);
        
        // Change amount, up to 5 degrees at a time
        var amount = Random.Shared.NextDouble() * 5;
        
        var change = sign * amount;
        
        if (bias == 0)
        {
            // No bias currently, replace it with the current random change
            bias = change;
        }
        else
        {
            // The higher the bias, the higher the bias change will be, never less than half a degree
            // More extreme temperatures will hopefully mellow out faster, but temperatures around zero will persist longer
            var factor = Math.Max(0.5, bias / 2d);
            bias += factor * (bias switch
            {
                < 0 => 1,
                > 0 => -1,
                _ => 0
            });
        }
        temp += change;
        return temp;
    }
}