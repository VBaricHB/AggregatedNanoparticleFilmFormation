using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.FilmFormation.interfaces;

using Moq;

using Xunit;

namespace ANPaX.Simulation.FilmFormation.tests
{
    public class BallisticAggregateDepositionTest
    {

        private IFilmFormationConfig _config = new TestFilmFormationConfig();
        private readonly INeighborslistFactory _neighborslistfactory = new AccordNeighborslistFactory();
        private static Aggregate GetExampleAggregate()
        {
            var radius = 1.0;
            var p11 = new PrimaryParticle(0, new Vector3(0, 0, 100), radius);
            var p12 = new PrimaryParticle(1, new Vector3(2 * radius, 0, 100), radius);
            var p13 = new PrimaryParticle(2, new Vector3(0, 2 * radius, 100), radius);
            var p14 = new PrimaryParticle(3, new Vector3(2 * radius, 2 * radius, 100), radius);
            var cluster1 = new Cluster(0, new List<PrimaryParticle>() { p11, p12, p13, p14 });

            var p21 = new PrimaryParticle(0, new Vector3(0, 0, 100 - 2 * radius), radius);
            var p22 = new PrimaryParticle(1, new Vector3(2 * radius, 0, 100 - 2 * radius), radius);
            var p23 = new PrimaryParticle(2, new Vector3(0, 2 * radius, 100 - 2 * radius), radius);
            var p24 = new PrimaryParticle(3, new Vector3(2 * radius, 2 * radius, 100 - 2 * radius), radius);
            var cluster2 = new Cluster(0, new List<PrimaryParticle>() { p21, p22, p23, p24 });

            var aggregate = new Aggregate(new List<Cluster>() { cluster1, cluster2 });

            return aggregate;
        }

        private static IEnumerable<PrimaryParticle> GetDepositedParticles()
        {
            var radius = 1.0;
            var pos2 = new Vector3(0, 0, 1);
            var pp2 = new PrimaryParticle(1, pos2, radius);
            return new List<PrimaryParticle>() { pp2 };
        }

        private static IEnumerable<PrimaryParticle> GetDepositedParticlesFarAway()
        {
            var radius = 1.0;
            var pos2 = new Vector3(30, 30, 1);
            var pp2 = new PrimaryParticle(1, pos2, radius);
            return new List<PrimaryParticle>() { pp2 };
        }

        [Fact]
        private void DepositAggregateOnGroundTest()
        {
            var spHandler = new BallisticSingleParticleDepositionHandler();
            var aggHandler = new BallisticAggregateDepositionHandler(spHandler);
            var aggregate = GetExampleAggregate();
            aggHandler.DepositOnGround(aggregate);
            Assert.Equal(1.0, aggregate.Cluster.SelectMany(c => c.PrimaryParticles).Select(p => p.Position.Z).Min());
        }

        [Fact]
        private void IsWithoutContactTest_NoContact_ShouldBeTrue()
        {
            var spHandler = new BallisticSingleParticleDepositionHandler();
            var aggHandler = new BallisticAggregateDepositionHandler(spHandler);

            var distance = _config.LargeNumber;

            var isWithoutContact = aggHandler.IsWithoutContact(distance, _config.LargeNumber);
            Assert.True(isWithoutContact);
        }

        [Fact]
        private void IsWithoutContactTest_HasContact_ShouldBeFalse()
        {
            var spHandler = new BallisticSingleParticleDepositionHandler();
            var aggHandler = new BallisticAggregateDepositionHandler(spHandler);

            var isWithoutContact = aggHandler.IsWithoutContact(20, _config.LargeNumber);
            Assert.False(isWithoutContact);
        }

        [Fact]
        private void DepositAtParticleTest_CorrectDepositionDistance()
        {
            var spHandler = new BallisticSingleParticleDepositionHandler();
            var aggHandler = new BallisticAggregateDepositionHandler(spHandler);

            var aggregate = GetExampleAggregate();

            aggHandler.DepositAtParticle(aggregate, 20);

            Assert.Equal(78.0, aggregate.Cluster.SelectMany(c => c.PrimaryParticles).Select(p => p.Position.Z).Min());
        }

        [Fact]
        private async Task DepositAggregate_DepositAtPrimaryParticle()
        {
            var spHandler = new BallisticSingleParticleDepositionHandler();
            var aggHandler = new BallisticAggregateDepositionHandler(spHandler);

            var aggregate = GetExampleAggregate();
            var neighborslist = new Accord2DNeighborslist(GetDepositedParticles());
            var ct = new CancellationToken();
            await aggHandler.DepositAggregate_Async(aggregate, GetDepositedParticles(), neighborslist, GetDepositedParticles().GetMaxRadius(), Environment.ProcessorCount, _config.Delta, ct);

            Assert.Equal(3.0, aggregate.Cluster.SelectMany(c => c.PrimaryParticles).Select(p => p.Position.Z).Min());
        }

        [Fact]
        private async Task DepositAggregate_DepositOnGround()
        {
            var spHandler = new BallisticSingleParticleDepositionHandler();
            var aggHandler = new BallisticAggregateDepositionHandler(spHandler);

            var aggregate = GetExampleAggregate();
            var neighborslist = new Mock<INeighborslist>().Object;
            var ct = new CancellationToken();
            await aggHandler.DepositAggregate_Async(aggregate, GetDepositedParticlesFarAway(), neighborslist, GetDepositedParticlesFarAway().GetMaxRadius(), Environment.ProcessorCount, _config.Delta, ct);

            Assert.Equal(1.0, aggregate.Cluster.SelectMany(c => c.PrimaryParticles).Select(p => p.Position.Z).Min());
        }
    }
}
