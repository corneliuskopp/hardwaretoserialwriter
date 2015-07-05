namespace HardwareToSerialWriter.WPF
{
    public interface ISplashScreen
    {
        void AddMessage(string message);
        void LoadComplete();
    }
}