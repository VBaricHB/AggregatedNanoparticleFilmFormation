

using AggregateFormation.interfaces;
using CommonLibrary;
using CommonLibrary.interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregateFormation
{
    public class AggregateFormationService
    {
        private ISizeDistribution<double> _psd;
        private ISizeDistribution<int> _asd;
        private IConfig _config;
        private IParticleFormationParameters _parameters;
        private IParticleFactory<Aggregate> _particleGenerationService;


        public AggregateFormationService
            (
            ISizeDistribution<double> primaryParticleSizeDistribution, 
            ISizeDistribution<int> aggregateSizeDistribution,
            IConfig config,
            IParticleFormationParameters particleFormationParameters,
            IParticleFactory<Aggregate> particleGenerationService
            )
        {
            _psd = primaryParticleSizeDistribution;
            _asd = aggregateSizeDistribution;
            _config = config;
            _parameters = particleFormationParameters;
            _particleGenerationService = particleGenerationService;
        }

        public async Task<List<Aggregate>> GenerateAggregates_Async()
        {
            var aggregateSizes = GenerateAggregateSizes();
            var aggregateGenTasks = new List<Task<Aggregate>>();
            foreach( var size in aggregateSizes)
            {
                aggregateGenTasks.Add(Task.Run(() => _particleGenerationService.Build(size)));
            }

            var aggregates = await Task.WhenAll(aggregateGenTasks);

            return aggregates.ToList();

        }

        public List<Aggregate> GenerateAggregates()
        {
            var aggregateSizes = GenerateAggregateSizes();
            var aggregates = new List<Aggregate>();
            foreach (var size in aggregateSizes)
            {
                aggregates.Add(_particleGenerationService.Build(size));
            }

            return aggregates;
        }

        private List<int> GenerateAggregateSizes()
        {
            var sizes = new List<int>();
            var sum = 0;
            while (sum < _parameters.TotalPrimaryParticles)
            {
                sizes.Add(_asd.GetRandomSize());
                sum = sizes.Sum();
            }

            return sizes;
        }
    }
}
