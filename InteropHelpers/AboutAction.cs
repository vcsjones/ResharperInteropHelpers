using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace InteropHelpers
{
  [ActionHandler("InteropHelpers.About")]
  public class AboutAction : IActionHandler
  {
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      // return true or false to enable/disable this action
      return true;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      MessageBox.Show(
        "Interop Assistance\nKevin Jones\n\nProvides assistance for Interop",
        "About Interop Assistance",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }
  }
}
