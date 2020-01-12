using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Customer.Api.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Commands
{
    public class DeleteCustomerCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteCustomerHandler : AsyncRequestHandler<DeleteCustomerCommand>
    {
        private readonly CustomerContext _context;


        public DeleteCustomerHandler(CustomerContext context)
        {
            _context = context;
        }
        protected override async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
        }
    }
}
