using System.Numerics;
using QuantumDynamics;

var config = new SimulationSettings();
// create the quantum engine with given settings (time step, grid points,...) 
var engine = new QuantumEngine(config); 
var wavefunction = InitializeWavefunction(config);

using var exporter = new SimulationExporter("results.csv", config);

for (int i = 0; i < 300; i++)
{
    if (i % 10 == 0)
    {
        exporter.RecordStep(i * config.TimeStep, wavefunction);
    }
    engine.Step(wavefunction);
}

return;

// init wavefunctino as Gaussian wave packet - envelope times momentum (complex exponential)
// centered at x = -8 with width 2 and initial momentum defined in settings
// returns array of complex numbers representing the wavefunction on the grid
// TODO: normalize the wavefunction
Complex[] InitializeWavefunction(SimulationSettings p)
{
    var waveFunction = new Complex[p.GridPoints];
    for (int i = 0; i < p.GridPoints; i++)
    {
        var x = i * p.Dx - p.DomainLength / 2.0;
        const double center = -8.0;
        const double width = 2.0;

        var distance = x - center;
        var envelope = Math.Exp(-0.5 * Math.Pow(distance / width, 2));

        var phase = p.InitialMomentum * x;
        var momentum = Complex.Exp(new Complex(0, phase));
        waveFunction[i] = envelope * momentum;
    }
    // could be normalized here --> TODO 
    return waveFunction;
}