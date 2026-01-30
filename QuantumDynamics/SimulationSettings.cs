namespace QuantumDynamics;

public record SimulationSettings(
    int GridPoints = 512,
    double DomainLength = 60.0,
    double TimeStep = 0.05,
    double InitialMomentum = 4.0,
    double BarrierHeight = 20.0
)
{
    public double Dx => DomainLength / GridPoints; // spatial step size TODO: make property for consistency (readonly)
}