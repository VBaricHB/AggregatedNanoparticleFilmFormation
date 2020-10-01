using System.Collections.Generic;

using ANPaX.Core;
using ANPaX.Core.ParticleFilm.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation
{
    public class PeriodicBoundaryCollisionHandler : IWallCollisionHandler
    {

        public PeriodicBoundaryCollisionHandler()
        {
        }

        public void CheckPrimaryParticle(PrimaryParticle primaryParticle, ISimulationBox simulationBox)
        {
            CheckXDimension(primaryParticle, simulationBox);
            CheckYDimension(primaryParticle, simulationBox);
        }

        public void CheckPrimaryParticle(IEnumerable<PrimaryParticle> primaryParticles, ISimulationBox simulationBox)
        {
            foreach (var primaryParticle in primaryParticles)
            {
                CheckPrimaryParticle(primaryParticle, simulationBox);
            }
        }

        private void CheckXDimension(PrimaryParticle primaryParticle, ISimulationBox simulationBox)
        {
            if (primaryParticle.Position.X > simulationBox.XDim.Upper)
            {
                primaryParticle.MoveBy(new Vector3(-1 * simulationBox.XDim.Width, 0, 0));
            }
            if (primaryParticle.Position.X < simulationBox.XDim.Lower)
            {
                primaryParticle.MoveBy(new Vector3(simulationBox.XDim.Width, 0, 0));
            }
        }

        private void CheckYDimension(PrimaryParticle primaryParticle, ISimulationBox simulationBox)
        {
            if (primaryParticle.Position.Y > simulationBox.YDim.Upper)
            {
                primaryParticle.MoveBy(new Vector3(0, -1 * simulationBox.YDim.Width, 0));
            }
            if (primaryParticle.Position.Y < simulationBox.YDim.Lower)
            {
                primaryParticle.MoveBy(new Vector3(0, simulationBox.YDim.Width, 0));
            }
        }
    }
}
