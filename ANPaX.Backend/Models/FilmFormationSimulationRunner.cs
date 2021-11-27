using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.FilmFormation;
using ANPaX.Simulation.FilmFormation.interfaces;

namespace ANPaX.Backend.Models
{
    public class FilmFormationSimulationRunner : ISimulationRunner
    {
        private readonly IFilmFormationConfig _filmFormationConfig;
        private readonly IProgress<ProgressReportModel> _progress;
        private IEnumerable<Aggregate> _aggregates;
        private CancellationTokenSource _cts;
        private double _delta = 1.01;
        private FilmFormationService _filmFormationService;
        private IParticleFilm<Aggregate> _result;

        public FilmFormationSimulationRunner(IEnumerable<Aggregate> aggregates, IFilmFormationConfig filmFormationConfig)
        {
            _aggregates = aggregates;
            _filmFormationConfig = filmFormationConfig;
            _progress = new Progress<ProgressReportModel>();
            _filmFormationService = new FilmFormationService(filmFormationConfig);
            _cts = new CancellationTokenSource();
        }

        public async Task Run_Async()
        {
            _result = await _filmFormationService.BuildFilm_Async(_aggregates, _progress, _delta, _cts.Token);
        }

        public IParticleFilm<Aggregate> Get_Film()
        {
            return _result;
        }

        public IEnumerable<Aggregate> Get_Aggregates()
        {
            return _result.Particles;
        }

        public void Cancel()
        {
            _cts.Cancel();
        }
    }
}
