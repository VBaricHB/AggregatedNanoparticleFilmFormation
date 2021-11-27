using System;
using System.Collections.Generic;

using ANPaX.Simulation.FilmFormation;
using ANPaX.Simulation.FilmFormation.interfaces;
using ANPaX.UI.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.UI.DesktopUI.ViewModels
{
    public class FilmFormationConfigViewModel : Screen, IConfigViewModel
    {
        public string Header => "Film Formation Configuration";
        public IFilmFormationConfig Config { get; set; }

        private double _filmWidthAbsolute = 2000;

        public double FilmWidthAbsolute
        {
            get => _filmWidthAbsolute;
            set
            {
                if (value != _filmWidthAbsolute)
                {
                    _filmWidthAbsolute = value;
                    NotifyOfPropertyChange(() => FilmWidthAbsolute);
                    Config.XFilmWidthAbsolute = value;
                    Config.YFilmWidthAbsolute = value;
                }
            }
        }

        private int _maxCPU = Environment.ProcessorCount;

        public int MaxCPU
        {
            get => _maxCPU;
            set
            {
                if (_maxCPU != value)
                {
                    _maxCPU = value;
                    NotifyOfPropertyChange(() => MaxCPU);
                    Config.MaxCPU = value;
                }
            }
        }

        private double _delta = 1.01;

        public double Delta
        {
            get => _delta;
            set
            {
                if (_delta != value)
                {
                    _delta = value;
                    NotifyOfPropertyChange(() => Delta);
                    Config.Delta = value;
                }
            }
        }

        private string _wallCollisionType = "Periodic";
        public string WallCollisionType
        {
            get => _wallCollisionType;

            set
            {
                if (_wallCollisionType != value)
                {
                    _wallCollisionType = value;
                    NotifyOfPropertyChange(() => WallCollisionType);
                    switch (value)
                    {
                        case "Open":
                            Config.WallCollisionHandler = new OpenWallCollisionHandler();
                            break;
                        default:
                        case "Periodic":
                            Config.WallCollisionHandler = new PeriodicBoundaryCollisionHandler();
                            break;
                    }
                }
            }
        }

        public List<string> AvailableWallCollisionTypes { get; set; }
            = new List<string> { "Periodic", "Open" };

        public FilmFormationConfigViewModel(IFilmFormationConfig config)
        {
            Config = config;
        }
    }
}
