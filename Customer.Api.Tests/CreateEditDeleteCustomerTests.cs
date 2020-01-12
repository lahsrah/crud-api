using System;
using System.Linq;
using Customer.Api.Commands;
using Customer.Api.Exceptions;
using Customer.Api.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Customer.Api.Tests
{

    /// <summary>
    /// This contains some example tests for Create/Edit/Delete/Query operations on customers.  In a production app I would add
    /// more tests for each scenario.
    /// </summary>
    public class CreateEditDeleteCustomerTests
    {
        private CustomerContext _dbContext = null;


        public CreateEditDeleteCustomerTests()
        {
            var options = new DbContextOptionsBuilder<CustomerContext>().UseInMemoryDatabase(databaseName: "CustomerDb").Options;
            _dbContext = new CustomerContext(options);
            _dbContext.Database.EnsureDeleted();

            var customers = new[]
            {
                new Model.Customer {
                    Id =  new Guid("10000000-0000-0000-0000-000000000000"),
                    FirstName = "Jane",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(2000,1,1)
                },
                new Model.Customer {
                    Id = new Guid("20000000-0000-0000-0000-000000000000"),
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(2001,1,1)

                },
                new Model.Customer {
                    Id = new Guid("30000000-0000-0000-0000-000000000000"),
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    DateOfBirth = new DateTime(1985,10,15)
                }
            }.ToList();

            _dbContext.Customers.AddRange(customers);
            _dbContext.SaveChanges();
        }

        [Fact]
        public void CustomerQueryTest_SearchTermNotMatchingAnyCustomer_ReturnsNoCustomers()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);

            var command = new CustomerQuery
            {
                SearchText = "41234132413414"
            };

            var result = mediator.Send(command).GetAwaiter().GetResult();

            Assert.Empty(result);
        }

        [Fact]
        public void CustomerQueryTest_EmptySearch_ReturnsAllCustomers()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);

            var command = new CustomerQuery
            {
               SearchText = ""
            };

            var result = mediator.Send(command).GetAwaiter().GetResult();
            
            Assert.Equal(3, result.Count());
        }   
        
        [Fact]
        public void CustomerQueryTest_FirstNameQuery_SearchReturnsMatchingCustomers()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);


            var command = new CustomerQuery
            {
                SearchText = "John"
            };

            var result = mediator.Send(command).GetAwaiter().GetResult();

            Assert.Single(result);
        }


        [Fact]
        public void CustomerQueryTest_PartialFirstNameQuery_SearchReturnsMatchingCustomers()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);


            var command = new CustomerQuery
            {
                SearchText = "Jo"
            };

            var result = mediator.Send(command).GetAwaiter().GetResult();

            Assert.Equal(2, result.Count());

        }

        [Fact]
        public void CustomerQueryTest_PartialLastNameQuery_SearchReturnsMatchingCustomers()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);


            var command = new CustomerQuery
            {
                SearchText = "Blog"
            };

            var result = mediator.Send(command).GetAwaiter().GetResult();
            Assert.Single(result);
        }


        [Fact]
        public void CustomerQueryTest_LastNameQuery_SearchReturnsMatchingCustomers()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);


            var command = new CustomerQuery
            {
                SearchText = "Doe"
            };

            var result = mediator.Send(command).GetAwaiter().GetResult();
            
            Assert.Equal(2, result.Count());

        }

        [Fact]
        public void CreateCustomerTest_CustomerShouldBeCreated()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);

            var c = _dbContext.Customers.ToList();

            var command = new CreateCustomerCommand
            {
                FirstName = "Tom",
                LastName = "Read",
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            mediator.Send(command).GetAwaiter().GetResult();
            Assert.Equal(4, _dbContext.Customers.Count());

        }

        [Fact]
        public void EditCustomerTest_FieldsShouldBeEdited()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);

            var command = new EditCustomerCommand
            {
                Id = new Guid("20000000-0000-0000-0000-000000000000"),
                FirstName = "John edited",
                LastName = "Doe edited",
                DateOfBirth = new DateTime(2000, 1, 2)
            };

            mediator.Send(command).GetAwaiter().GetResult();

            var johnEdited = _dbContext.Customers.First(x => x.Id == new Guid("20000000-0000-0000-0000-000000000000"));

            Assert.Equal("John edited", johnEdited.FirstName);
            Assert.Equal("Doe edited", johnEdited.LastName);
            Assert.Equal(new DateTime(2000, 1, 2), johnEdited.DateOfBirth);

        }

        [Fact]
        public void EditCustomerTest_InvalidId_ThrowsCustomerNotFoundException()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);

            var command = new EditCustomerCommand
            {
                Id = new Guid("55000000-0000-0000-0000-000000000000"),
                FirstName = "John edited",
                LastName = "Doe edited",
                DateOfBirth = new DateTime(2000, 1, 2)
            };

            Assert.Throws<CustomerNotFoundException>(() => mediator.Send(command).GetAwaiter().GetResult());

        }


        [Fact]
        public void DeleteCustomerTest_CustomerShouldBeDeleted()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);

            var c = _dbContext.Customers.ToList();

            var command = new DeleteCustomerCommand
            {
                Id = new Guid("10000000-0000-0000-0000-000000000000"),
            };

            mediator.Send(command).GetAwaiter().GetResult();
            Assert.Equal(2, _dbContext.Customers.Count());

        }

        [Fact]
        public void DeleteCustomerTest_InvalidId_ThrowsCustomerNotFoundException()
        {
            var mediator = TestMediatorFactory.BuildMediator(_dbContext);

            var command = new DeleteCustomerCommand
            {
                Id = new Guid("55000000-0000-0000-0000-000000000000")
            };

            Assert.Throws<CustomerNotFoundException>(() => mediator.Send(command).GetAwaiter().GetResult());

        }
    }
}
