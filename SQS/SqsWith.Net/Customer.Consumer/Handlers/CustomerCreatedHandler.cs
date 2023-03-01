﻿using Customer.Consumer.Messages;
using MediatR;

namespace Customer.Consumer.Handlers
{
    public class CustomerCreatedHandler : IRequestHandler<CustomerCreated>
    {
        private readonly ILogger<CustomerCreatedHandler> _logger;

        public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CustomerCreated request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(request.FullName);
            return Unit.Task;
        }
    }
}
