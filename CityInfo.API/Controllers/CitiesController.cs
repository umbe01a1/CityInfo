﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Services;
using CityInfo.API.Models;
using AutoMapper;

namespace CityInfo.API.Controllers {
    [Route("api/cities")]
    public class CitiesController : Controller {

        private ICityInfoRepository _cityInfoRepository;

        public CitiesController( ICityInfoRepository cityInfoRepository) {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult GetCities() {
            //return Ok(CitiesDataStore.Current.Cities);

            var cityEntities = _cityInfoRepository.GetCities();

            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDTO>>(cityEntities);
            
            return Ok(results);

        }

        [HttpGet("{id}")]
        public IActionResult GetCity( int id, bool includePointsOfInterest = false) {

            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (city == null)
                return NotFound();

            if (includePointsOfInterest) {
                var cityResult = Mapper.Map<CityDTO>(city);
                return Ok(cityResult);
            }

            var cityWithoutPointsOfInterestResult = Mapper.Map<CityWithoutPointsOfInterestDTO>(city);

            return Ok(cityWithoutPointsOfInterestResult);
            
            //var CityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == id);

            //if (CityToReturn == null)
            //    return NotFound();

            //return Ok(CityToReturn);
        }
    }
}
