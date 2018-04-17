using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Services;
using AutoMapper;

namespace CityInfo.API.Controllers {

    [Route("api/cities")]
    public class PointsOfInterestController : Controller {

        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository ) {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }


        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId) {

            try {
                if (!_cityInfoRepository.CityExists(cityId)) {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();                    
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                var pointsOfInterestForCityResults = Mapper.Map<IEnumerable<PointOfInterestDTO>>(pointsOfInterestForCity);
                return Ok(pointsOfInterestForCityResults);

                //if (city == null) {
                //    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                //    return NotFound(); 
                //}
                
                //return Ok(city.PointsofInterest);
            }
            catch (Exception ex) {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}. \r\n", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }

        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id) {

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterest == null)
                return NotFound();

            var result = Mapper.Map<PointOfInterestDTO>(pointOfInterest);

            return Ok(result);

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //    return NotFound(); //If city is not found return a not found.  No city thus no points of interest.

            //var pointOfInterest = city.PointsofInterest.FirstOrDefault(p => p.Id == id);

            //if (pointOfInterest == null)
            //    return NotFound();

            //return Ok(pointOfInterest);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDTO pointOfInterest) {
            //JeremySkinner / FluentValidation

            if (pointOfInterest == null)
                return BadRequest();

            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("Description", "The provided descriptoin should be different from the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var finalPOI = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPOI);

            if (!_cityInfoRepository.Save()) {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            var createdPointOfInterestToReturn = Mapper.Map<Models.PointOfInterestDTO>(finalPOI);

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = createdPointOfInterestToReturn.Id }, createdPointOfInterestToReturn);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForCreationDTO pointOfInterest) {
            if (pointOfInterest == null)
                return BadRequest();

            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("Description", "The provided descriptoin should be different from the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound(); //If city is not found return a not found.  No city thus no points of interest.

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();

            Mapper.Map(pointOfInterest, pointOfInterestEntity);

            if (!_cityInfoRepository.Save()) {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDTO> patchDoc) {
            if (patchDoc == null)
                return BadRequest("bad patch");

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();
            
            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
           
            if (pointOfInterestEntity == null)
                return NotFound();


            var pointOfInterestToPatch = Mapper.Map<PointOfInterestForUpdateDTO>(pointOfInterestEntity);
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
                ModelState.AddModelError("Description", "The provided descriptoin should be different from the name.");

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            if (!_cityInfoRepository.Save()) {
                return StatusCode(500, "A problem happened while handling your request.");
            }
            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest( int cityId, int id) {

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var poi = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            
            if (poi == null)
                return NotFound();

            _cityInfoRepository.DeletePointOfInterest(poi);

            if (!_cityInfoRepository.Save()) {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            _mailService.Send("Point of interest deleted.", $"Point of interest {poi.Name} with id {poi.Id} was deleted.");

            return NoContent();
        }
    }
}
