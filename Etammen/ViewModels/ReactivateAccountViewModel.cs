using DataAccessLayerEF.Models;

namespace Etammen.ViewModels;

public class ReactivateAccountViewModel
{
    public string ApplicationUserId { get; set; }
    public string Username { get; set; }
    public string ProfilePic { get; set; }
    public string Email { get; set; }
}
