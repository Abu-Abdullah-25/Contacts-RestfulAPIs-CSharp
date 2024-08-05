using ContactDataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ContactAPi.Controllers
{
    [Route("api/Contacts")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllContacts")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        //here we used ContactDTO
        public ActionResult<IEnumerable<ContactDTO>> GetAllContacts() // Define a method to get all Contacts.
        {

            List<ContactDTO> ContactsList = ContactAPIBusinessLayer.Contact.GetAllContacts();

            if (ContactsList.Count == 0)
            {
                return NotFound("No Contacts Found!");
            }

            return Ok(ContactsList); // Returns the list of Contacts.


        }




        [HttpGet("{id}", Name = "GetContactById")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<ContactDTO> GetContactById(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid Contact Id");
            }

            var Contact = ContactAPIBusinessLayer.Contact.Find(id);

            if (Contact == null)
            {
                return NotFound("Contact with Id : " + id + " not Found.");
            }

            ContactDTO CDTO = Contact.CDTO;

            return Ok(CDTO);
        }


        //Feature : Implement 'GetContactByName(string ContactName)'
        [HttpGet("contactId/{id}", Name = "IsContactExist")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> IsContactExist(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid Contact ID");
            }

            try
            {
                bool isExist = ContactAPIBusinessLayer.Contact.isContactExist(id);

                if (!isExist)
                {
                    return NotFound("Contact with ID: " + id + " not found.");
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }


        [HttpPost(Name = "AddContact")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ContactDTO> AddContact(ContactDTO newContactDTO)
        {
            if (
                string.IsNullOrEmpty(newContactDTO.FirstName) ||
                string.IsNullOrEmpty(newContactDTO.LastName) ||
                string.IsNullOrEmpty(newContactDTO.Phone) ||
                string.IsNullOrEmpty(newContactDTO.Address) ||
                string.IsNullOrEmpty(newContactDTO.Email) ||
                newContactDTO.DateOfBirth == default(DateTime)
                )
            {
                return BadRequest("Invalid Contact Data");
            }

            ContactAPIBusinessLayer.Contact contact = new ContactAPIBusinessLayer.Contact(new ContactDTO(newContactDTO.ContactID,
                                                            newContactDTO.FirstName,newContactDTO.LastName,newContactDTO.Email,
                                                            newContactDTO.Phone,newContactDTO.Address,newContactDTO.DateOfBirth,
                                                             newContactDTO.CountryID,newContactDTO.ImagePath));

            // Assuming `Save` method saves the contact to the database
            contact.Save();

            newContactDTO.ContactID = contact.ContactID;

            return CreatedAtRoute("GetContactById", new { id = newContactDTO.ContactID }, newContactDTO);
        }



        [HttpPut("{id}", Name = "UpdateContact")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<CountryDTO> UpdateContact(int id, ContactDTO newContactDTO)
        {
            if (id < 1 ||
                string.IsNullOrEmpty(newContactDTO.FirstName) ||
                string.IsNullOrEmpty(newContactDTO.LastName) ||
                string.IsNullOrEmpty(newContactDTO.Phone) ||
                string.IsNullOrEmpty(newContactDTO.Address) ||
                string.IsNullOrEmpty(newContactDTO.Email))
            {
                return BadRequest("Invalid Contact Data");
            }

            var contact = ContactAPIBusinessLayer.Contact.Find(id);
            if (contact == null)
            {
                return NotFound("Contact with id : " + id + " not Found");
            }

            contact.FirstName = newContactDTO.FirstName;
            contact.LastName = newContactDTO.LastName;
            contact.Email = newContactDTO.Email;
            contact.Phone = newContactDTO.Phone;
            contact.Address = newContactDTO.Address;
            contact.DateOfBirth = newContactDTO.DateOfBirth;
            contact.CountryID = newContactDTO.CountryID;
            contact.ImagePath = newContactDTO.ImagePath;


            contact.Save();

            return Ok(contact.CDTO);
        }






        [HttpDelete("{id}", Name = "DeleteContact")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult DeleteContact(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid id");
            }

            if (ContactAPIBusinessLayer.Contact.DeleteContact(id))
            {

                return Ok("Contact with : " + id + " deleted successfuly");
            }

            else
            {
                return NotFound("Not Found");
            }
        }
    }

}
