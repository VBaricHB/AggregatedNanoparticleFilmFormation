using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using ANPaX.Core;
using ANPaX.Core.interfaces;

namespace ANPaX.IO
{
    internal static class LammpsDumpDeserializer
    {

        public static IParticleFilm<Aggregate> DeserializeParticleFilm(string filename, INeighborslistFactory neighborslistFactory)
        {
            var fileContent = File.ReadAllLines(filename);
            var boxWidth = GetBoxWidth(fileContent);
            var simulationBox = new AbsoluteTetragonalSimulationBox(boxWidth);
            var particleFilm = new TetragonalAggregatedParticleFilm(simulationBox, neighborslistFactory)
            {
                Particles = GetAggregates(fileContent)
            };

            return particleFilm;
        }

        private static IList<Aggregate> GetAggregates(string[] fileContent)
        {
            var map = InitializeMapper(fileContent[8]);

            var aggregates = new List<Aggregate>();
            for (var i = 9; i < fileContent.Count(); i++)
            {
                AddParticleToAggregates(aggregates, fileContent[i], map);
            }


            return aggregates;
        }

        private static void AddParticleToAggregates(List<Aggregate> aggregates, string line, Dictionary<string, int> map)
        {
            var primaryParticleId = Convert.ToInt32(line[map["id"]]);
            var aggregateId = Convert.ToInt32(line[map["aggregate"]]);
            var clusterId = Convert.ToInt32(line[map["cluster"]]);
            var x = Convert.ToDouble(line[map["x"]], CultureInfo.InvariantCulture);
            var y = Convert.ToDouble(line[map["y"]], CultureInfo.InvariantCulture);
            var z = Convert.ToDouble(line[map["z"]], CultureInfo.InvariantCulture);
            var radius = Convert.ToDouble(line[map["Radius"]], CultureInfo.InvariantCulture);

            var position = new Vector3(x, y, z);
            var pp = new PrimaryParticle(primaryParticleId, position, radius);
            var clus = GetCluster(aggregates, aggregateId, clusterId);

            clus.PrimaryParticles.Add(pp);
        }

        private static Cluster GetCluster(List<Aggregate> aggregates, int aggregateId, int clusterId)
        {
            var aggregate = GetAggregate(aggregates, aggregateId);

            var cluster = aggregate.Cluster.FirstOrDefault(c => c.Id == clusterId);
            if (cluster == default)
            {
                cluster = new Cluster() { Id = clusterId };
                aggregate.Cluster.Add(cluster);
            }

            return cluster;
        }

        private static Aggregate GetAggregate(List<Aggregate> aggregates, int aggregateId)
        {
            var aggregate = aggregates.FirstOrDefault(a => a.Id == aggregateId);
            if (aggregate == default)
            {
                aggregate = new Aggregate() { Id = aggregateId };
                aggregate.Cluster = new List<Cluster>();
                aggregates.Add(aggregate);
            }


            return aggregate;
        }

        private static Dictionary<string, int> InitializeMapper(string mapperLine)
        {
            var map = new Dictionary<string, int>();
            for (var e = 0; e < mapperLine.Split(" ").Count(); e++)
            {
                if (mapperLine[e].ToString() == "id")
                {
                    map["id"] = e - 2;
                }
                else if (mapperLine[e].ToString() == "type")
                {
                    map["type"] = e - 2;
                }
                else if (mapperLine[e].ToString() == "aggregate")
                {
                    map["aggregate"] = e - 2;
                }
                else if (mapperLine[e].ToString() == "cluster")
                {
                    map["cluster"] = e - 2;
                }
                else if (mapperLine[e].ToString() == "x")
                {
                    map["x"] = e - 2;
                }
                else if (mapperLine[e].ToString() == "y")
                {
                    map["y"] = e - 2;
                }
                else if (mapperLine[e].ToString() == "z")
                {
                    map["z"] = e - 2;
                }
                else if (mapperLine[e].ToString() == "Radius")
                {
                    map["Radius"] = e - 2;
                }
            }

            return map;
        }

        private static double GetBoxWidth(string[] fileContent)
        {
            var widthLine = fileContent[5];
            var limits = widthLine.Split(' ').Select(w => Convert.ToDouble(w, CultureInfo.InvariantCulture)).ToList();
            return limits[1] - limits[0];
        }
    }
}
