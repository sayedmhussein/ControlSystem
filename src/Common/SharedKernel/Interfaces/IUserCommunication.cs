namespace WeeControl.Common.SharedKernel.Interfaces;

public interface IUserCommunication
{
    public string ServerBaseAddress { get; }
    
    public HttpClient HttpClient { get; }
    
}