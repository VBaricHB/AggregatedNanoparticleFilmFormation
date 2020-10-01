using System;

namespace ANPaX.IO.DTO
{
    public class ParticleSimulationModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public string EMail { get; set; }
        public int PrimaryParticles { get; set; }
        public double XWidth { get; set; }
        public double YWidth { get; set; }
        public double ZWidth { get; set; }
        public DateTime Date { get; set; }
        public string SimulationType { get; set; }
        public string SimulationPath { get; set; }
        public int AggregateFormationConfigId { get; set; }
        public int FilmFormationConfigId { get; set; }
        public string Status { get; set; }
        public double Percentage { get; set; }
    }
}
