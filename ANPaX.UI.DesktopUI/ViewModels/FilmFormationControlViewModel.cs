﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ANPaX.Analysis.Validation;
using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.DesktopUI.Models;
using ANPaX.IO;

using ANPaX.Simulation.FilmFormation;
using ANPaX.Simulation.FilmFormation.interfaces;
using ANPaX.UI.DesktopUI.Models;
using ANPaX.UI.DesktopUI.Views;

using Caliburn.Micro;

namespace ANPaX.UI.DesktopUI.ViewModels
{
    public class FilmFormationControlViewModel : Conductor<object>
    {
        private List<Aggregate> _aggregates;

        public IFilmFormationConfig FilmFormationConfig { get; set; }
        public FilmFormationConfigViewModel FilmFormationConfigViewModel { get; set; }
        public AggFormationControlViewModel AggFormationControlViewModel { get; set; }

        private CancellationTokenSource _cts;

        public FilmFormationService FilmFormationService { get; set; }
        private StatusViewModel _statusViewModel;
        private LoggingViewModel _loggingViewModel;
        private FileExport _export = new FileExport();
        private FileImport _import = new FileImport();

        public string ParticleFilmFileName { get; set; } = "ParticleFilm.trj";

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
            FilmFormationConfigViewModel = new FilmFormationConfigViewModel(FilmFormationConfig);
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

            FilmFormationService = new FilmFormationService(FilmFormationConfig);
            ParticleFilm = await Task.Run(() => FilmFormationService.BuildFilm_Async(_aggregates, progress, FilmFormationConfig.Delta, _cts.Token));
            _loggingViewModel.LogInfo("Film generation complete.");
            ExportFilm();
            _statusViewModel.SimulationStatus = SimulationStatus.Idle;

        }

        public async Task CheckForOverlaps()
        {
            _loggingViewModel.LogInfo("Searching for overlapping primary particles");
            var count = await ParticleFilmOverlappingAnalyser.GetNumberOfOverlappingPrimaryParticles_Async(ParticleFilm, true);
            // every primary particle was counted twice
            _loggingViewModel.LogInfo($"Found {count / 2} overlappings.");
            ExportFilm("particleFilm_overlapping.trj");
        }

        public async Task ImportAggregatesFromFile()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();

            var filename = dialog.FileName;
            if (result != DialogResult.OK)
            {
                return;
            }

            _loggingViewModel.LogInfo($"Importing aggregates from {filename}.");
            try
            {
                _aggregates = await Task.Run(() => _import.GetAggregatsFromFile(filename).ToList());
                _loggingViewModel.LogInfo($"Finished file import.");
            }
            catch (AggregateException e)
            {
                _loggingViewModel.LogWarning($"Error during file import: {e.InnerExceptions}");
            }
            DoGenerateAggregates = false;
        }

        public void ExportFilm(string filename)
        {
            var fileNameWithPath = Path.Join(SimProp.SimulationPath, filename);

            _loggingViewModel.LogInfo($"Exporting particle film:{Environment.NewLine}{fileNameWithPath}");

            _export.Export(
                ParticleFilm,
                fileNameWithPath, FileFormat.LammpsDump, false);

            _loggingViewModel.LogInfo("done.");
        }

        public void ExportFilm()
        {
            var filename = "particleFilm.trj";
            ExportFilm(filename);
        }

        #region ViewVisibility

        public void ShowFilmFormationConfigView(FilmFormationControlView view)
        {
            SwitchToHideFilmFormationConfigButton(view);
            ActivateItem(FilmFormationConfigViewModel);
        }

        public void HideFilmFormationConfigView(FilmFormationControlView view)
        {
            SwitchToShowFilmFormationConfigButton(view);
            DeactivateItem(FilmFormationConfigViewModel, false);
        }
        #endregion

        #region ButtonVisibility
        private void SwitchToShowFilmFormationConfigButton(FilmFormationControlView view)
        {
            view.HideFilmFormationConfigButton.Visibility = System.Windows.Visibility.Hidden;
            view.ShowFilmFormationConfigButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void SwitchToHideFilmFormationConfigButton(FilmFormationControlView view)
        {
            view.HideFilmFormationConfigButton.Visibility = System.Windows.Visibility.Visible;
            view.ShowFilmFormationConfigButton.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion
    }
}
