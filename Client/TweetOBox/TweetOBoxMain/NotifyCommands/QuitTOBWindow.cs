using System.Windows;
using System.Windows.Input;

namespace TweetOBoxMain.NotifyCommands
{
  /// <summary>
  /// Hides the main window.
  /// </summary>
  public class QuitTOBWindow : CommandBase<QuitTOBWindow>
  {

    public override void Execute(object parameter)
    {
       // ClientKUI.Instance.TOBStop();
        Application.Current.Shutdown();        
        CommandManager.InvalidateRequerySuggested();
    }


    public override bool CanExecute(object parameter)
    {    
        return true;
    }


  }
}
