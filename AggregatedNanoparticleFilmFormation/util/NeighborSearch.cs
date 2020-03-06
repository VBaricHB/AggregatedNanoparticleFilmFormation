using System.Collections.Generic;
using Accord.Collections;
using Common;
using Common.interfaces;
using AggregatedNanoparticleFilmFormation.interfaces;
using System.Linq;

namespace AggregatedNanoparticleFilmFormation
{
    public class NeighborSearch
    {

        private KDTree<PrimaryParticle> Tree { get; set; }
        private ISimulationDomain SimulationDomain { get; }
        private IConfig Config { get; }

        public NeighborSearch(ISimulationDomain simulationDomain, IConfig config)
        {
            SimulationDomain = SimulationDomain;
            Config = config;
        }

        /// <summary>
        /// Build a new KDTree from the deposited primaryParticles
        /// </summary>
        public void Build()
        {
            Tree = KDTree.FromData<PrimaryParticle>(GetPrimaryParticleCoordinates());
        }

        /// <summary>
        /// Search for all neighbors around a primaryParticle.
        /// The search radius is the primaryParticle radius
        /// + max Radius in the simulation
        /// + an additional safety distance
        /// </summary>
        /// <param name="primaryParticle">Find neighbors around this PrimaryParticle.</param>
        /// <returns></returns>
        public List<NodeDistance<KDTreeNode<PrimaryParticle>>> GetNeighbors(PrimaryParticle primaryParticle)
        {
            return Tree.Nearest(primaryParticle.Position.ToArray(),
                radius: primaryParticle.Radius + SimulationDomain.MaxPrimaryParticleRadius + Config.NeighborAddDistance);
        }

        private double[][] GetPrimaryParticleCoordinates()
        {
            double[][] coordinates = new double[SimulationDomain.DepositedPrimaryParticles][];
            int i = 0;
            foreach(var aggregate in SimulationDomain.DepositedAggregates)
            {
                foreach (var primaryParticle in aggregate.Cluster.SelectMany(c => c.PrimaryParticles))
                {
                    coordinates[i++] = primaryParticle.Position.ToArray();
                }
            }
            return coordinates;
        }
    }
}
