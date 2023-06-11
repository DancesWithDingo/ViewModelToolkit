using Microsoft.Maui.Handlers;
using ViewModelToolkit.Dialogs;

namespace ViewModelToolkit;

#if IOS || MACCATALYST
public class ModalPageHandler : PageHandler
{
    protected override Microsoft.Maui.Platform.ContentView CreatePlatformView() {
        _ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a LayoutView");
        _ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} cannot be null");

        if ( ViewController == null )
            ViewController = new ModalPageViewController(VirtualView, MauiContext);

        if ( ViewController is Microsoft.Maui.Platform.PageViewController pc && pc.CurrentPlatformView is Microsoft.Maui.Platform.ContentView pv )
            return pv;

        if ( ViewController.View is Microsoft.Maui.Platform.ContentView cv )
            return cv;

        throw new InvalidOperationException($"PageViewController.View must be of type {nameof(Microsoft.Maui.Platform.ContentView)}");
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

            var tbi = Page.ToolbarItems.FirstOrDefault(tbi => tbi.Priority == int.MinValue);
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