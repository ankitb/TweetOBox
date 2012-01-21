using System.Windows;
using System.Windows.Input;

namespace TweetOBoxMain.NotifyCommands
{
  /// <summary>
  /// Shows the main window.
  /// </summary>
  public class OpenTOBWindow : CommandBase<OpenTOBWindow>
  {
    public override void Execute(object parameter)
    {
      GetTaskbarWindow(parameter).Show();
      CommandManager.InvalidateRequerySuggested();
      GetTaskbarWindow(parameter).WindowState = WindowState.Normal;
      GetTaskbarWindow(parameter).Focus();
    }
      
    public override bool CanExecute(object parameter)
    {     
        return true;
    }
  }
}