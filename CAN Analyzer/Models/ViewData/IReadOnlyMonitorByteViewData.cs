using System.ComponentModel;

namespace CANAnalyzer.Models.ViewData
{
    public interface IReadOnlyMonitorByteViewData : INotifyPropertyChanged
    {
        bool IsChanged { get; }
        byte Data { get; }
    }
}
