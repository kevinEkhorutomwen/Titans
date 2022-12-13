namespace Titans.Application.Query
{
    public interface IGetUserInformationApplicationService
    {
        string GetCurrentUserName();
        string GetCurrentRefreshToken();
    }
}
