using ContactDataAccessLayer;
using ContactAPIBusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ContactAPi.Controllers
{
    [Route("api/Countries")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllCountries")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        //here we used CountryDTO
        public ActionResult<IEnumerable<CountryDTO>> GetAllCountries() // Define a method to get all Countrys.
        {

            List<CountryDTO> CountrysList = ContactAPIBusinessLayer.Country.GetAllCountries();
            if (CountrysList.Count == 0)
            {
                return NotFound("No Countrys Found!");
            }

            return Ok(CountrysList); // Returns the list of Countrys.


        }




        [HttpGet("{id}", Name = "GetCountryById")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<CountryDTO> GetCountryById(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid Country Id");
            }

            var Country = ContactAPIBusinessLayer.Country.Find(id);

            if (Country == null)
            {
                return NotFound("Country with Id : " + id + " not Found.");
            }

            CountryDTO CDTO = Country.CDTO;

            return Ok(CDTO);
        }
        [HttpGet("countryName/{name}", Name = "GetCountryByName")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<CountryDTO> GetCountryByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Invalid Country Name");
            }

            var Country = ContactAPIBusinessLayer.Country.Find(name);

            if (Country == null)
            {
                return NotFound("Country with Name : " + name + " not Found.");
            }

            CountryDTO CDTO = Country.CDTO;

            return Ok(CDTO);
        }

        //Feature : Implement 'GetCountryByName(string countryName)'



        //for add new we use Http Post

        [HttpPost(Name = "AddCountry")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CountryDTO> AddCountry(CountryDTO newCountryDTO)
        {
            if (string.IsNullOrEmpty(newCountryDTO.CountryName) || string.IsNullOrEmpty(newCountryDTO.Code) || string.IsNullOrEmpty(newCountryDTO.PhoneCode))
            {
                return BadRequest("Invalid Country Data");
            }

            ContactAPIBusinessLayer.Country country = new ContactAPIBusinessLayer.Country(new CountryDTO(newCountryDTO.CountryID, newCountryDTO.CountryName, newCountryDTO.Code, newCountryDTO.PhoneCode));

            country.Save();
            newCountryDTO.CountryID = country.CountryID;

            return CreatedAtRoute("GetCountryById", new { id = newCountryDTO.CountryID }, newCountryDTO);
        }



        [HttpPut("{id}", Name = "UpdateCountry")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<CountryDTO> UpdateCountry(int id, CountryDTO CDTO)
        {
            if (id < 1 || string.IsNullOrEmpty(CDTO.CountryName) || string.IsNullOrEmpty(CDTO.Code) || string.IsNullOrEmpty(CDTO.PhoneCode))
            {
                return BadRequest("Invalid Country data");
            }

            var Country = ContactAPIBusinessLayer.Country.Find(id);
            if (Country == null)
            {
                return NotFound("Country with id : " + id + " not Found");
            }

            Country.CountryName = CDTO.CountryName;
            Country.Code = CDTO.Code;
            Country.PhoneCode = CDTO.PhoneCode;

            Country.Save();

            return Ok(Country.CDTO);
        }





        [HttpDelete("{id}", Name = "DeleteCountry")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult DeleteCountry(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid id");
            }

            if (ContactAPIBusinessLayer.Country.DeleteCountry(id))
            {

                return Ok("Country with : " + id + " deleted successfuly");
            }

            else
            {
                return NotFound("Not Found");
            }
        }


    }
}
