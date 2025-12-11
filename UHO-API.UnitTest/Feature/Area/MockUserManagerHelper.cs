using Microsoft.AspNetCore.Identity;
using Moq;

namespace UHO_API.UnitTest.Feature.Area;

public static class MockUserManagerHelper
{
    public static Mock<UserManager<TUser>> CreateMockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var emailStore = new Mock<IUserEmailStore<TUser>>();
        store.As<IUserEmailStore<TUser>>().Setup(x => x.GetNormalizedEmailAsync(It.IsAny<TUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TUser user, CancellationToken ct) => user.ToString()!.ToUpperInvariant());
            
        var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
        mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            
        return mgr;
    }
}
