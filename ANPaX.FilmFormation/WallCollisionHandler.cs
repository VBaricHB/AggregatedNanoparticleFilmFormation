using System.Collections.Generic;

using ANPaX.Collection;
using ANPaX.FilmFormation.interfaces;

namespace ANPaX.FilmFormation
{
    internal class WallCollisionHandler : IWallCollisionHandler
    {
        private readonly ISimulationBox _simulationBox;

        public WallCollisionHandler(ISimulationBox simulationBox)
        {
            _simulationBox = simulationBox;
        }

        public void CheckPrimaryParticle(PrimaryParticle primaryParticle)
        {
            CheckXDimension(primaryParticle);
            CheckYDimension(primaryParticle);
        }

        public void CheckPrimaryParticle(IEnumerable<PrimaryParticle> primaryParticles)
        {
            foreach (var primaryParticle in primaryParticles)
            {
                CheckPrimaryParticle(primaryParticle);
            }
        }

        private void CheckXDimension(PrimaryParticle primaryParticle)
        {
            if (primaryParticle.Position.X > _simulationBox.XDim.Upper)
            {
                primaryParticle.MoveBy(new Vector3(-1 * _simulationBox.XDim.Width, 0, 0));
            }
            if (primaryParticle.Position.X < _simulationBox.XDim.Lower)
            {
                primaryParticle.MoveBy(new Vector3(_simulationBox.XDim.Width, 0, 0));
            }
        }

        private void CheckYDimension(PrimaryParticle primaryParticle)
        {
            if (primaryParticle.Position.Y > _simulationBox.YDim.Upper)
            {
                primaryParticle.MoveBy(new Vector3(0, -1 * _simulationBox.YDim.Width, 0));
            }
            if (primaryParticle.Position.Y < _simulationBox.YDim.Lower)
            {
                primaryParticle.MoveBy(new Vector3(0, _simulationBox.YDim.Width, 0));
            }
        }
    }
}
