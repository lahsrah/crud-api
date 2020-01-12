using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customer.Api.Commands;
using Customer.Api.Exceptions;
using Customer.Api.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerContext _context;
        private readonly IMediator _mediator;

        public CustomersController(CustomerContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string search = null)
        {
            var customers = await _mediator.Send(new CustomerQuery { SearchText = search });

            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, EditCustomerCommand command)
        {
            command.Id = id;

            try
            {
                await _mediator.Send(command);

            }
            catch (CustomerNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteCustomerCommand { Id = id });
            }
            catch (CustomerNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
