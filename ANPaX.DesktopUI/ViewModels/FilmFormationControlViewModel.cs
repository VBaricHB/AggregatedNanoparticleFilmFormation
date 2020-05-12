using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ANPaX.AggregateFormation;
using ANPaX.Core;
using ANPaX.DesktopUI.Models;
using ANPaX.Export;
using ANPaX.FilmFormation;
using ANPaX.FilmFormation.interfaces;

using Caliburn.Micro;

namespace ANPaX.DesktopUI.ViewModels
{
    public class FilmFormationControlViewModel : Screen
    {
        private List<Aggregate> _aggregates;

        public IFilmFormationConfig FilmFormationConfig { get; set; }
        public AggFormationControlViewModel AggFormationControlViewModel { get; set; }

        private CancellationTokenSource _cts;

        public AggregateFilmFormationService AggregateFilmFormationService { get; set; }
        private StatusViewModel _statusViewModel;
        private LoggingViewModel _loggingViewModel;
        private FileExport _export = new FileExport();

        public SimulationProperties SimProp { get; set; }
        public IParticleFilm<Aggregate> ParticleFilm { get; set; }

        public bool DoGenerateAggregates { get; set; } = true;

        public FilmFormationControlViewModel(
            IFilmFormationConfig filmFormationConfig,
            SimulationProperties simProp,
            AggFormationControlViewModel aggFormationControlViewModel,
            StatusViewModel statusViewModel,
            LoggingViewModel loggingViewModel)
        {
            FilmFormationConfig = filmFormationConfig;
            SimProp = simProp;
            AggFormationControlViewModel = aggFormationControlViewModel;
            _statusViewModel = statusViewModel;
            _loggingViewModel = loggingViewModel;
        }

        public void GetAggregatesFromGeneration()
        {
            _aggregates = new List<Aggregate>();
            _aggregates.AddRange(AggFormationControlViewModel.GeneratedAggregates);
        }

        public async Task BuildParticleFilm()
        {
            if (DoGenerateAggregates)
            {
                await AggFormationControlViewModel.GenerateAggregates(null);
                GetAggregatesFromGeneration();
            }

            var progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += _statusViewModel.ReportFilmProgress;

            _loggingViewModel.LogInfo("Initiating film generation.");
            _statusViewModel.SimulationStatus = SimulationStatus.Running;

            _cts = new CancellationTokenSource();

            AggregateFilmFormationService = new AggregateFilmFormationService(FilmFormationConfig);
            ParticleFilm = await AggregateFilmFormationService.BuildFilm_Async(_aggregates, progress, _cts.Token);
            ExportFilm();
            _loggingViewModel.LogInfo("Film generation complete.");
            _statusViewModel.SimulationStatus = SimulationStatus.Idle;
        }

        public void ExportFilm()
        {
            var filename = "particleFilm.trj";
            var fileNameWithPath = Path.Join(SimProp.SimulationPath, filename);
            _loggingViewModel.LogInfo($"Exported particle film: {fileNameWithPath}");
            _export.Export(
                ParticleFilm,
                fileNameWithPath, FileFormat.LammpsDump, false);
        }

    }
}
