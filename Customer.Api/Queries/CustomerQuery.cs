using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customer.Api.Dto;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Queries
{
    public class CustomerQuery : IRequest<IEnumerable<CustomerDto>>
    {
        public string SearchText { get; set; }
    }


    public class CustomerQueryHandler : IRequestHandler<CustomerQuery, IEnumerable<CustomerDto>>
    {
        private readonly CustomerContext _dbContext;


        public CustomerQueryHandler(CustomerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CustomerDto>> Handle(CustomerQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Customers
                .Where(x => string.IsNullOrEmpty(request.SearchText) ||
                            x.FirstName.Contains(request.SearchText) ||
                            x.LastName.Contains(request.SearchText))
                .Select(x => new CustomerDto
                {
                    Id =  x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DateOfBirth = x.DateOfBirth
                })
                .ToListAsync(cancellationToken);
        }
    }
}
