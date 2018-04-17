using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public static class CityInfoExtensions
    {
        public static void EnsureSeedDataForContext(this CityInfoContext context) {

            if (context.Cities.Any())
                return;


            var cities = new List<City>() {
                new City() {
                    Name = "New York City",
                    Description = "The one with that big park, hey now!",
                    PointsOfInterest = new List<PointOfInterest>() {
                        new PointOfInterest() {
                            Name = "Central Park",
                            Description = "The most visited urban park in the United States."
                        },
                        new PointOfInterest() {
                            Name = "Empire State Building",
                            Description = "A 102-story skyscraper located in Midtown Manhatten."
                        }
                    }
                },
                new City() {
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never really finished.",
                    PointsOfInterest = new List<PointOfInterest>() {
                        new PointOfInterest() {
                            Name = "Some Cathedral",
                            Description = "They ran out of money!"
                        },
                        new PointOfInterest() {
                            Name = "Antwerp Zoo",
                            Description = "Oldest animal park in the counry and one of the oldest in the world.."
                        }
                    }
                },
                new City() {
                    Name = "Paris",
                    Description = "The one with that big tower.",
                    PointsOfInterest = new List<PointOfInterest>() {
                        new PointOfInterest() {
                            Name = "Eiffle Tower",
                            Description = "Wrought iron lattice tower on the Champ de Mars in Paris."
                        },
                        new PointOfInterest() {
                            Name = "Nothign else",
                            Description = "Please move on."
                        }
                    }
                }
            };

            context.Cities.AddRange(cities);            
            context.SaveChanges();
            
        }        
    }
}
