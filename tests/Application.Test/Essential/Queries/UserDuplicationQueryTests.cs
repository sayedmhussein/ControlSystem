using System.Threading.Tasks;
using MediatR;
using WeeControl.Application.Contexts.Essential.Queries;
using WeeControl.Application.Exceptions;
using WeeControl.Domain.Contexts.Essential;
using WeeControl.SharedKernel.Contexts.Essential.DataTransferObjects.User;
using Xunit;

namespace WeeControl.Application.Test.Essential.Queries;

public class UserDuplicationQueryTests
{
    [Fact]
    public async void TestForSuccess()
    {
        using var testHelper = new TestHelper();

        var handler = await GetHandler(testHelper);
        
        Assert.Equal(Unit.Value, await handler.Handle(new UserDuplicationQuery(nameof(RegisterCustomerDto.User.Username), "username1"), default));
        Assert.Equal(Unit.Value, await handler.Handle(new UserDuplicationQuery(nameof(RegisterCustomerDto.User.Email), "email@email.com"), default));
        Assert.Equal(Unit.Value, await handler.Handle(new UserDuplicationQuery(nameof(RegisterCustomerDto.User.MobileNo), "+33"), default));
    }
    
    [Theory]
    [InlineData("username", "", "")]
    [InlineData("", "user@email.com", "")]
    [InlineData("", "", "+2022")]
    public async void TestsForFailures(string username, string email, string mobileNo)
    {
        using var testHelper = new TestHelper();
        var handler = await GetHandler(testHelper);
        
        await Assert.ThrowsAsync<ConflictFailureException>(async () =>
        {
            await handler.Handle(
                new UserDuplicationQuery(nameof(RegisterCustomerDto.User.Username), username),
                default);
            await handler.Handle(
                new UserDuplicationQuery(nameof(RegisterCustomerDto.User.Email), email),
                default);
            await handler.Handle(
                new UserDuplicationQuery(nameof(RegisterCustomerDto.User.MobileNo), mobileNo),
                default);
        });
        
    }
    
    private async Task<UserDuplicationQuery.UserDuplicationHandler> GetHandler(TestHelper testHelper)
    {
        var user = testHelper.GetUserDboWithEncryptedPassword("username", "password");
        user.Email = "user@email.com";
        user.MobileNo = "+2022";
        await testHelper.EssentialDb.Users
            .AddAsync(user);
        await testHelper.EssentialDb.SaveChangesAsync(default);
        return new UserDuplicationQuery.UserDuplicationHandler(testHelper.EssentialDb);
    }
}