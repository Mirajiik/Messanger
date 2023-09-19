using Messanger.ViewModels.Base;

namespace Messanger.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Название окна
        private string _TitleWindow = "Hello World!";
        public string TitleWindow
        {
            get => _TitleWindow;
            set => Set(ref _TitleWindow, value);
        } 
        #endregion
        

    }
}
