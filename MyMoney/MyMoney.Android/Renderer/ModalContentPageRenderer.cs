using Android.Content;
using MyMoney.Controls;
using MyMoney.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ModalContentPage), typeof(ModalContentPageRenderer))]
namespace MyMoney.Droid.Renderer
{
    public class ModalContentPageRenderer : PageRenderer
    {
        public ModalContentPageRenderer(Context context) : base(context)
        {
        }
    }
}