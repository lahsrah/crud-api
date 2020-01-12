using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Customer.Api.Commands
{
    public class CreateCustomerCommand : IRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class CreateCustomerHandler : AsyncRequestHandler<CreateCustomerCommand>
    {
        private readonly CustomerContext _context;


        public CreateCustomerHandler(CustomerContext context)
        {
            _context = context;
        }
        protected override async Task Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = new Model.Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth
            };

           _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }
    }
}
