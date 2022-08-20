using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WeeControl.Application.Contexts.Essential.Notifications;
using WeeControl.Application.Exceptions;
using WeeControl.Application.Interfaces;
using WeeControl.SharedKernel.Contexts.Essential.DataTransferObjects.User;
using WeeControl.SharedKernel.Interfaces;

namespace WeeControl.Application.Contexts.Essential.Commands;

public class UserForgotMyPasswordCommand : IRequest
{
    private readonly IRequestDto<UserPasswordResetRequestDto> dto;

    public UserForgotMyPasswordCommand(IRequestDto<UserPasswordResetRequestDto> dto)
    {
        this.dto = dto;
    }
    
    public class ForgotMyPasswordHandler : IRequestHandler<UserForgotMyPasswordCommand>
    {
        private readonly IEssentialDbContext context;
        private readonly IMediator mediator;
        private readonly IPasswordSecurity passwordSecurity;

        public ForgotMyPasswordHandler(IEssentialDbContext context, IMediator mediator, IPasswordSecurity passwordSecurity)
        {
            this.context = context;
            this.mediator = mediator;
            this.passwordSecurity = passwordSecurity;
        }
    
        public async Task<Unit> Handle(UserForgotMyPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.dto.DeviceId) ||
                string.IsNullOrWhiteSpace(request.dto.Payload.Email)||
                string.IsNullOrWhiteSpace(request.dto.Payload.Username))
            {
                throw new BadRequestException("Invalid device or email or username");
            }
        
            var user = await context.Users.FirstOrDefaultAsync(x => x.Username == request.dto.Payload.Username.ToLower() && x.Email == request.dto.Payload.Email.ToLower(), cancellationToken);
            if (user is not null)
            {
                if (string.IsNullOrEmpty(user.SuspendArgs) == false)
                    throw new NotAllowedException("Account is locked.");
                
                var password = passwordSecurity.GenerateRandomPassword();
                user.SetTemporaryPassword(passwordSecurity.Hash(password));
                await context.SaveChangesAsync(cancellationToken);
            
                await mediator.Publish(new PasswordResetNotification(user.UserId, password), cancellationToken);
            
                return Unit.Value;
            }
            
            return Unit.Value;
        }
    }
}