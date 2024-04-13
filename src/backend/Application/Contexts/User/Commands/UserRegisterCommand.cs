﻿using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeeControl.Core.Application.Exceptions;
using WeeControl.Core.Application.Interfaces;
using WeeControl.Core.DataTransferObject.Temporary.Entities;
using WeeControl.Core.DataTransferObject.Temporary.User;
using WeeControl.Core.DataTransferObject.User;
using ValidationException = WeeControl.Core.Application.Exceptions.ValidationException;

namespace WeeControl.Core.Application.Contexts.User.Commands;

public class UserRegisterCommand : IRequest<IResponseDto<TokenResponseDto>>
{
    private readonly IRequestDto request;
    private readonly PersonalEntity person;
    private readonly UserEntity user;
    private readonly EmployeeEntity employee;
    private readonly CustomerEntity customer;

    public UserRegisterCommand(IRequestDto<RegisterCustomerDto> dto)
    {
        request = dto;
        person = dto.Payload.Personal;
        user = dto.Payload.User;
        customer = dto.Payload.Customer;
    }

    public UserRegisterCommand(IRequestDto<RegisterEmployeeDto> dto)
    {
        request = dto;
        person = dto.Payload.Personal;
        user = dto.Payload.User;
        employee = dto.Payload.Employee;
    }

    public class RegisterHandler : IRequestHandler<UserRegisterCommand, IResponseDto<TokenResponseDto>>
    {
        private readonly IEssentialDbContext context;
        private readonly IMediator mediator;
        private readonly IPasswordSecurity passwordSecurity;

        public RegisterHandler(IEssentialDbContext context, IMediator mediator, IPasswordSecurity passwordSecurity)
        {
            this.context = context;
            this.mediator = mediator;
            this.passwordSecurity = passwordSecurity;
        }

        public async Task<IResponseDto<TokenResponseDto>> Handle(UserRegisterCommand cmd, CancellationToken cancellationToken)
        {
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(
                cmd.person,
                new ValidationContext(cmd.person),
                validationResults,
                true);
            if (result == false)
            {
                throw new ValidationException(validationResults);
            }

            if (context.Users.Any(x =>
                    x.Username == cmd.user.Username.ToLower() ||
                    x.Email == cmd.user.Email.ToLower() ||
                    x.MobileNo == cmd.user.MobileNo.ToLower()
                ))
            {
                throw new ConflictFailureException();
            }

            var user = new UserDbo(cmd.user)
            {
                Password = passwordSecurity.Hash(cmd.user.Password)
            };

            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var request =
                new SessionCreateCommand(RequestDto.Create(LoginRequestDto.Create(user.Username, cmd.user.Password),
                    cmd.request));
            var response = await mediator.Send(request, cancellationToken);
            return response;
        }
    }
}