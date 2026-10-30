using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Efakturon.Domain.Entities;

namespace Efakturon.Web.Components.Shared;

public class SignOutStateManager
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SignOutStateManager(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task SetSignOutState()
    {
        await _signInManager.SignOutAsync();
    }
}
