using System.Windows;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Controller menuController;
        public MainWindow()
        {
            InitializeComponent();
            this.menuController = new Controller(this);
        }
        
    }
}
