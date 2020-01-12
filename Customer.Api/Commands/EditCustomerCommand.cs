using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Customer.Api.Exceptions;
using MediatR;

namespace Customer.Api.Commands
{
    public class EditCustomerCommand : IRequest
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class EditCustomerHandler : AsyncRequestHandler<EditCustomerCommand>
    {
        private readonly CustomerContext _context;


        public EditCustomerHandler(CustomerContext context)
        {
            _context = context;
        }
        protected override async Task Handle(EditCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.FindAsync(request.Id);

            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.DateOfBirth = request.DateOfBirth;

            await _context.SaveChangesAsync();
        }
    }
}
