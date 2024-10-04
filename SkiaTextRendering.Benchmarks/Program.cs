using BenchmarkDotNet.Running;

namespace SkiaTextRendering.Benchmarks;

public class Program
{
	private static void Main(string[] args)
	{
		var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
	}
}