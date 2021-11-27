using System.Collections.Generic;

using ANPaX.Core;
using ANPaX.Core.ParticleFilm.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Simulation.FilmFormation
{
    public class OpenWallCollisionHandler : IWallCollisionHandler
    {
        /// <summary>
        /// Nothing to do here, since particles can penetrate the wall and have no lateral movement.
        /// </summary>
        /// <param name="primaryParticle"></param>
        /// <param name="simulationBox"></param>
        public void CheckPrimaryParticle(PrimaryParticle primaryParticle, ISimulationBox simulationBox)
        {
            return;
        }

        /// <summary>
        /// <inheritdoc cref="CheckPrimaryParticle(PrimaryParticle, ISimulationBox)"/>
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="simulationBox"></param>
        public void CheckPrimaryParticle(IEnumerable<PrimaryParticle> enumerable, ISimulationBox simulationBox)
        {
            return;
        }
    }
}
