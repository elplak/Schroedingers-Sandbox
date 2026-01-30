using System.Numerics;
using MathNet.Numerics;

namespace QuantumDynamics;


// Exports simulation data (time and probability density at selected grid points) to a CSV file; primarily D: 
// for later analysis or visualization.
// could be extended to export more data (e.g., expectation values) or different formats (e.g., binary)
public class SimulationExporter : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly SimulationSettings _simParams;

    public SimulationExporter(string filePath, SimulationSettings simParams)
    {
        _simParams = simParams;
        _writer = new StreamWriter(filePath);
        WriteHeader();
    }

    private void WriteHeader()
    {
        var headers = Enumerable.Range(0, _simParams.GridPoints)
            .Where(i => i % 4 == 0)
            .Select(i => (i * _simParams.Dx - _simParams.DomainLength / 2.0).ToString("F2"));
        _writer.WriteLine("t;" + string.Join(";", headers));
    }

    public void RecordStep(double time, Complex[] wavefunction)
    {
        var row = Enumerable.Range(0, _simParams.GridPoints)
            .Where(i => i % 4 == 0)
            .Select(i => wavefunction[i].MagnitudeSquared().ToString("G4")); // probability density
        _writer.WriteLine($"{time:F2};" + string.Join(";", row)); 
        // TODO: buffering writes for performance if needed (e.g., every N steps)
    }

    public void Dispose() => _writer.Dispose();
}