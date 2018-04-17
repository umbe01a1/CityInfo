using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public List<CityDTO> Cities { get; set; }
        
        public CitiesDataStore() {

            Cities = new List<CityDTO>() {
                new CityDTO() {
                    Id = 1,
                    Name = "New York City",
                    Description = "The one with that big park, hey now!",
                    PointsofInterest = new List<PointOfInterestDTO>() {
                        new PointOfInterestDTO() {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited urban park in the United States."
                        },
                        new PointOfInterestDTO() {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "A 102-story skyscraper located in Midtown Manhatten."
                        }
                    }
                },
                new CityDTO() {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never really finished.",
                    PointsofInterest = new List<PointOfInterestDTO>() {
                        new PointOfInterestDTO() {
                            Id = 3,
                            Name = "Some Cathedral",
                            Description = "They ran out of money!"
                        },
                        new PointOfInterestDTO() {
                            Id = 4,
                            Name = "Antwerp Zoo",
                            Description = "Oldest animal park in the counry and one of the oldest in the world.."
                        }
                    }
                },
                new CityDTO() {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with that big tower.",
                    PointsofInterest = new List<PointOfInterestDTO>() {
                        new PointOfInterestDTO() {
                            Id = 5,
                            Name = "Eiffle Tower",
                            Description = "Wrought iron lattice tower on the Champ de Mars in Paris."
                        },
                        new PointOfInterestDTO() {
                            Id = 6,
                            Name = "Nothign else",
                            Description = "Please move on."
                        }
                    }
                }
            };
        }
    }
}
