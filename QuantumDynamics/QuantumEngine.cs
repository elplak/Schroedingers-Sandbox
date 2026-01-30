using System.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace QuantumDynamics;

public class QuantumEngine
{
    private readonly SimulationSettings _simParams;
    private readonly Complex[] _kineticPropagator; 
    private readonly Complex[] _potentialPropagator;

    public QuantumEngine(SimulationSettings parameters)
    {
        _simParams = parameters;
        _kineticPropagator = new Complex[_simParams.GridPoints];
        _potentialPropagator = new Complex[_simParams.GridPoints];
        PrecomputeOperators();
    }

    private void PrecomputeOperators()
    {
        for (int i = 0; i < _simParams.GridPoints; i++)
        {
            var x = i * _simParams.Dx - _simParams.DomainLength / 2.0; // position for potential calculation

            double potential;
            if (x is > 0 and < 1.5)
            {
                potential = _simParams.BarrierHeight;
            }
            else
            {
                potential = 0.0;
            }
            
            _potentialPropagator[i] = Complex.Exp(new Complex(0, -0.5 * potential * _simParams.TimeStep));
            
            double k; // wave number (momentum for free particle)
            if (i < _simParams.GridPoints / 2)
            {
                k = 2 * Math.PI * i / _simParams.DomainLength;
            }
            else
            {
                k = 2 * Math.PI * (i - _simParams.GridPoints) / _simParams.DomainLength;
            }

            var exponent = new Complex(0, -0.5 * k * k * _simParams.TimeStep);
            _kineticPropagator[i] = Complex.Exp(exponent);
        }
    }

    public void Step(Complex[] wavefunction)
    {
        // half step potential --> kinetic --> half step potential (split operator method (analysis))
        ApplyOperator(wavefunction, _potentialPropagator);

        Fourier.Forward(wavefunction, FourierOptions.Default);
        ApplyOperator(wavefunction, _kineticPropagator);
        Fourier.Inverse(wavefunction, FourierOptions.Default);

        ApplyOperator(wavefunction, _potentialPropagator);
    }

    private static void ApplyOperator(Complex[] target, Complex[] @operator)
    {
        for (int i = 0; i < target.Length; i++)
        {
            target[i] = target[i] * @operator[i]; // target *= operator[i]; --> no operator overloading for arrays
            // element-wise multiplication (vl -> could be vectorized for performance)
        }
    }
}