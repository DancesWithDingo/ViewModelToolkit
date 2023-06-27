using Microsoft.Maui.Handlers;
using ViewModelToolkit.Dialogs;

#if IOS || MACCATALYST
using ContentView = Microsoft.Maui.Platform.ContentView;
#endif

namespace ViewModelToolkit;

#if IOS || MACCATALYST
public class ModalPageHandler : PageHandler
{
    protected override ContentView CreatePlatformView() {
        _ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a LayoutView");
        _ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} cannot be null");

        ViewController ??= new ModalPageViewController(VirtualView, MauiContext);

        if ( ViewController is Microsoft.Maui.Platform.PageViewController pc && pc.CurrentPlatformView is ContentView pv )
            return pv;

        if ( ViewController.View is ContentView cv )
            return cv;

        throw new InvalidOperationException($"PageViewController.View must be of type {nameof(ContentView)}");
    }

    class ModalPageViewController : Microsoft.Maui.Platform.PageViewController
    {
        public ModalPageViewController(IView page, IMauiContext mauiContext) : base(page, mauiContext) { }

        ContentPage Page => CurrentView as ContentPage;

        public override void ViewWillAppear(bool animated) {
            base.ViewWillAppear(animated);
            if ( Page.BindingContext is IDialogSupport )
                ManageBarButtons();
        }

        void ManageBarButtons() {
            var navItem = NavigationController.TopViewController.NavigationItem;

            var tbi = Page.ToolbarItems.FirstOrDefault(tbi => tbi is CancelToolbarItem);
            if ( tbi is not null ) {
                Page.ToolbarItems.Remove(tbi);

                var btn = new UIKit.UIBarButtonItem(
                    tbi.Text,
                    UIKit.UIBarButtonItemStyle.Plain,
                    (s, e) => tbi.Command?.Execute(tbi.CommandParameter)
                );

                navItem.LeftBarButtonItem = btn;
            }
        }
    }
}

#else

public class ModalPageHandler : PageHandler { }

#endif
