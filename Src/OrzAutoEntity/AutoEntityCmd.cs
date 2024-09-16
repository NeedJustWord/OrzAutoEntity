using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using OrzAutoEntity.EncodingProviders;
using OrzAutoEntity.Helpers;
using OrzAutoEntity.Views;
using Task = System.Threading.Tasks.Task;

namespace OrzAutoEntity
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AutoEntityCmd
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("d1fede31-143f-45de-a644-736d452ba4dc");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private readonly FrmBatch frmBatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoEntityCmd"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AutoEntityCmd(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            Cp936EncodingProvider.RegisterProvider();
            frmBatch = new FrmBatch();
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            if (sender is OleMenuCommand cmd)
            {
                cmd.Visible = ConfigHelper.HasConfigFile(DTEHelper.GetSelectedProjectFullPath());
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AutoEntityCmd Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in AutoEntityCmd's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AutoEntityCmd(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            var configPath = DTEHelper.GetSelectedProjectFullPath();
            //刚打开解决方案时扩展还没加载，命令都是可见，此时点击命令也会执行到这，所以此处需要判断配置文件是否存在
            if (ConfigHelper.HasConfigFile(configPath) == false)
            {
                MessageBox.Show($"{configPath}{ConfigHelper.ConfigFileName}配置文件不存在", "错误提示");
                return;
            }

            ConfigHelper.Init(configPath);
            frmBatch.Reset();
            frmBatch.ShowDialog();
        }
    }
}
