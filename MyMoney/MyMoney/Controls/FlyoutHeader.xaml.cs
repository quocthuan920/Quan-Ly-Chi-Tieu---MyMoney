

namespace MyMoney.Controls
{
    public partial class FlyoutHeader 
    {

        public FlyoutHeader()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.BackupViewModel;
        }

       
    }
}